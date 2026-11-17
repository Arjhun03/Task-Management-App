using Microsoft.EntityFrameworkCore;
using TaskManagement.Api.Data;
using TaskManagement.Api.DTOs.Tasks;
using TaskManagement.Api.Enums;
using TaskManagement.Api.Models;
using TaskStatus = TaskManagement.Api.Enums.TaskStatus;

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
        // Category must belong to the current user.
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

            Title = request.Title,

            Description = request.Description,

            // New tasks always start as Pending.
            Status = TaskStatus.Pending,

            Priority = request.Priority,

            DueDate = request.DueDate,

            CategoryId = request.CategoryId,

            CreatedAt = DateTime.UtcNow,

            UpdatedAt = null
        };

        _context.Tasks.Add(task);

        await _context.SaveChangesAsync();

        return await BuildTaskResponseAsync(task.TaskId);
    }

    // =========================================
    // GET ALL TASKS
    // =========================================

    public async Task<List<TaskResponse>> GetTasksAsync(
        int userId)
    {
        return await _context.Tasks
            .AsNoTracking()
            .Where(t => t.UserId == userId)
            .Include(t => t.Category)
            .OrderByDescending(t => t.CreatedAt)
            .Select(t => new TaskResponse
            {
                TaskId = t.TaskId,

                Title = t.Title,

                Description = t.Description,

                Status = t.Status,

                Priority = t.Priority,

                DueDate = t.DueDate,

                CategoryId = t.CategoryId,

                CategoryName = t.Category != null
                    ? t.Category.Name
                    : null,

                CreatedAt = t.CreatedAt,

                UpdatedAt = t.UpdatedAt
            })
            .ToListAsync();
    }

    // =========================================
    // GET TASK BY ID
    // =========================================

    public async Task<TaskResponse?> GetTaskByIdAsync(
        int userId,
        int taskId)
    {
        return await _context.Tasks
            .AsNoTracking()
            .Where(t =>
                t.TaskId == taskId &&
                t.UserId == userId)
            .Include(t => t.Category)
            .Select(t => new TaskResponse
            {
                TaskId = t.TaskId,

                Title = t.Title,

                Description = t.Description,

                Status = t.Status,

                Priority = t.Priority,

                DueDate = t.DueDate,

                CategoryId = t.CategoryId,

                CategoryName = t.Category != null
                    ? t.Category.Name
                    : null,

                CreatedAt = t.CreatedAt,

                UpdatedAt = t.UpdatedAt
            })
            .FirstOrDefaultAsync();
    }

    // =========================================
    // UPDATE TASK
    // =========================================

    public async Task<TaskResponse?> UpdateTaskAsync(
        int userId,
        int taskId,
        UpdateTaskRequest request)
    {
        var task = await _context.Tasks
            .FirstOrDefaultAsync(t =>
                t.TaskId == taskId &&
                t.UserId == userId);

        if (task == null)
        {
            return null;
        }

        // Category must belong to the current user.
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

        task.Title = request.Title;

        task.Description = request.Description;

        task.Status = request.Status;

        task.Priority = request.Priority;

        task.DueDate = request.DueDate;

        task.CategoryId = request.CategoryId;

        task.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        return await BuildTaskResponseAsync(task.TaskId);
    }

    // =========================================
    // UPDATE STATUS ONLY
    // =========================================

    public async Task<TaskResponse?> UpdateTaskStatusAsync(
        int userId,
        int taskId,
        TaskStatus status)
    {
        var task = await _context.Tasks
            .FirstOrDefaultAsync(t =>
                t.TaskId == taskId &&
                t.UserId == userId);

        if (task == null)
        {
            return null;
        }

        task.Status = status;

        task.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        return await BuildTaskResponseAsync(task.TaskId);
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
    // TASK DISCOVERY
    // =========================================

    public async Task<List<TaskResponse>> DiscoverTasksAsync(
        int userId,
        string? search,
        TaskStatus? status,
        TaskPriority? priority,
        int? categoryId,
        string? due,
        string? sortBy,
        string? sortOrder)
    {
        IQueryable<TaskItem> query = _context.Tasks
            .AsNoTracking()
            .Where(t => t.UserId == userId)
            .Include(t => t.Category);

        // =========================================
        // SEARCH
        // =========================================

        if (!string.IsNullOrWhiteSpace(search))
        {
            string searchValue = search.Trim();

            query = query.Where(t =>
                EF.Functions.Like(
                    t.Title,
                    $"%{searchValue}%"));
        }

        // =========================================
        // STATUS FILTER
        // =========================================

        if (status.HasValue)
        {
            query = query.Where(t =>
                t.Status == status.Value);
        }

        // =========================================
        // PRIORITY FILTER
        // =========================================

        if (priority.HasValue)
        {
            query = query.Where(t =>
                t.Priority == priority.Value);
        }

        // =========================================
        // CATEGORY FILTER
        // =========================================

        if (categoryId.HasValue)
        {
            // Since the base query is already scoped
            // to userId, the category must be attached
            // to one of this user's tasks.
            query = query.Where(t =>
                t.CategoryId == categoryId.Value);
        }

        // =========================================
        // DUE FILTER
        // =========================================

        if (!string.IsNullOrWhiteSpace(due))
        {
            string dueValue = due.Trim().ToLowerInvariant();

            DateTime now = DateTime.UtcNow;

            switch (dueValue)
            {
                case "overdue":

                    query = query.Where(t =>
                        t.DueDate.HasValue &&
                        t.DueDate.Value < now &&
                        t.Status != TaskStatus.Completed);

                    break;

                case "upcoming":

                    // Upcoming = due within the next 7 days.
                    DateTime upcomingLimit = now.AddDays(7);

                    query = query.Where(t =>
                        t.DueDate.HasValue &&
                        t.DueDate.Value >= now &&
                        t.DueDate.Value <= upcomingLimit &&
                        t.Status != TaskStatus.Completed);

                    break;

                case "none":

                    query = query.Where(t =>
                        !t.DueDate.HasValue);

                    break;

                default:

                    throw new ArgumentException(
                        "Invalid due filter. Use overdue, upcoming, or none.");
            }
        }

        // =========================================
        // SORTING
        // =========================================

        bool descending =
            string.Equals(
                sortOrder,
                "desc",
                StringComparison.OrdinalIgnoreCase);

        switch (sortBy?.Trim().ToLowerInvariant())
        {
            case "duedate":

                query = descending
                    ? query.OrderByDescending(t => t.DueDate)
                    : query.OrderBy(t => t.DueDate);

                break;

            case "createdat":

                query = descending
                    ? query.OrderByDescending(t => t.CreatedAt)
                    : query.OrderBy(t => t.CreatedAt);

                break;

            case "title":

                query = descending
                    ? query.OrderByDescending(t => t.Title)
                    : query.OrderBy(t => t.Title);

                break;

            case null:
            case "":

                query = query.OrderByDescending(
                    t => t.CreatedAt);

                break;

            default:

                throw new ArgumentException(
                    "Invalid sortBy. Use dueDate, createdAt, or title.");
        }

        // =========================================
        // RESPONSE
        // =========================================

        return await query
            .Select(t => new TaskResponse
            {
                TaskId = t.TaskId,

                Title = t.Title,

                Description = t.Description,

                Status = t.Status,

                Priority = t.Priority,

                DueDate = t.DueDate,

                CategoryId = t.CategoryId,

                CategoryName = t.Category != null
                    ? t.Category.Name
                    : null,

                CreatedAt = t.CreatedAt,

                UpdatedAt = t.UpdatedAt
            })
            .ToListAsync();
    }

    // =========================================
    // BUILD RESPONSE
    // =========================================

    private async Task<TaskResponse> BuildTaskResponseAsync(
        int taskId)
    {
        var response = await _context.Tasks
            .AsNoTracking()
            .Where(t => t.TaskId == taskId)
            .Include(t => t.Category)
            .Select(t => new TaskResponse
            {
                TaskId = t.TaskId,

                Title = t.Title,

                Description = t.Description,

                Status = t.Status,

                Priority = t.Priority,

                DueDate = t.DueDate,

                CategoryId = t.CategoryId,

                CategoryName = t.Category != null
                    ? t.Category.Name
                    : null,

                CreatedAt = t.CreatedAt,

                UpdatedAt = t.UpdatedAt
            })
            .FirstAsync();

        return response;
    }
}