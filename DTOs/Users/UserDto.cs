namespace TaskManagementBackend.DTOs;

public class UserDto
{
  public int Id { get; set; }
  public string FirstName { get; set; } = string.Empty;
  public string LastName { get; set; } = string.Empty;
  public string Email { get; set; } = string.Empty;
  public string Role { get; set; } = "employee";
  public bool IsActive { get; set; } = true;
  public int? SupervisorId { get; set; }
  public string? SupervisorName { get; set; }
  public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
