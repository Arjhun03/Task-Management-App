using TaskManagement.Api.Enums;
using TaskStatus = TaskManagement.Api.Enums.TaskStatus;

namespace TaskManagement.Api.DTOs.Tasks;

public class UpdateTaskStatusRequest
{
    public TaskStatus Status { get; set; }
}
