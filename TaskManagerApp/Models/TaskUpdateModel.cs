public class TaskUpdateModel
{
    public string TaskId { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public DateTime DueDate { get; set; }
    public TimeSpan DueTime { get; set; }
    public string Status { get; set; }
    public string Priority { get; set; }
}
