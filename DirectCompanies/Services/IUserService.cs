using DirectCompanies.Dtos;
using DirectCompanies.Models;

namespace DirectCompanies.Services
{
    public interface IUserService
    {
        public Task HandleUsersSentFromErp(List<ApplicationUserDto> ApplicationUsers);

        public Task<ApplicationUser> GetLoggedUser();


    }
}
