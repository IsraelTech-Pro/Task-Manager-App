namespace TaskManagementAPI.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Fname { get; set; } = string.Empty;
        public string Sname { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string ProfileImage { get; set; } = string.Empty; // This can be NULL if intended
    }
}
