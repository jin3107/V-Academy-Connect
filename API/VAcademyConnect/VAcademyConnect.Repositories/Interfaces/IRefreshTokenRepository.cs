using MayNghien.Infrastructures.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VAcademyConnect.Models.Data;
using VAcademyConnect.Models.Entities;

namespace VAcademyConnect.Repositories.Interfaces
{
    public interface IRefreshTokenRepository : IGenericRepository<RefreshTokenModel, ApplicationDbContext, ApplicationUser>
    {
        Task<RefreshTokenModel?> GetByTokenAsync(string refreshToken);
    }
}
