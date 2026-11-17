using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskManagement.Api.DTOs.Tasks;
using TaskManagement.Api.Enums;
using TaskManagement.Api.Services;
using TaskManagement.API.Services;
using TaskStatus = TaskManagement.Api.Enums.TaskStatus;

namespace TaskManagement.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class TasksController : ControllerBase
{
    private readonly ITaskService _taskService;
    private readonly ICurrentUserService _currentUserService;

    public TasksController(
        ITaskService taskService,
        ICurrentUserService currentUserService)
    {
        _taskService = taskService;
        _currentUserService = currentUserService;
    }

    // =========================================
    // CREATE
    // POST /api/tasks
    // =========================================

    [HttpPost]
    public async Task<ActionResult<TaskResponse>> CreateTask(
        [FromBody] CreateTaskRequest request)
    {
        try
        {
            var task = await _taskService.CreateTaskAsync(
                _currentUserService.UserId,
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

    // =========================================
    // LIST / SEARCH / FILTER / SORT
    //
    // GET /api/tasks
    //
    // Examples:
    //
    // /api/tasks
    // /api/tasks?search=angular
    // /api/tasks?status=Pending
    // /api/tasks?priority=High
    // /api/tasks?categoryId=1
    // /api/tasks?due=overdue
    // /api/tasks?due=upcoming
    // /api/tasks?due=none
    // /api/tasks?sortBy=dueDate&sortOrder=asc
    //
    // =========================================

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
                _currentUserService.UserId,
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

    // =========================================
    // GET BY ID
    // GET /api/tasks/{id}
    // =========================================

    [HttpGet("{id:int}")]
    public async Task<ActionResult<TaskResponse>> GetTaskById(
        int id)
    {
        var task = await _taskService.GetTaskByIdAsync(
            _currentUserService.UserId,
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

    // =========================================
    // UPDATE
    // PUT /api/tasks/{id}
    // =========================================

    [HttpPut("{id:int}")]
    public async Task<ActionResult<TaskResponse>> UpdateTask(
        int id,
        [FromBody] UpdateTaskRequest request)
    {
        try
        {
            var task = await _taskService.UpdateTaskAsync(
                _currentUserService.UserId,
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

    // =========================================
    // UPDATE STATUS ONLY
    // PATCH /api/tasks/{id}/status
    // =========================================

    [HttpPatch("{id:int}/status")]
    public async Task<ActionResult<TaskResponse>> UpdateTaskStatus(
        int id,
        [FromBody] UpdateTaskStatusRequest request)
    {
        var task = await _taskService.UpdateTaskStatusAsync(
            _currentUserService.UserId,
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

    // =========================================
    // DELETE
    // DELETE /api/tasks/{id}
    // =========================================

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteTask(
        int id)
    {
        bool deleted = await _taskService.DeleteTaskAsync(
            _currentUserService.UserId,
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
