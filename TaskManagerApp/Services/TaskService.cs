using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components.Forms;
using TaskManagerApp.Models;
using Blazored.LocalStorage;

namespace TaskManagerApp.Services
{
    public class TaskService
    {
        private readonly HttpClient _httpClient;
        private readonly ILocalStorageService _localStorage;

        private const string BaseApiUrl = "http://localhost:5182/api"; 

        public TaskService(HttpClient httpClient, ILocalStorageService localStorage)
        {
            _httpClient = httpClient;
            _localStorage = localStorage;
        }

        private async Task SetAuthorizationHeaderAsync()
        {
            var token = await _localStorage.GetItemAsync<string>("authToken");
            if (!string.IsNullOrEmpty(token))
            {
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }
        }

       
        public async Task<string> CreateTaskAsync(TaskCreateModel model)
        {
            await SetAuthorizationHeaderAsync();
            var response = await _httpClient.PostAsJsonAsync($"{BaseApiUrl}/Tasks", model); 
            response.EnsureSuccessStatusCode();

            var createdTask = await response.Content.ReadFromJsonAsync<TaskModel>();
            return createdTask.TaskId; 
        }

        public async Task<ImageUploadModel> UploadTaskImageAsync(string taskId, IBrowserFile imageFile)
        {
            await SetAuthorizationHeaderAsync();

            var content = new MultipartFormDataContent();
            var fileStream = imageFile.OpenReadStream(maxAllowedSize: 10 * 1024 * 1024); // Set max size as needed
            var fileContent = new StreamContent(fileStream);
            fileContent.Headers.ContentType = new MediaTypeHeaderValue(imageFile.ContentType);
            content.Add(fileContent, "file", imageFile.Name);

            var response = await _httpClient.PostAsync($"{BaseApiUrl}/Tasks/upload-image/{taskId}", content);
            response.EnsureSuccessStatusCode();

           
            return await response.Content.ReadFromJsonAsync<ImageUploadModel>();
        }

       
        public async Task<TaskCountsModel> GetTaskCountsAsync()
        {
            await SetAuthorizationHeaderAsync();
            var response = await _httpClient.GetAsync($"{BaseApiUrl}/Tasks/counts"); // Adjust endpoint as needed
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<TaskCountsModel>();
        }

       
        public async Task<List<TaskModel>> GetAllUserTasksAsync()
        {
            await SetAuthorizationHeaderAsync();
            var response = await _httpClient.GetAsync($"{BaseApiUrl}/Tasks"); // Adjust endpoint as needed
            response.EnsureSuccessStatusCode();
            var tasks = await response.Content.ReadFromJsonAsync<List<TaskModel>>();

           
            if (tasks != null)
            {
                foreach (var task in tasks)
                {
                    if (!string.IsNullOrEmpty(task.ImageUpload) && !task.ImageUpload.StartsWith("http"))
                    {
                        task.ImageUpload = $"{BaseApiUrl.TrimEnd('/')}{task.ImageUpload}"; 
                    }
                }
            }

            return tasks;
        }

        public async Task<TaskModel> GetTaskByIdAsync(string taskId)
        {
            await SetAuthorizationHeaderAsync();
            var response = await _httpClient.GetAsync($"{BaseApiUrl}/Tasks/{taskId}");
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<TaskModel>();
        }

       
        public async Task UpdateTaskAsync(string taskId, TaskUpdateModel model)
        {
            await SetAuthorizationHeaderAsync();
            var response = await _httpClient.PutAsJsonAsync($"{BaseApiUrl}/Tasks/{taskId}", model);
            response.EnsureSuccessStatusCode();
        }

        
        public async Task UpdateTaskStatusAsync(string taskId, string status)
        {
            await SetAuthorizationHeaderAsync();
            var response = await _httpClient.PutAsJsonAsync($"{BaseApiUrl}/Tasks/update-status/{taskId}", status);
            response.EnsureSuccessStatusCode();
        }


        public async Task DeleteTaskAsync(string taskId)
        {
            await SetAuthorizationHeaderAsync();
            var response = await _httpClient.DeleteAsync($"{BaseApiUrl}/Tasks/{taskId}");
            response.EnsureSuccessStatusCode();
        }
    }
}
