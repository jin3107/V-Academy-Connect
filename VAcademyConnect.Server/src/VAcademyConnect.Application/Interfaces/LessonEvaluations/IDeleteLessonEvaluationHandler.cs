using MayNghien.Infrastructures.Models.Responses;

namespace VAcademyConnect.Application.Interfaces.LessonEvaluations
{
    public interface IDeleteLessonEvaluationHandler
    {
        Task<AppResponse<string>> HandleAsync(Guid id);
    }
}
