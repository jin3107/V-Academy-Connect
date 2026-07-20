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
    public class ClassStudentRepository : GenericRepository<ClassStudent, ApplicationDbContext, ApplicationUser>, IClassStudentRepository
    {
        public ClassStudentRepository(ApplicationDbContext unitOfWork) : base(unitOfWork)
        {
        }
    }
}
