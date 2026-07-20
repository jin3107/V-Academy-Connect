using MayNghien.Infrastructures.Repository;
using VAcademyConnect.Application.Interfaces.Repositories;
using VAcademyConnect.Domain.Entities;
using VAcademyConnect.Infrastructure.Persistence.Data;

namespace VAcademyConnect.Infrastructure.Persistence.Repositories
{
    public class ScoreRepository : GenericRepository<Score, ApplicationDbContext, ApplicationUser>, IScoreRepository
    {
        public ScoreRepository(ApplicationDbContext unitOfWork) : base(unitOfWork)
        {
        }
    }
}
