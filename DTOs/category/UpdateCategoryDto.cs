using System.ComponentModel.DataAnnotations;

namespace Task_Management_App.DTOs.Category;

public class UpdateCategoryDto
{
    [Required]
    [StringLength(60)]
    public string Name { get; set; } = string.Empty;

    [StringLength(250)]
    public string? Description { get; set; }
}