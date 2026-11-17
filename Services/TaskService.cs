using Microsoft.EntityFrameworkCore;
using TaskManagement.Api.Data;
using TaskManagement.Api.DTOs.Tasks;
using TaskManagement.Api.Enums;
using TaskManagement.Api.Models;

namespace TaskManagement.Api.Services;

public class TaskService : ITaskService
{
    private readonly ApplicationDbContext _context;

    public TaskService(ApplicationDbContext context)
    {
        _context = context;
    }

    // =========================================
    // CREATE TASK
    // =========================================

    public async Task<TaskResponse> CreateTaskAsync(
        int userId,
        CreateTaskRequest request)
    {
        // Validate title
        if (string.IsNullOrWhiteSpace(request.Title))
        {
            throw new ArgumentException("Task title is required.");
        }

        // Validate category ownership if CategoryId is provided
        if (request.CategoryId.HasValue)
        {
            bool categoryExists = await _context.Categories
                .AnyAsync(c =>
                    c.CategoryId == request.CategoryId.Value &&
                    c.UserId == userId);

            if (!categoryExists)
            {
                throw new ArgumentException(
                    "The selected category does not belong to the current user.");
            }
        }

        var task = new TaskItem
        {
            UserId = userId,
            Title = request.Title.Trim(),
            Description = request.Description,
            Status = TaskStatus.Pending,
            Priority = request.Priority,
            DueDate = request.DueDate,
            CategoryId = request.CategoryId,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = null
        };

        _context.Tasks.Add(task);

        await _context.SaveChangesAsync();

        return MapToResponse(task);
    }


    // =========================================
    // GET ALL TASKS
    // =========================================

    public async Task<IEnumerable<TaskResponse>> GetTasksAsync(
        int userId)
    {
        var tasks = await _context.Tasks
            .AsNoTracking()
            .Where(t => t.UserId == userId)
            .OrderByDescending(t => t.CreatedAt)
            .ToListAsync();

        return tasks.Select(MapToResponse);
    }


    // =========================================
    // GET TASK BY ID
    // =========================================

    public async Task<TaskResponse?> GetTaskByIdAsync(
        int userId,
        int taskId)
    {
        var task = await _context.Tasks
            .AsNoTracking()
            .FirstOrDefaultAsync(t =>
                t.TaskId == taskId &&
                t.UserId == userId);

        if (task == null)
        {
            return null;
        }

        return MapToResponse(task);
    }


    // =========================================
    // UPDATE TASK
    // =========================================

    public async Task<TaskResponse?> UpdateTaskAsync(
        int userId,
        int taskId,
        UpdateTaskRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Title))
        {
            throw new ArgumentException("Task title is required.");
        }

        var task = await _context.Tasks
            .FirstOrDefaultAsync(t =>
                t.TaskId == taskId &&
                t.UserId == userId);

        if (task == null)
        {
            return null;
        }

        // Validate category ownership
        if (request.CategoryId.HasValue)
        {
            bool categoryExists = await _context.Categories
                .AnyAsync(c =>
                    c.CategoryId == request.CategoryId.Value &&
                    c.UserId == userId);

            if (!categoryExists)
            {
                throw new ArgumentException(
                    "The selected category does not belong to the current user.");
            }
        }

        task.Title = request.Title.Trim();
        task.Description = request.Description;
        task.Priority = request.Priority;
        task.Status = request.Status;
        task.DueDate = request.DueDate;
        task.CategoryId = request.CategoryId;
        task.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        return MapToResponse(task);
    }


    // =========================================
    // UPDATE STATUS
    // =========================================

    public async Task<bool> UpdateTaskStatusAsync(
        int userId,
        int taskId,
        UpdateTaskStatusRequest request)
    {
        var task = await _context.Tasks
            .FirstOrDefaultAsync(t =>
                t.TaskId == taskId &&
                t.UserId == userId);

        if (task == null)
        {
            return false;
        }

        task.Status = request.Status;
        task.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        return true;
    }


    // =========================================
    // DELETE TASK
    // =========================================

    public async Task<bool> DeleteTaskAsync(
        int userId,
        int taskId)
    {
        var task = await _context.Tasks
            .FirstOrDefaultAsync(t =>
                t.TaskId == taskId &&
                t.UserId == userId);

        if (task == null)
        {
            return false;
        }

        _context.Tasks.Remove(task);

        await _context.SaveChangesAsync();

        return true;
    }


    // =========================================
    // MAPPING
    // =========================================

    private static TaskResponse MapToResponse(TaskItem task)
    {
        return new TaskResponse
        {
            TaskId = task.TaskId,
            UserId = task.UserId,
            Title = task.Title,
            Description = task.Description,
            Status = task.Status,
            Priority = task.Priority,
            DueDate = task.DueDate,
            CategoryId = task.CategoryId,
            CreatedAt = task.CreatedAt,
            UpdatedAt = task.UpdatedAt
        };
    }
}