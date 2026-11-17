using TaskManagement.Api.Enums;

namespace TaskManagement.Api.Models;

public class TaskItem
{
    public int TaskId { get; set; }

    public int UserId { get; set; }

    public string Title { get; set; } = string.Empty;

    public string? Description { get; set; }

    public TaskManagement.Api.Enums.TaskStatus Status { get; set; } = TaskManagement.Api.Enums.TaskStatus.Pending;

    public TaskPriority Priority { get; set; } = TaskPriority.Medium;

    public DateTime? DueDate { get; set; }

    public int? CategoryId { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public Users Users { get; set; } = null!;

    public Category? Category { get; set; }
}
