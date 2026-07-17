using Microsoft.AspNetCore.Identity;
using VAcademyConnect.Application.Interfaces.Authentication;
using VAcademyConnect.Domain.Entities;

namespace VAcademyConnect.Infrastructure.Services.Implements.Authentication
{
    public class LogoutHandler : ILogoutHandler
    {
        private readonly SignInManager<ApplicationUser> _signInManager;

        public LogoutHandler(SignInManager<ApplicationUser> signInManager)
        {
            _signInManager = signInManager;
        }

        public async Task HandleAsync()
        {
            await _signInManager.SignOutAsync();
        }
    }
}
