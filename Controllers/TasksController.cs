using Microsoft.AspNetCore.Mvc;
using TaskManagement.Api.DTOs.Tasks;
using TaskManagement.Api.Enums;
using TaskManagement.Api.Services;
using TaskStatus = TaskManagement.Api.Enums.TaskStatus;

namespace TaskManagement.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TasksController : ControllerBase
{
    private readonly ITaskService _taskService;

    // =========================================
    // TEMPORARY DEVELOPMENT USER
    // =========================================
    //
    // Developer 2 will later provide the actual
    // authenticated user ID through
    // ICurrentUserService.
    //
    // DO NOT keep this in the final version.
    // =========================================

    private const int DevelopmentUserId = 1;

    public TasksController(
        ITaskService taskService)
    {
        _taskService = taskService;
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
                DevelopmentUserId,
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
                DevelopmentUserId,
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
            DevelopmentUserId,
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
                DevelopmentUserId,
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
            DevelopmentUserId,
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
            DevelopmentUserId,
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