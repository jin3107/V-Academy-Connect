using VAcademyConnect.Commons.Enums;
using VAcademyConnect.Domain.Entities;

namespace VAcademyConnect.Application.Interfaces.Repositories
{
    public interface IOtpCodeRepository
    {
        Task AddAsync(OtpCode entity);
        Task UpdateAsync(OtpCode entity);
        Task<OtpCode?> FindActiveAsync(string email, OtpPurpose purpose);
        Task<OtpCode?> FindRecentlyVerifiedAsync(string email, OtpPurpose purpose);
        Task InvalidatePreviousAsync(string email, OtpPurpose purpose);
        Task ClearExpiredAsync();
    }
}
