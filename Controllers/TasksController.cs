using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using TaskManagement.API.Services;
using TaskManagement.Api.DTOs.Tasks;
using TaskManagement.Api.Enums;
using TaskManagement.Api.Services;
using TaskStatus = TaskManagement.Api.Enums.TaskStatus;

namespace TaskManagement.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class TasksController : ControllerBase
{
    private readonly ITaskService _taskService;

    private readonly ICurrentUserService _currentUser;

    public TasksController(ITaskService taskService, ICurrentUserService currentUser)
    {
        _taskService = taskService;
        _currentUser = currentUser;
    }

    // =========================================================
    // CREATE TASK
    // POST: /api/Tasks
    // =========================================================

    [HttpPost]
    public async Task<ActionResult<TaskResponse>> CreateTask(
        [FromBody] CreateTaskRequest request)
    {
        try
        {
            var task = await _taskService.CreateTaskAsync(
                _currentUser.UserId,
                request);

            return CreatedAtAction(
                nameof(GetTaskById),
                new { id = task.TaskId },
                task);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new
            {
                message = ex.Message
            });
        }
    }

    // =========================================================
    // GET / SEARCH / FILTER / SORT
    // GET: /api/Tasks
    //
    // Examples:
    // /api/Tasks
    // /api/Tasks?search=angular
    // /api/Tasks?status=Pending
    // /api/Tasks?priority=High
    // /api/Tasks?categoryId=1
    // /api/Tasks?due=overdue
    // /api/Tasks?due=upcoming
    // /api/Tasks?due=none
    // /api/Tasks?sortBy=dueDate&sortOrder=asc
    // =========================================================

    [HttpGet]
    public async Task<ActionResult<List<TaskResponse>>> GetTasks(
        [FromQuery] string? search,
        [FromQuery] TaskStatus? status,
        [FromQuery] TaskPriority? priority,
        [FromQuery] int? categoryId,
        [FromQuery] string? due,
        [FromQuery] string? sortBy,
        [FromQuery] string? sortOrder)
    {
        try
        {
            var tasks = await _taskService.DiscoverTasksAsync(
                _currentUser.UserId,
                search,
                status,
                priority,
                categoryId,
                due,
                sortBy,
                sortOrder);

            return Ok(tasks);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new
            {
                message = ex.Message
            });
        }
    }

    // =========================================================
    // GET TASK BY ID
    // GET: /api/Tasks/{id}
    // =========================================================

    [HttpGet("{id:int}")]
    public async Task<ActionResult<TaskResponse>> GetTaskById(
        int id)
    {
        var task = await _taskService.GetTaskByIdAsync(
            _currentUser.UserId,
            id);

        if (task == null)
        {
            return NotFound(new
            {
                message = "Task not found."
            });
        }

        return Ok(task);
    }

    // =========================================================
    // UPDATE TASK
    // PUT: /api/Tasks/{id}
    // =========================================================

    [HttpPut("{id:int}")]
    public async Task<ActionResult<TaskResponse>> UpdateTask(
        int id,
        [FromBody] UpdateTaskRequest request)
    {
        try
        {
            var task = await _taskService.UpdateTaskAsync(
                _currentUser.UserId,
                id,
                request);

            if (task == null)
            {
                return NotFound(new
                {
                    message = "Task not found."
                });
            }

            return Ok(task);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new
            {
                message = ex.Message
            });
        }
    }

    // =========================================================
    // UPDATE TASK STATUS
    // PATCH: /api/Tasks/{id}/status
    // =========================================================

    [HttpPatch("{id:int}/status")]
    public async Task<ActionResult<TaskResponse>> UpdateTaskStatus(
        int id,
        [FromBody] UpdateTaskStatusRequest request)
    {
        var task = await _taskService.UpdateTaskStatusAsync(
            _currentUser.UserId,
            id,
            request.Status);

        if (task == null)
        {
            return NotFound(new
            {
                message = "Task not found."
            });
        }

        return Ok(task);
    }

    // =========================================================
    // DELETE TASK
    // DELETE: /api/Tasks/{id}
    // =========================================================

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteTask(
        int id)
    {
        var deleted = await _taskService.DeleteTaskAsync(
            _currentUser.UserId,
            id);

        if (!deleted)
        {
            return NotFound(new
            {
                message = "Task not found."
            });
        }

        return NoContent();
    }
}