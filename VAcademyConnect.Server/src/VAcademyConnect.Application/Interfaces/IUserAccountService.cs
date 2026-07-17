using VAcademyConnect.Domain.Entities;

namespace VAcademyConnect.Application.Interfaces
{
    public interface IUserAccountService
    {
        Task<ApplicationUser?> FindByEmailAsync(string email);
        Task<bool> ExistsByEmailOrPhoneAsync(string email, string phoneNumber);
        Task<(bool Succeeded, IEnumerable<string> Errors, ApplicationUser? User)> CreateAsync(ApplicationUser user, string password);
        Task<(bool Succeeded, IEnumerable<string> Errors)> UpdateAsync(ApplicationUser user);
        Task DeleteAsync(ApplicationUser user);
        Task AssignRoleAsync(ApplicationUser user, string role);
    }
}
