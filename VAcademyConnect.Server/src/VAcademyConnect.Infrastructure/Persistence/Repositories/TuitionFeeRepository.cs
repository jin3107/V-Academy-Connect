using MayNghien.Infrastructures.Repository;
using VAcademyConnect.Application.Interfaces.Repositories;
using VAcademyConnect.Domain.Entities;
using VAcademyConnect.Infrastructure.Persistence.Data;

namespace VAcademyConnect.Infrastructure.Persistence.Repositories
{
    public class TuitionFeeRepository : GenericRepository<TuitionFee, ApplicationDbContext, ApplicationUser>, ITuitionFeeRepository
    {
        public TuitionFeeRepository(ApplicationDbContext unitOfWork) : base(unitOfWork)
        {
        }
    }
}
