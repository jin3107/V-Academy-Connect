using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using VAcademyConnect.Application.Interfaces;
using VAcademyConnect.Domain.Entities;

namespace VAcademyConnect.Infrastructure.Identity
{
    public class CurrentUserService : ICurrentUserService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IHttpContextAccessor _contextAccessor;

        public CurrentUserService(UserManager<ApplicationUser> userManager, IHttpContextAccessor contextAccessor)
        {
            _userManager = userManager;
            _contextAccessor = contextAccessor;
        }

        public async Task<ApplicationUser?> GetCurrentUserAsync()
        {
            var email = _contextAccessor.HttpContext?.User.Identity?.Name;
            if (string.IsNullOrEmpty(email))
                return null;

            return await _userManager.FindByEmailAsync(email);
        }

        public async Task<IList<string>> GetRolesAsync(ApplicationUser user)
        {
            return await _userManager.GetRolesAsync(user);
        }
    }
}
