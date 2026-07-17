using MayNghien.Infrastructures.Repository;
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
    public class StaffRepository : GenericRepository<Staff, ApplicationDbContext, ApplicationUser>, IStaffRepository
    {
        public StaffRepository(ApplicationDbContext unitOfWork) : base(unitOfWork)
        {
        }
    }
}
