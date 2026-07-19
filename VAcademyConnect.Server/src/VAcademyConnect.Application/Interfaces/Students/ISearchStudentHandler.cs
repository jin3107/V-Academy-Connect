using MayNghien.Infrastructures.Models.Requests;
using MayNghien.Infrastructures.Models.Responses;
using VAcademyConnect.DTOs.Responses;

namespace VAcademyConnect.Application.Interfaces.Students
{
    public interface ISearchStudentHandler
    {
        Task<AppResponse<SearchResponse<StudentResponse>>> HandleAsync(SearchRequest request);
    }
}
