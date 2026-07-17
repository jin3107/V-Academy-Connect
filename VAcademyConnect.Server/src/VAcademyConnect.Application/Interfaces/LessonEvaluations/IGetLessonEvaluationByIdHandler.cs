using MayNghien.Infrastructures.Models.Responses;
using VAcademyConnect.DTOs.Responses;

namespace VAcademyConnect.Application.Interfaces.LessonEvaluations
{
    public interface IGetLessonEvaluationByIdHandler
    {
        Task<AppResponse<LessonEvaluationResponse>> HandleAsync(Guid id);
    }
}
