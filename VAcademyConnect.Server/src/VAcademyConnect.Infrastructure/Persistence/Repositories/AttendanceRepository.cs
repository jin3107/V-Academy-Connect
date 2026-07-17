using MayNghien.Infrastructures.Repository;
using VAcademyConnect.Application.Interfaces.Repositories;
using VAcademyConnect.Domain.Entities;
using VAcademyConnect.Infrastructure.Persistence.Data;

namespace VAcademyConnect.Infrastructure.Persistence.Repositories
{
    public class AttendanceRepository : GenericRepository<Attendance, ApplicationDbContext, ApplicationUser>, IAttendanceRepository
    {
        public AttendanceRepository(ApplicationDbContext unitOfWork) : base(unitOfWork)
        {
        }
    }
}
