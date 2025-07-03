using System.ComponentModel.DataAnnotations;


namespace TaskManagementBackend.DTOs;

public class UpdateUserDto
{
  public string? FirstName { get; set; }

  public string? LastName { get; set; }

  [EmailAddress]
  public string? Email { get; set; }

  public string? Role { get; set; }

  public bool? IsActive { get; set; }

  public int? SupervisorId { get; set; }
}
