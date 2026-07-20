using MayNghien.Infrastructures.Models.Requests;
using MayNghien.Infrastructures.Models.Responses;
using VAcademyConnect.DTOs.Responses;

namespace VAcademyConnect.Application.Interfaces.BackgroundJobs
{
    public interface ISearchBackgroundJobHandler
    {
        Task<AppResponse<SearchResponse<BackgroundJobResponse>>> HandleAsync(SearchRequest request);
    }
}
