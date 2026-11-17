using TaskManagement.Api.DTOs.Tasks;

namespace TaskManagement.Api.Services;

public interface ITaskService
{
    Task<TaskResponse> CreateTaskAsync(
        int userId,
        CreateTaskRequest request);

    Task<IEnumerable<TaskResponse>> GetTasksAsync(
        int userId);

    Task<TaskResponse?> GetTaskByIdAsync(
        int userId,
        int taskId);

    Task<TaskResponse?> UpdateTaskAsync(
        int userId,
        int taskId,
        UpdateTaskRequest request);

    Task<bool> UpdateTaskStatusAsync(
        int userId,
        int taskId,
        UpdateTaskStatusRequest request);

    Task<bool> DeleteTaskAsync(
        int userId,
        int taskId);
}