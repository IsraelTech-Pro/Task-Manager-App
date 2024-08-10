namespace TaskManagerApp.Models
{
    public class TaskModel
    {
        public string TaskId { get; set; }
        public string UserId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime DueDate { get; set; }
        public TimeSpan DueTime { get; set; }
        public string Status { get; set; }
        public string Priority { get; set; }
        public string ImageUpload { get; set; }
    }
}
