using System.ComponentModel.DataAnnotations;

namespace TaskManagementBackend.DTOs;

public class CreateTaskDto
{
  [Required]
  public string Title { get; set; } = string.Empty;

  [Required]
  public string Description { get; set; } = string.Empty;

  [Required]
  public string Status { get; set; } = "pending";

  [Required]
  public string Priority { get; set; } = "medium";

  [Required]
  public string AssignedTo { get; set; } = string.Empty;

  [Required]
  public string AssignedBy { get; set; } = string.Empty;

  public DateTime? DueDate { get; set; }

  public List<string>? Tags { get; set; }
}
