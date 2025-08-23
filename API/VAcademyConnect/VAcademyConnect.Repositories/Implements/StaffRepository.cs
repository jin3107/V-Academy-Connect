using MayNghien.Infrastructures.Repository;
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
    public class StaffRepository : GenericRepository<Staff, ApplicationDbContext, ApplicationUser>, IStaffRepository
    {
        public StaffRepository(ApplicationDbContext unitOfWork) : base(unitOfWork)
        {
        }
    }
}
