using MayNghien.Infrastructures.Models.Requests;
using MayNghien.Infrastructures.Models.Responses;
using VAcademyConnect.DTOs.Responses;

namespace VAcademyConnect.Application.Interfaces.ScoreStatistics
{
    public interface ISearchScoreStatisticHandler
    {
        Task<AppResponse<SearchResponse<ScoreStatisticResponse>>> HandleAsync(SearchRequest request);
    }
}
