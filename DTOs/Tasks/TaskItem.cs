namespace TaskManagementBackend.DTOs;

public class TaskItem
{
  public string Id { get; set; } = string.Empty;

  public string Title { get; set; } = string.Empty;

  public string Description { get; set; } = string.Empty;

  public string Status { get; set; } = "pending";

  public string Priority { get; set; } = "medium";

  public string AssignedTo { get; set; } = string.Empty;

  public string AssignedBy { get; set; } = string.Empty;

  public string CreatedAt { get; set; } = string.Empty; // ISO 8601

  public string UpdatedAt { get; set; } = string.Empty;

  public string? DueDate { get; set; }

  public List<string>? Tags { get; set; }
}
