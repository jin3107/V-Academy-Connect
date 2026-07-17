using MayNghien.Infrastructures.Repository;
using VAcademyConnect.Application.Interfaces.Repositories;
using VAcademyConnect.Domain.Entities;
using VAcademyConnect.Infrastructure.Persistence.Data;

namespace VAcademyConnect.Infrastructure.Persistence.Repositories
{
    public class ScoreStatisticRepository : GenericRepository<ScoreStatistic, ApplicationDbContext, ApplicationUser>, IScoreStatisticRepository
    {
        public ScoreStatisticRepository(ApplicationDbContext unitOfWork) : base(unitOfWork)
        {
        }
    }
}
