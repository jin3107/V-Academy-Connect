using MayNghien.Infrastructures.Models.Responses;
using VAcademyConnect.DTOs.Requests;
using VAcademyConnect.DTOs.Responses;

namespace VAcademyConnect.Application.Interfaces.LessonEvaluations
{
    public interface IUpdateLessonEvaluationHandler
    {
        Task<AppResponse<LessonEvaluationResponse>> HandleAsync(LessonEvaluationRequest request);
    }
}
