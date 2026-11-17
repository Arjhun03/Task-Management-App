using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskManagement.Api.DTOs.Tasks;
using TaskManagement.Api.Services;

namespace TaskManagement.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
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
    // CREATE TASK
    // POST: /api/tasks
    // =========================================

    [HttpPost]
    public async Task<ActionResult<TaskResponse>> CreateTask(
        [FromBody] CreateTaskRequest request)
    {
        try
        {
            int userId = _currentUserService.UserId;

            var task = await _taskService.CreateTaskAsync(
                userId,
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
    // GET ALL TASKS
    // GET: /api/tasks
    // =========================================

    [HttpGet]
    public async Task<ActionResult<IEnumerable<TaskResponse>>> GetTasks()
    {
        int userId = _currentUserService.UserId;

        var tasks = await _taskService.GetTasksAsync(userId);

        return Ok(tasks);
    }


    // =========================================
    // GET TASK BY ID
    // GET: /api/tasks/{id}
    // =========================================

    [HttpGet("{id:int}")]
    public async Task<ActionResult<TaskResponse>> GetTaskById(
        int id)
    {
        int userId = _currentUserService.UserId;

        var task = await _taskService.GetTaskByIdAsync(
            userId,
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
    // UPDATE TASK
    // PUT: /api/tasks/{id}
    // =========================================

    [HttpPut("{id:int}")]
    public async Task<ActionResult<TaskResponse>> UpdateTask(
        int id,
        [FromBody] UpdateTaskRequest request)
    {
        try
        {
            int userId = _currentUserService.UserId;

            var task = await _taskService.UpdateTaskAsync(
                userId,
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
    // UPDATE TASK STATUS
    // PATCH: /api/tasks/{id}/status
    // =========================================

    [HttpPatch("{id:int}/status")]
    public async Task<ActionResult> UpdateTaskStatus(
        int id,
        [FromBody] UpdateTaskStatusRequest request)
    {
        int userId = _currentUserService.UserId;

        bool updated = await _taskService.UpdateTaskStatusAsync(
            userId,
            id,
            request);

        if (!updated)
        {
            return NotFound(new
            {
                message = "Task not found."
            });
        }

        return Ok(new
        {
            message = "Task status updated successfully."
        });
    }


    // =========================================
    // DELETE TASK
    // DELETE: /api/tasks/{id}
    // =========================================

    [HttpDelete("{id:int}")]
    public async Task<ActionResult> DeleteTask(
        int id)
    {
        int userId = _currentUserService.UserId;

        bool deleted = await _taskService.DeleteTaskAsync(
            userId,
            id);

        if (!deleted)
        {
            return NotFound(new
            {
                message = "Task not found."
            });
        }

        return Ok(new
        {
            message = "Task deleted successfully."
        });
    }
}