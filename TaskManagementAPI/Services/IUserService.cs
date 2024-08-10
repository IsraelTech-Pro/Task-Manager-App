using System.Threading.Tasks;
using TaskManagementAPI.Models;

namespace TaskManagementAPI.Services
{
    public interface IUserService
    {
        Task<UserProfile> GetUserProfileAsync(string email);
        Task<bool> UpdateUserProfileAsync(string email, UserProfileUpdateModel model);
        Task<bool> UpdateUserProfileImageAsync(string email, string imageUrl);
        Task<bool> ChangePasswordAsync(string email, ChangePasswordModel model);
    }
}
