using MayNghien.Infrastructures.Repository;
using VAcademyConnect.Application.Interfaces.Repositories;
using VAcademyConnect.Domain.Entities;
using VAcademyConnect.Infrastructure.Persistence.Data;

namespace VAcademyConnect.Infrastructure.Persistence.Repositories
{
    public class PaymentRepository : GenericRepository<Payment, ApplicationDbContext, ApplicationUser>, IPaymentRepository
    {
        public PaymentRepository(ApplicationDbContext unitOfWork) : base(unitOfWork)
        {
        }
    }
}
