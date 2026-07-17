using MayNghien.Infrastructures.Repository;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VAcademyConnect.Infrastructure.Persistence.Data;
using VAcademyConnect.Domain.Entities;
using VAcademyConnect.Application.Interfaces.Repositories;

namespace VAcademyConnect.Infrastructure.Persistence.Repositories
{
    public class RefreshTokenRepository : GenericRepository<RefreshTokenModel, ApplicationDbContext, ApplicationUser>, IRefreshTokenRepository
    {
        public RefreshTokenRepository(ApplicationDbContext unitOfWork) : base(unitOfWork)
        {
        }

        public async Task<RefreshTokenModel?> GetByTokenAsync(string refreshToken)
        {
            return await _context.RefreshTokenModels.FirstOrDefaultAsync(r => r.RefreshToken == refreshToken && !r.IsRevoked);
        }
    }
}
