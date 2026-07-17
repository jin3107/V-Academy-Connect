using Microsoft.EntityFrameworkCore;
using VAcademyConnect.Application.Interfaces.Repositories;
using VAcademyConnect.Commons.Enums;
using VAcademyConnect.Domain.Entities;
using VAcademyConnect.Infrastructure.Persistence.Data;

namespace VAcademyConnect.Infrastructure.Persistence.Repositories
{
    public class OtpCodeRepository : IOtpCodeRepository
    {
        private readonly ApplicationDbContext _context;

        public OtpCodeRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(OtpCode entity)
        {
            entity.CreatedOn ??= DateTime.UtcNow;
            await _context.OtpCodes.AddAsync(entity);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(OtpCode entity)
        {
            entity.ModifiedOn = DateTime.UtcNow;
            _context.OtpCodes.Update(entity);
            await _context.SaveChangesAsync();
        }

        public async Task<OtpCode?> FindActiveAsync(string email, OtpPurpose purpose)
        {
            var normalizedEmail = email.Trim().ToLowerInvariant();
            return await _context.OtpCodes
                .Where(o => !o.IsDeleted && o.Email.ToLower() == normalizedEmail
                    && o.Purpose == purpose && !o.IsUsed && o.ExpiresAt > DateTime.UtcNow)
                .OrderByDescending(o => o.CreatedOn)
                .FirstOrDefaultAsync();
        }

        public async Task<OtpCode?> FindRecentlyVerifiedAsync(string email, OtpPurpose purpose)
        {
            var normalizedEmail = email.Trim().ToLowerInvariant();
            var window = DateTime.UtcNow.AddMinutes(-10);
            return await _context.OtpCodes
                .Where(o => !o.IsDeleted && o.Email.ToLower() == normalizedEmail
                    && o.Purpose == purpose && o.IsUsed && o.ModifiedOn >= window)
                .OrderByDescending(o => o.ModifiedOn)
                .FirstOrDefaultAsync();
        }

        public async Task InvalidatePreviousAsync(string email, OtpPurpose purpose)
        {
            var normalizedEmail = email.Trim().ToLowerInvariant();
            var activeCodes = await _context.OtpCodes
                .Where(o => !o.IsDeleted && o.Email.ToLower() == normalizedEmail
                    && o.Purpose == purpose && !o.IsUsed)
                .ToListAsync();

            foreach (var code in activeCodes)
            {
                code.IsUsed = true;
                code.ModifiedOn = DateTime.UtcNow;
            }

            if (activeCodes.Count > 0)
                await _context.SaveChangesAsync();
        }

        public async Task ClearExpiredAsync()
        {
            var expired = await _context.OtpCodes
                .Where(o => o.ExpiresAt <= DateTime.UtcNow || o.IsUsed)
                .ToListAsync();

            if (expired.Count == 0)
                return;

            _context.OtpCodes.RemoveRange(expired);
            await _context.SaveChangesAsync();
        }
    }
}
