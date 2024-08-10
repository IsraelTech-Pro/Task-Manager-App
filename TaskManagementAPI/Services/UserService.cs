using System.Threading.Tasks;
using TaskManagementAPI.Models;
using Microsoft.AspNetCore.Identity;
using TaskManagementAPI.Data;

namespace TaskManagementAPI.Services
{
    public class UserService : IUserService
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public UserService(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task<UserProfile> GetUserProfileAsync(string email)
        {
            var user = await _userManager.FindByEmailAsync(email); 
            if (user == null)
                return null;

            return new UserProfile
            {
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                ProfileImage = user.ProfileImage
            };
        }

        public async Task<bool> UpdateUserProfileAsync(string email, UserProfileUpdateModel model)
        {
            var user = await _userManager.FindByEmailAsync(email); 
            if (user == null)
                return false;

            user.FirstName = model.FirstName;
            user.LastName = model.LastName;
            user.ProfileImage = model.ProfileImage;

            var result = await _userManager.UpdateAsync(user);
            return result.Succeeded;
        }

        public async Task<bool> UpdateUserProfileImageAsync(string email, string imageUrl)
        {
            var user = await _userManager.FindByEmailAsync(email); 
            if (user == null)
                return false;

            user.ProfileImage = imageUrl;

            var result = await _userManager.UpdateAsync(user);
            return result.Succeeded;
        }

        public async Task<bool> ChangePasswordAsync(string email, ChangePasswordModel model)
        {
            var user = await _userManager.FindByEmailAsync(email); 
            if (user == null)
                return false;

            var result = await _userManager.ChangePasswordAsync(user, model.CurrentPassword, model.NewPassword);
            return result.Succeeded;
        }
    }
}
