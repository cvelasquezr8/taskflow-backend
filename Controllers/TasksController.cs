using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TaskManagementBackend.Data;
using TaskManagementBackend.DTOs;
using TaskManagementBackend.Models;

namespace TaskManagementBackend.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class TasksController : ControllerBase
{
  private readonly ApplicationDbContext _context;

  public TasksController(ApplicationDbContext context)
  {
    _context = context;
  }

  // Map TaskModel to TaskItem (DTO)
  private TaskItem MapToDto(TaskModel task) => new TaskItem
  {
    Id = task.Id,
    Title = task.Title,
    Description = task.Description,
    Status = task.Status,
    Priority = task.Priority,
    AssignedTo = task.AssignedTo,
    AssignedBy = task.AssignedBy,
    CreatedAt = task.CreatedAt.ToString("o"),
    UpdatedAt = task.UpdatedAt.ToString("o"),
    DueDate = task.DueDate?.ToString("o"),
    Tags = task.Tags?.Split(',').ToList()
  };


  private int GetUserId()
  {
    return int.Parse(User.Claims.First(c => c.Type == "userId").Value);
  }


  [HttpGet]
  public async Task<ActionResult<IEnumerable<TaskItem>>> GetTasks()
  {
    var userId = GetUserId();

    var currentUser = await _context.Users
        .Include(u => u.SupervisedEmployees)
        .FirstOrDefaultAsync(u => u.Id == userId);

    if (currentUser == null)
      return Unauthorized();

    IQueryable<TaskModel> query = _context.Tasks.AsQueryable();

    if (currentUser.Role == "admin")
    {
      // Admin: can see all tasks
    }
    else if (currentUser.Role == "supervisor")
    {
      var supervisedIds = currentUser.SupervisedEmployees?
          .Select(e => e.Id.ToString())
          .ToList() ?? new List<string>();

      supervisedIds.Add(currentUser.Id.ToString());

      query = query.Where(t => t.AssignedTo != null && supervisedIds.Contains(t.AssignedTo));
    }
    else // employee
    {
      query = query.Where(t => t.AssignedTo == currentUser.Id.ToString());
    }

    var tasks = await query.ToListAsync();
    return tasks.Select(MapToDto).ToList();
  }

  [HttpGet("{id}")]
  public async Task<ActionResult<TaskItem>> GetTask(string id)
  {
    var task = await _context.Tasks.FindAsync(id);
    if (task == null)
      return NotFound();

    return MapToDto(task);
  }

  [HttpPost]
  public async Task<ActionResult<TaskItem>> CreateTask(CreateTaskDto dto)
  {
    var task = new TaskModel
    {
      Title = dto.Title,
      Description = dto.Description,
      Status = dto.Status,
      Priority = dto.Priority,
      AssignedTo = dto.AssignedTo,
      AssignedBy = dto.AssignedBy,
      DueDate = dto.DueDate,
      Tags = dto.Tags != null ? string.Join(",", dto.Tags) : null,
      CreatedAt = DateTime.UtcNow,
      UpdatedAt = DateTime.UtcNow
    };

    _context.Tasks.Add(task);
    await _context.SaveChangesAsync();

    return CreatedAtAction(nameof(GetTask), new { id = task.Id }, MapToDto(task));
  }

  [HttpPut("{id}")]
  public async Task<IActionResult> UpdateTask(string id, UpdateTaskDto dto)
  {
    var task = await _context.Tasks.FindAsync(id);
    if (task == null)
      return NotFound();

    task.Title = dto.Title ?? task.Title;
    task.Description = dto.Description ?? task.Description;
    task.Status = dto.Status ?? task.Status;
    task.Priority = dto.Priority ?? task.Priority;
    task.AssignedTo = dto.AssignedTo ?? task.AssignedTo;
    task.AssignedBy = dto.AssignedBy ?? task.AssignedBy;
    task.DueDate = dto.DueDate ?? task.DueDate;
    task.Tags = dto.Tags != null ? string.Join(",", dto.Tags) : task.Tags;
    task.UpdatedAt = DateTime.UtcNow;

    await _context.SaveChangesAsync();
    return Ok(MapToDto(task));
  }

  [HttpDelete("{id}")]
  public async Task<IActionResult> DeleteTask(string id)
  {
    var task = await _context.Tasks.FindAsync(id);
    if (task == null)
      return NotFound();

    _context.Tasks.Remove(task);
    await _context.SaveChangesAsync();

    return Ok(MapToDto(task));
  }
}
