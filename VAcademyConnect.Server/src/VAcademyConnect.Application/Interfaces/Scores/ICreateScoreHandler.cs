using MayNghien.Infrastructures.Models.Responses;
using VAcademyConnect.DTOs.Requests;
using VAcademyConnect.DTOs.Responses;

namespace VAcademyConnect.Application.Interfaces.Scores
{
    public interface ICreateScoreHandler
    {
        Task<AppResponse<ScoreResponse>> HandleAsync(ScoreRequest request);
    }
}
