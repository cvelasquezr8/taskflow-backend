using System.Security.Cryptography;
using System.Text;
using Microsoft.EntityFrameworkCore;
using TaskManagementBackend.Data;
using TaskManagementBackend.Models;

namespace TaskManagementBackend.Seed;

public static class DbSeeder
{
  public static void SeedRootUser(IServiceProvider services)
  {
    using var scope = services.CreateScope();
    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

    var existingUser = context.Users.FirstOrDefault(u => u.Email == "root@taskflow.com");
    if (existingUser != null)
    {
      Console.WriteLine("⚠️  Root user already exists.");
      return;
    }

    using var hmac = new HMACSHA512();
    var password = "Admin123!";
    var rootUser = new User
    {
      FirstName = "System",
      LastName = "Admin",
      Email = "root@taskflow.com",
      PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password)),
      PasswordSalt = hmac.Key,
      Role = "admin"
    };

    context.Users.Add(rootUser);
    context.SaveChanges();

    Console.WriteLine("✅ Root user created with email: root@taskflow.com and password: Admin123!");
  }
}
