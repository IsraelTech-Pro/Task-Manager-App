using Microsoft.AspNetCore.Identity;

namespace TaskManagementAPI.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string ProfileImage { get; set; } = "default-profile-image-url"; // Set a default image URL
    }
}
