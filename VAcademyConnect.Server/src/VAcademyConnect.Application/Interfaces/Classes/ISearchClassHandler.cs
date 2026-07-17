using MayNghien.Infrastructures.Models.Requests;
using MayNghien.Infrastructures.Models.Responses;
using VAcademyConnect.DTOs.Responses;

namespace VAcademyConnect.Application.Interfaces.Classes
{
    public interface ISearchClassHandler
    {
        Task<AppResponse<SearchResponse<ClassResponse>>> HandleAsync(SearchRequest request);
    }
}
