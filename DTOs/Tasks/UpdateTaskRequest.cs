using TaskManagement.Api.Enums;
using TaskStatus = TaskManagement.Api.Enums.TaskStatus;

namespace TaskManagement.Api.DTOs.Tasks;

public class UpdateTaskRequest
{
    public string Title { get; set; } = string.Empty;

    public string? Description { get; set; }

    public TaskPriority Priority { get; set; }

    public TaskStatus Status { get; set; }

    public DateTime? DueDate { get; set; }

    public int? CategoryId { get; set; }
}
