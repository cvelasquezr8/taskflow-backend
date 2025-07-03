using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;
using TaskManagementBackend.Data;
using TaskManagementBackend.DTOs;
using TaskManagementBackend.Models;

namespace TaskManagementBackend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
  private readonly ApplicationDbContext _context;
  private readonly IConfiguration _config;

  public AuthController(ApplicationDbContext context, IConfiguration config)
  {
    _context = context;
    _config = config;
  }

  [HttpPost("register")]
  public async Task<IActionResult> Register(RegisterDto dto)
  {
    var emailExists = await _context.Users.AnyAsync(u => u.Email == dto.Email.ToLower());
    if (emailExists)
      return BadRequest("Email is already registered.");

    using var hmac = new HMACSHA512();

    var user = new User
    {
      FirstName = dto.FirstName,
      LastName = dto.LastName,
      Email = dto.Email.ToLower(),
      PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(dto.Password)),
      PasswordSalt = hmac.Key,
      Role = dto.Role
    };

    _context.Users.Add(user);
    await _context.SaveChangesAsync();

    var token = GenerateJwtToken(user);

    return Ok(new
    {
      token,
      user = new
      {
        user.Id,
        user.FirstName,
        user.LastName,
        user.Email,
        user.Role
      }
    });
  }

  private string GenerateJwtToken(User user)
  {
    var tokenHandler = new System.IdentityModel.Tokens.Jwt.JwtSecurityTokenHandler();
    var key = Encoding.ASCII.GetBytes(_config["Jwt:Key"]);

    var tokenDescriptor = new SecurityTokenDescriptor
    {
      Subject = new ClaimsIdentity(new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
             new Claim("userId", user.Id.ToString()),
            new Claim(ClaimTypes.Name, user.Email),
            new Claim(ClaimTypes.Role, user.Role)
        }),
      Expires = DateTime.UtcNow.AddHours(2),
      Issuer = _config["Jwt:Issuer"],
      SigningCredentials = new SigningCredentials(
            new SymmetricSecurityKey(key),
            SecurityAlgorithms.HmacSha256Signature)
    };

    var token = tokenHandler.CreateToken(tokenDescriptor);
    return tokenHandler.WriteToken(token);
  }

  [HttpPost("login")]
  public async Task<IActionResult> Login(LoginDto dto)
  {
    var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == dto.Email.ToLower());
    if (user == null)
      return Unauthorized("Invalid email or password.");

    using var hmac = new HMACSHA512(user.PasswordSalt);
    var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(dto.Password));

    if (!computedHash.SequenceEqual(user.PasswordHash))
      return Unauthorized("Invalid email or password.");

    var token = GenerateJwtToken(user);
    return Ok(new
    {
      token,
      user = new
      {
        user.Id,
        user.FirstName,
        user.LastName,
        user.Email,
        user.Role
      }
    });
  }
}
