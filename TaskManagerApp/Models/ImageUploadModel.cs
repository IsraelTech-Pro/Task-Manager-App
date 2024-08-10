namespace TaskManagerApp.Models
{
    public class ImageUploadModel
    {
      
        public string ImageUrl { get; set; }

     
        public string FileName { get; set; }
        public long FileSize { get; set; } 
        public string ContentType { get; set; }  
    }
}
