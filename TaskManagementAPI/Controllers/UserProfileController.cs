using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using System.Threading.Tasks;
using TaskManagementAPI.Models;
using TaskManagementAPI.Services;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;
using System.IO;

namespace TaskManagementAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserProfileController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly ILogger<UserProfileController> _logger;

        public UserProfileController(IUserService userService, ILogger<UserProfileController> logger)
        {
            _userService = userService;
            _logger = logger;
        }

        [HttpGet("me")]
        public async Task<IActionResult> GetUserProfile()
        {
            var email = User.FindFirstValue(ClaimTypes.Email);
            _logger.LogInformation("Request received to get user profile for email: {Email}", email);

            if (email == null)
            {
                _logger.LogWarning("Email is null. Unauthorized access attempt.");
                return Unauthorized("User is not authenticated.");
            }

            var userProfile = await _userService.GetUserProfileAsync(email);

            if (userProfile == null)
            {
                _logger.LogWarning("User profile not found for email: {Email}", email);
                return NotFound("User profile not found.");
            }

            _logger.LogInformation("User profile retrieved successfully for email: {Email}", email);
            return Ok(userProfile);
        }

        [HttpPut("me")]
        public async Task<IActionResult> UpdateUserProfile([FromBody] UserProfileUpdateModel model)
        {
            var email = User.FindFirstValue(ClaimTypes.Email);
            _logger.LogInformation("Request received to update user profile for email: {Email}", email);

            if (email == null)
            {
                _logger.LogWarning("Email is null. Unauthorized access attempt.");
                return Unauthorized("User is not authenticated.");
            }

            var result = await _userService.UpdateUserProfileAsync(email, model);

            if (result)
            {
                _logger.LogInformation("User profile updated successfully for email: {Email}", email);
                return Ok("Profile updated successfully.");
            }
            else
            {
                _logger.LogWarning("Failed to update user profile for email: {Email}", email);
                return BadRequest("Failed to update profile.");
            }
        }

        [HttpPost("me/upload-image")]
        public async Task<IActionResult> UploadProfileImage(IFormFile file)
        {
            var email = User.FindFirstValue(ClaimTypes.Email);
            if (email == null)
            {
                return Unauthorized("User is not authenticated.");
            }

            if (file == null || file.Length == 0)
            {
                return BadRequest("No file uploaded.");
            }

            var imgDirectory = Path.Combine(Directory.GetCurrentDirectory(), "img");
            if (!Directory.Exists(imgDirectory))
            {
                Directory.CreateDirectory(imgDirectory);
            }

            var filePath = Path.Combine(imgDirectory, file.FileName);
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            var imageUrl = $"/img/{file.FileName}";
            var result = await _userService.UpdateUserProfileImageAsync(email, imageUrl);

            if (result)
            {
                return Ok(new { ImageUrl = imageUrl });
            }
            else
            {
                return BadRequest("Failed to update profile image.");
            }
        }

        [HttpGet("me/profile-image")]
        public async Task<IActionResult> GetProfileImage()
        {
            var email = User.FindFirstValue(ClaimTypes.Email);
            if (email == null)
            {
                return Unauthorized("User is not authenticated.");
            }

            var userProfile = await _userService.GetUserProfileAsync(email);

            if (userProfile == null || string.IsNullOrEmpty(userProfile.ProfileImage))
            {
                return NotFound("Profile image not found.");
            }

            var imgDirectory = Path.Combine(Directory.GetCurrentDirectory(), "img");
            var filePath = Path.Combine(imgDirectory, Path.GetFileName(userProfile.ProfileImage));

            if (!System.IO.File.Exists(filePath))
            {
                return NotFound("Profile image not found on the server.");
            }

            var image = System.IO.File.OpenRead(filePath);
            return File(image, "image/jpeg");
        }

        [HttpPost("me/change-password")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordModel model)
        {
            var email = User.FindFirstValue(ClaimTypes.Email);
            if (email == null)
            {
                return Unauthorized("User is not authenticated.");
            }

            if (model.NewPassword != model.ConfirmNewPassword)
            {
                return BadRequest("New password and confirmation password do not match.");
            }

            var result = await _userService.ChangePasswordAsync(email, model);
            if (result)
            {
                return Ok("Password changed successfully.");
            }
            else
            {
                return BadRequest("Failed to change password.");
            }
        }
    }
}
