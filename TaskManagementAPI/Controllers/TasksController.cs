using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using TaskManagementAPI.Data;
using TaskManagementAPI.Models;

namespace TaskManagementAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class TasksController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly string _imagePath;
        private readonly string _defaultImagePath;

        public TasksController(ApplicationDbContext context, IConfiguration configuration)
        {
            _context = context;
            _imagePath = configuration["ImagePath"] ?? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "img");
            _defaultImagePath = "/img/profile.png"; // Path to the default image
        }

        // Helper method to get user by email
        private async Task<ApplicationUser> GetUserByEmail(string email)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
        }

        // POST: api/Tasks
        [HttpPost]
        public async Task<ActionResult<TaskModel>> CreateTask(TaskCreateModel model)
        {
            var userEmail = User.FindFirstValue(ClaimTypes.Email);
            if (userEmail == null)
            {
                return BadRequest("User email claim is missing.");
            }

            var user = await GetUserByEmail(userEmail);
            if (user == null)
            {
                return BadRequest("User not found.");
            }

            var task = new TaskModel
            {
                UserId = user.Id,
                Title = model.Title,
                Description = model.Description,
                DueDate = model.DueDate,
                DueTime = model.DueTime,
                Status = model.Status,
                Priority = model.Priority,
                ImageUpload = _defaultImagePath // Set default image
            };

            _context.Tasks.Add(task);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetTaskById), new { id = task.TaskId }, task);
        }

        // PUT: api/Tasks/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTask(string id, TaskCreateModel model)
        {
            var userEmail = User.FindFirstValue(ClaimTypes.Email);
            if (userEmail == null)
            {
                return BadRequest("User email claim is missing.");
            }

            var user = await GetUserByEmail(userEmail);
            if (user == null)
            {
                return BadRequest("User not found.");
            }

            var task = await _context.Tasks.FindAsync(id);

            if (task == null || task.UserId != user.Id)
            {
                return NotFound();
            }

            // Update fields except for ImageUpload
            task.Title = model.Title;
            task.Description = model.Description;
            task.DueDate = model.DueDate;
            task.DueTime = model.DueTime;
            task.Status = model.Status;
            task.Priority = model.Priority;

            _context.Entry(task).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TaskExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // PUT: api/Tasks/update-status/5
        [HttpPut("update-status/{id}")]
        public async Task<IActionResult> UpdateTaskStatus(string id, [FromBody] string status)
        {
            var userEmail = User.FindFirstValue(ClaimTypes.Email);
            if (userEmail == null)
            {
                return BadRequest("User email claim is missing.");
            }

            var user = await GetUserByEmail(userEmail);
            if (user == null)
            {
                return BadRequest("User not found.");
            }

            var task = await _context.Tasks.FindAsync(id);

            if (task == null || task.UserId != user.Id)
            {
                return NotFound();
            }

            task.Status = status;

            _context.Entry(task).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TaskExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // DELETE: api/Tasks/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTask(string id)
        {
            var userEmail = User.FindFirstValue(ClaimTypes.Email);
            if (userEmail == null)
            {
                return BadRequest("User email claim is missing.");
            }

            var user = await GetUserByEmail(userEmail);
            if (user == null)
            {
                return BadRequest("User not found.");
            }

            var task = await _context.Tasks.FindAsync(id);

            if (task == null || task.UserId != user.Id)
            {
                return NotFound();
            }

            _context.Tasks.Remove(task);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // POST: api/Tasks/upload-image/{taskId}
        [HttpPost("upload-image/{taskId}")]
        public async Task<IActionResult> UploadImage(string taskId, IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest("No file uploaded.");
            }

            if (!Directory.Exists(_imagePath))
            {
                Directory.CreateDirectory(_imagePath);
            }

            var fileName = Path.GetFileName(file.FileName);
            var filePath = Path.Combine(_imagePath, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            var task = await _context.Tasks.FindAsync(taskId);
            if (task == null)
            {
                return NotFound("Task not found.");
            }

            task.ImageUpload = $"/img/{fileName}";
            _context.Entry(task).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TaskExists(taskId))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Ok(new { fileUrl = task.ImageUpload });
        }

        // GET: api/Tasks/5/image
        [HttpGet("{id}/image")]
        public async Task<IActionResult> GetTaskImage(string id)
        {
            var task = await _context.Tasks.FindAsync(id);
            if (task == null || string.IsNullOrEmpty(task.ImageUpload))
            {
                return NotFound("Image not found.");
            }

            var filePath = Path.Combine(_imagePath, Path.GetFileName(task.ImageUpload));
            if (!System.IO.File.Exists(filePath))
            {
                return NotFound("Image not found on the server.");
            }

            var image = System.IO.File.OpenRead(filePath);
            return File(image, "image/jpeg");
        }

        // GET: api/Tasks/5
        [HttpGet("{id}")]
        public async Task<ActionResult<TaskModel>> GetTaskById(string id)
        {
            var userEmail = User.FindFirstValue(ClaimTypes.Email);
            if (userEmail == null)
            {
                return BadRequest("User email claim is missing.");
            }

            var user = await GetUserByEmail(userEmail);
            if (user == null)
            {
                return BadRequest("User not found.");
            }

            var task = await _context.Tasks.FindAsync(id);

            if (task == null || task.UserId != user.Id)
            {
                return NotFound();
            }

            // Create a full URL for the image
            task.ImageUpload = Request.Scheme + "://" + Request.Host + task.ImageUpload;

            return task;
        }

        // GET: api/Tasks
        [HttpGet]
        public async Task<ActionResult<IEnumerable<TaskModel>>> GetAllUserTasks()
        {
            var userEmail = User.FindFirstValue(ClaimTypes.Email);
            if (userEmail == null)
            {
                return BadRequest("User email claim is missing.");
            }

            var user = await GetUserByEmail(userEmail);
            if (user == null)
            {
                return BadRequest("User not found.");
            }

            var tasks = await _context.Tasks.Where(t => t.UserId == user.Id).ToListAsync();
            // Create full URLs for images
            foreach (var task in tasks)
            {
                task.ImageUpload = Request.Scheme + "://" + Request.Host + task.ImageUpload;
            }

            return Ok(tasks);
        }

        // GET: api/Tasks/counts
        [HttpGet("counts")]
        public async Task<ActionResult<TaskCountsDto>> GetTaskCounts()
        {
            var userEmail = User.FindFirstValue(ClaimTypes.Email);
            if (userEmail == null)
            {
                return BadRequest("User email claim is missing.");
            }

            var user = await GetUserByEmail(userEmail);
            if (user == null)
            {
                return BadRequest("User not found.");
            }

            var totalTasks = await _context.Tasks.CountAsync(t => t.UserId == user.Id);
            var completedTasks = await _context.Tasks.CountAsync(t => t.UserId == user.Id && t.Status == "Completed");
            var inProgressTasks = await _context.Tasks.CountAsync(t => t.UserId == user.Id && t.Status == "In Progress");
            var notStartedTasks = await _context.Tasks.CountAsync(t => t.UserId == user.Id && t.Status == "Not Started");

            var counts = new TaskCountsDto
            {
                Total = totalTasks,
                Completed = completedTasks,
                InProgress = inProgressTasks,
                NotStarted = notStartedTasks
            };

            return Ok(counts);
        }

        // DTO for task counts
        public class TaskCountsDto
        {
            public int Total { get; set; }
            public int Completed { get; set; }
            public int InProgress { get; set; }
            public int NotStarted { get; set; }
        }

        private bool TaskExists(string id)
        {
            return _context.Tasks.Any(e => e.TaskId == id);
        }
    }
}
