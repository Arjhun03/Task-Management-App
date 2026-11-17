using TaskManagement.Api.DTOs.Tasks;
using TaskManagement.Api.Enums;
using TaskStatus = TaskManagement.Api.Enums.TaskStatus;

namespace TaskManagement.Api.Services;

public interface ITaskService
{
    // =========================================
    // CREATE
    // =========================================

    Task<TaskResponse> CreateTaskAsync(
        int userId,
        CreateTaskRequest request);

    // =========================================
    // LIST
    // =========================================

    Task<List<TaskResponse>> GetTasksAsync(
        int userId);

    // =========================================
    // GET BY ID
    // =========================================

    Task<TaskResponse?> GetTaskByIdAsync(
        int userId,
        int taskId);

    // =========================================
    // UPDATE
    // =========================================

    Task<TaskResponse?> UpdateTaskAsync(
        int userId,
        int taskId,
        UpdateTaskRequest request);

    // =========================================
    // UPDATE STATUS ONLY
    // =========================================

    Task<TaskResponse?> UpdateTaskStatusAsync(
        int userId,
        int taskId,
        TaskStatus status);

    // =========================================
    // DELETE
    // =========================================

    Task<bool> DeleteTaskAsync(
        int userId,
        int taskId);

    // =========================================
    // TASK DISCOVERY
    // =========================================

    Task<List<TaskResponse>> DiscoverTasksAsync(
        int userId,
        string? search,
        TaskStatus? status,
        TaskPriority? priority,
        int? categoryId,
        string? due,
        string? sortBy,
        string? sortOrder);
}