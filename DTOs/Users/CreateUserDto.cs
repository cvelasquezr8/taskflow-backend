using System.ComponentModel.DataAnnotations;

namespace TaskManagementBackend.DTOs;

public class CreateUserDto
{
  [Required]
  public string FirstName { get; set; } = string.Empty;

  [Required]
  public string LastName { get; set; } = string.Empty;

  [Required]
  [EmailAddress]
  public string Email { get; set; } = string.Empty;

  public string Role { get; set; } = "employee";

  public bool IsActive { get; set; } = true;

  public int? SupervisorId { get; set; }
}
