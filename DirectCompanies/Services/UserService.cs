using DirectCompanies.Dtos;
using DirectCompanies.Enums;
using DirectCompanies.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace DirectCompanies.Services
{
    public class UserService:IUserService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly UserManager<ApplicationUser> _userManager;

        public UserService(IHttpContextAccessor httpContextAccessor,UserManager<ApplicationUser> userManager)
        {
            _httpContextAccessor = httpContextAccessor;
            _userManager = userManager;

        }
        public async Task HandleUsersSentFromErp(List<ApplicationUserDto> ApplicationUsers)
        {
            foreach (var ApplicationUser in ApplicationUsers)
            {
                var ExistingUser = await _userManager.FindByIdAsync(ApplicationUser.UserId.ToString());

                try
                {

                    if (ExistingUser == null)
                    {
                        var UserToAdd = new ApplicationUser { Id = ApplicationUser.UserId, CompanyName = ApplicationUser.CompanyName, UserName = ApplicationUser.UserName };
                        await _userManager.CreateAsync(UserToAdd, ApplicationUser.Password);

                    }
                    else
                    {
                        if (ApplicationUser.EventType == EventType.Modified)
                        {
                            ExistingUser.CompanyName = ApplicationUser.CompanyName;
                            ExistingUser.UserName = ApplicationUser.UserName;
                            ExistingUser.PasswordHash = ApplicationUser.Password;
                            ExistingUser.PasswordHash = _userManager.PasswordHasher.HashPassword(ExistingUser, ApplicationUser.Password);
                            await _userManager.UpdateAsync(ExistingUser);
                        }
                        else if (ApplicationUser.EventType == EventType.Deleted)
                        {
                            await _userManager.DeleteAsync(ExistingUser);

                        }

                    }

                }

                catch (Exception ex)
                {
                    throw new Exception(ex.Message);
                }
            }
        }
        public async Task<ApplicationUser> GetLoggedUser()
        {
            var UserName = _httpContextAccessor.HttpContext?.Request?.Cookies["UserName"];

            var user = await _userManager.FindByNameAsync(UserName);
            return user;
        }
    }
}
