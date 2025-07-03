namespace TaskManagementBackend.DTOs;

public class UpdateTaskDto
{
  public string? Title { get; set; }

  public string? Description { get; set; }

  public string? Status { get; set; }

  public string? Priority { get; set; }

  public string? AssignedTo { get; set; }

  public string? AssignedBy { get; set; }

  public DateTime? DueDate { get; set; }

  public List<string>? Tags { get; set; }
}
