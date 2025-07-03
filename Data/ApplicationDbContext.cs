using Microsoft.EntityFrameworkCore;
using TaskManagementBackend.Models;

namespace TaskManagementBackend.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<User> Users => Set<User>();
    public DbSet<TaskModel> Tasks { get; set; }
}
