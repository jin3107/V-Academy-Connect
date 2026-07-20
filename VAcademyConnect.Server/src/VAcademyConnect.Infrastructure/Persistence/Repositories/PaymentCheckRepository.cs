using MayNghien.Infrastructures.Repository;
using VAcademyConnect.Application.Interfaces.Repositories;
using VAcademyConnect.Domain.Entities;
using VAcademyConnect.Infrastructure.Persistence.Data;

namespace VAcademyConnect.Infrastructure.Persistence.Repositories
{
    public class PaymentCheckRepository : GenericRepository<PaymentCheck, ApplicationDbContext, ApplicationUser>, IPaymentCheckRepository
    {
        public PaymentCheckRepository(ApplicationDbContext unitOfWork) : base(unitOfWork)
        {
        }
    }
}
