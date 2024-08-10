using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Threading.Tasks;
using TaskManagerApp.Models;
using Blazored.LocalStorage;
using System.IO;
using System.Text.Json;

namespace TaskManagerApp.Services
{
    public class AuthenticationService
    {
        private readonly HttpClient _httpClient;
        private readonly ILocalStorageService _localStorage;
        private string _token;

        public AuthenticationService(HttpClient httpClient, ILocalStorageService localStorage)
        {
            _httpClient = httpClient;
            _localStorage = localStorage;
        }

        public async Task<string> RegisterAsync(RegisterModel registerModel)
        {
            var response = await _httpClient.PostAsJsonAsync("http://localhost:5182/api/UserRegistration/register", registerModel);
            return response.IsSuccessStatusCode ? "Success" : "Failure";
        }

        public async Task<string> LoginAsync(LoginModel loginModel)
        {
            var response = await _httpClient.PostAsJsonAsync("http://localhost:5182/api/UserLogin/login", loginModel);

            if (response.IsSuccessStatusCode)
            {
                var loginResponse = await response.Content.ReadFromJsonAsync<LoginResponseModel>();
                _token = loginResponse.Token;
                await _localStorage.SetItemAsync("authToken", _token);
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _token);
                return "Success";
            }
            else
            {
                return "Failure";
            }
        }

        private async Task SetAuthorizationHeaderAsync()
        {
            var token = await _localStorage.GetItemAsync<string>("authToken");
            if (!string.IsNullOrEmpty(token))
            {
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }
        }

        public async Task<UserProfile> GetUserProfileAsync()
        {
            await SetAuthorizationHeaderAsync();
            return await _httpClient.GetFromJsonAsync<UserProfile>("http://localhost:5182/api/UserProfile/me");
        }

        public async Task<string> GetUserProfileImageAsync()
        {
            await SetAuthorizationHeaderAsync();
            var response = await _httpClient.GetAsync("http://localhost:5182/api/UserProfile/me/profile-image");
            if (response.IsSuccessStatusCode)
            {
                var imageBytes = await response.Content.ReadAsByteArrayAsync();
                return Convert.ToBase64String(imageBytes);
            }
            return null;
        }

        public async Task<HttpResponseMessage> UpdateUserProfileAsync(UserProfile userProfile)
        {
            await SetAuthorizationHeaderAsync();
            return await _httpClient.PutAsJsonAsync("http://localhost:5182/api/UserProfile/me", userProfile);
        }

        public async Task<string> UploadProfileImageAsync(Stream fileStream, string fileName)
        {
            await SetAuthorizationHeaderAsync();
            var content = new MultipartFormDataContent();
            var streamContent = new StreamContent(fileStream);
            streamContent.Headers.ContentType = new MediaTypeHeaderValue("image/jpeg"); // Adjust as needed
            content.Add(streamContent, "file", fileName);

            var response = await _httpClient.PostAsync("http://localhost:5182/api/UserProfile/me/upload-image", content);

            if (response.IsSuccessStatusCode)
            {
                var jsonResponse = await response.Content.ReadAsStringAsync();
                using (JsonDocument doc = JsonDocument.Parse(jsonResponse))
                {
                    JsonElement root = doc.RootElement;
                    if (root.TryGetProperty("imageUrl", out JsonElement imageUrlElement))
                    {
                        return imageUrlElement.GetString();
                    }
                }
            }
            return null;
        }

       public async Task<bool> ChangePasswordAsync(ChangePasswordModel changePasswordModel)
{
    await SetAuthorizationHeaderAsync();
    var response = await _httpClient.PostAsJsonAsync("http://localhost:5182/api/UserProfile/me/change-password", changePasswordModel);
    return response.IsSuccessStatusCode;
}

    }

    public class LoginResponseModel
    {
        public string Token { get; set; }
    }
}
