using VAcademyConnect.Domain.Entities;

namespace VAcademyConnect.Application.Interfaces
{
    public interface ICurrentUserService
    {
        Task<ApplicationUser?> GetCurrentUserAsync();
        Task<IList<string>> GetRolesAsync(ApplicationUser user);
    }
}
