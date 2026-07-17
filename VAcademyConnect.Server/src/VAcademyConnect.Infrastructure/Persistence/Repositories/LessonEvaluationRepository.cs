using MayNghien.Infrastructures.Repository;
using VAcademyConnect.Application.Interfaces.Repositories;
using VAcademyConnect.Domain.Entities;
using VAcademyConnect.Infrastructure.Persistence.Data;

namespace VAcademyConnect.Infrastructure.Persistence.Repositories
{
    public class LessonEvaluationRepository : GenericRepository<LessonEvaluation, ApplicationDbContext, ApplicationUser>, ILessonEvaluationRepository
    {
        public LessonEvaluationRepository(ApplicationDbContext unitOfWork) : base(unitOfWork)
        {
        }
    }
}
