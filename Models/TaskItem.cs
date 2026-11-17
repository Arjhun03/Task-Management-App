using TaskManagement.Api.Enums;
using TaskStatus = TaskManagement.Api.Enums.TaskStatus;

namespace TaskManagement.Api.Models;

public class TaskItem
{
    public int TaskId { get; set; }

    public int UserId { get; set; }

    public string Title { get; set; } = string.Empty;

    public string? Description { get; set; }

    public TaskStatus Status { get; set; } = TaskStatus.Pending;

    public TaskPriority Priority { get; set; } = TaskPriority.Medium;

    public DateTime? DueDate { get; set; }

    public int? CategoryId { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    // Navigation properties
    public Users User { get; set; } = null!;

    public Category? Category { get; set; }
}
