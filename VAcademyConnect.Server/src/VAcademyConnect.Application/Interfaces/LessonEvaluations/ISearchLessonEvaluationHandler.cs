using MayNghien.Infrastructures.Models.Requests;
using MayNghien.Infrastructures.Models.Responses;
using VAcademyConnect.DTOs.Responses;

namespace VAcademyConnect.Application.Interfaces.LessonEvaluations
{
    public interface ISearchLessonEvaluationHandler
    {
        Task<AppResponse<SearchResponse<LessonEvaluationResponse>>> HandleAsync(SearchRequest request);
    }
}
