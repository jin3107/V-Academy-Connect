using MayNghien.Infrastructures.Models.Requests;
using MayNghien.Infrastructures.Models.Responses;
using VAcademyConnect.DTOs.Responses;

namespace VAcademyConnect.Application.Interfaces.ClassStudents
{
    public interface ISearchClassStudentHandler
    {
        Task<AppResponse<SearchResponse<ClassStudentResponse>>> HandleAsync(SearchRequest request);
    }
}
