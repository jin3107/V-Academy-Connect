using MayNghien.Infrastructures.Repository;
using VAcademyConnect.Application.Interfaces.Repositories;
using VAcademyConnect.Domain.Entities;
using VAcademyConnect.Infrastructure.Persistence.Data;

namespace VAcademyConnect.Infrastructure.Persistence.Repositories
{
    public class BackgroundJobRepository : GenericRepository<BackgroundJob, ApplicationDbContext, ApplicationUser>, IBackgroundJobRepository
    {
        public BackgroundJobRepository(ApplicationDbContext unitOfWork) : base(unitOfWork)
        {
        }
    }
}
