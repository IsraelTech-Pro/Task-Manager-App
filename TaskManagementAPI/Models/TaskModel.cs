using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TaskManagementAPI.Models
{
    public class TaskModel
    {
        [Key]
        public string TaskId { get; set; } = Guid.NewGuid().ToString(); // Primary Key

        public string UserId { get; set; } // Foreign Key to User

        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime DueDate { get; set; }
        public TimeSpan DueTime { get; set; }
        public string Status { get; set; }
        public string Priority { get; set; }
        public string ImageUpload { get; set; }
    }
}
