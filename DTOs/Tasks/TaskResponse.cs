using TaskManagement.Api.Enums;
using TaskStatus = TaskManagement.Api.Enums.TaskStatus;

namespace TaskManagement.Api.DTOs.Tasks;

public class TaskResponse
{
    public int TaskId { get; set; }

    public int UserId { get; set; }

    public string Title { get; set; } = string.Empty;

    public string? Description { get; set; }

    public TaskStatus Status { get; set; }

    public TaskPriority Priority { get; set; }

    public DateTime? DueDate { get; set; }

    public int? CategoryId { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }
}
