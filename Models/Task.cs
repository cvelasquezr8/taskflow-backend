using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TaskManagementBackend.Models;

public class TaskModel
{
  [Key]
  public string Id { get; set; } = Guid.NewGuid().ToString();

  [Required]
  public string Title { get; set; } = string.Empty;

  [Required]
  public string Description { get; set; } = string.Empty;

  [Required]
  public string Status { get; set; } = "pending"; // pending | in-progress | completed

  [Required]
  public string Priority { get; set; } = "medium"; // low | medium | high

  [Required]
  public string AssignedTo { get; set; } = string.Empty; // user ID

  [Required]
  public string AssignedBy { get; set; } = string.Empty; // user ID

  [Required]
  public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

  [Required]
  public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

  public DateTime? DueDate { get; set; }

  public string? Tags { get; set; } // stored as comma-separated string
}
