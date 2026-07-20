using MayNghien.Infrastructures.Models.Requests;
using MayNghien.Infrastructures.Models.Responses;
using VAcademyConnect.DTOs.Responses;

namespace VAcademyConnect.Application.Interfaces.Scores
{
    public interface ISearchScoreHandler
    {
        Task<AppResponse<SearchResponse<ScoreResponse>>> HandleAsync(SearchRequest request);
    }
}
