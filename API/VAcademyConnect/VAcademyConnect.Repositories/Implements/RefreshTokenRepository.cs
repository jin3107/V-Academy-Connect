using MayNghien.Infrastructures.Repository;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VAcademyConnect.Models.Data;
using VAcademyConnect.Models.Entities;
using VAcademyConnect.Repositories.Interfaces;

namespace VAcademyConnect.Repositories.Implements
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
