namespace TaskManagementAPI.Models
{
    public class TaskUpdateModel
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime DueDate { get; set; }
        public TimeSpan DueTime { get; set; }
        public string Status { get; set; }
        public string Priority { get; set; }
        public string ImageUpload { get; set; }
    }
}
