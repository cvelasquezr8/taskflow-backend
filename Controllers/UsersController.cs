using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TaskManagementBackend.Data;
using TaskManagementBackend.DTOs;
using TaskManagementBackend.Models;
using System.Security.Cryptography;
using System.Text;

namespace TaskManagementBackend.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class UsersController : ControllerBase
{
  private readonly ApplicationDbContext _context;

  public UsersController(ApplicationDbContext context)
  {
    _context = context;
  }

  [HttpGet]
  public async Task<ActionResult<IEnumerable<User>>> GetUsers()
  {
    var users = await _context.Users
       .Include(u => u.Supervisor)
       .Select(u => new UserDto
       {
         Id = u.Id,
         FirstName = u.FirstName,
         LastName = u.LastName,
         Email = u.Email,
         Role = u.Role,
         IsActive = u.IsActive,
         SupervisorId = u.SupervisorId,
         SupervisorName = u.Supervisor != null ? $"{u.Supervisor.FirstName} {u.Supervisor.LastName}" : null,
         CreatedAt = u.CreatedAt
       })
       .ToListAsync();

    return Ok(users);
  }

  [HttpGet("{id}")]
  public async Task<ActionResult<User>> GetUser(int id)
  {
    var user = await _context.Users
        .Include(u => u.Supervisor)
        .FirstOrDefaultAsync(u => u.Id == id);

    if (user == null)
      return NotFound();

    return user;
  }

  [HttpPost]
  public async Task<ActionResult<User>> CreateUser(CreateUserDto dto)
  {
    if (await _context.Users.AnyAsync(u => u.Email == dto.Email))
    {
      return BadRequest("A user with this email already exists.");
    }

    CreatePasswordHash("Temp1234!", out byte[] passwordHash, out byte[] passwordSalt); // You can change this temp password

    var user = new User
    {
      FirstName = dto.FirstName,
      LastName = dto.LastName,
      Email = dto.Email,
      PasswordHash = passwordHash,
      PasswordSalt = passwordSalt,
      Role = dto.Role,
      IsActive = dto.IsActive,
      SupervisorId = dto.SupervisorId
    };

    _context.Users.Add(user);
    await _context.SaveChangesAsync();

    return CreatedAtAction(nameof(GetUser), new { id = user.Id }, user);
  }

  [HttpPut("{id}")]
  public async Task<IActionResult> UpdateUser(int id, UpdateUserDto dto)
  {
    var user = await _context.Users.FindAsync(id);
    if (user == null)
      return NotFound();

    user.FirstName = dto.FirstName ?? user.FirstName;
    user.LastName = dto.LastName ?? user.LastName;
    user.Email = dto.Email ?? user.Email;
    user.Role = dto.Role ?? user.Role;
    user.IsActive = dto.IsActive ?? user.IsActive;
    user.SupervisorId = dto.SupervisorId ?? user.SupervisorId;

    await _context.SaveChangesAsync();

    return Ok(user);
  }

  [HttpDelete("{id}")]
  public async Task<IActionResult> DeleteUser(int id)
  {
    var user = await _context.Users.FindAsync(id);
    if (user == null)
      return NotFound();

    _context.Users.Remove(user);
    await _context.SaveChangesAsync();
    return NoContent();
  }

  private void CreatePasswordHash(string password, out byte[] hash, out byte[] salt)
  {
    using var hmac = new HMACSHA512();
    salt = hmac.Key;
    hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
  }
}
