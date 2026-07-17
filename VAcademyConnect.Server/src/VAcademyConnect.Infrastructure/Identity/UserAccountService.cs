using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using VAcademyConnect.Application.Interfaces;
using VAcademyConnect.Domain.Entities;

namespace VAcademyConnect.Infrastructure.Identity
{
    public class UserAccountService : IUserAccountService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public UserAccountService(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public Task<ApplicationUser?> FindByEmailAsync(string email) => _userManager.FindByEmailAsync(email);

        public async Task<bool> ExistsByEmailOrPhoneAsync(string email, string phoneNumber)
        {
            var userByEmail = await _userManager.FindByEmailAsync(email);
            if (userByEmail != null) return true;

            var userByPhoneNumber = await _userManager.Users.FirstOrDefaultAsync(x => x.PhoneNumber == phoneNumber);
            return userByPhoneNumber != null;
        }

        public async Task<(bool Succeeded, IEnumerable<string> Errors, ApplicationUser? User)> CreateAsync(ApplicationUser user, string password)
        {
            var result = await _userManager.CreateAsync(user, password);
            return (result.Succeeded, result.Errors.Select(e => e.Description), user);
        }

        public async Task<(bool Succeeded, IEnumerable<string> Errors)> UpdateAsync(ApplicationUser user)
        {
            var result = await _userManager.UpdateAsync(user);
            return (result.Succeeded, result.Errors.Select(e => e.Description));
        }

        public async Task DeleteAsync(ApplicationUser user)
        {
            await _userManager.DeleteAsync(user);
        }

        public async Task AssignRoleAsync(ApplicationUser user, string role)
        {
            var roles = await _userManager.GetRolesAsync(user);
            await _userManager.RemoveFromRolesAsync(user, roles);

            if (!await _roleManager.RoleExistsAsync(role))
                await _roleManager.CreateAsync(new IdentityRole(role));

            await _userManager.AddToRoleAsync(user, role);
        }
    }
}
