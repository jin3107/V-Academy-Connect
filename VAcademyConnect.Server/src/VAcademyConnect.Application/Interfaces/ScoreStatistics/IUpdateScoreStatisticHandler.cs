using MayNghien.Infrastructures.Models.Responses;
using VAcademyConnect.DTOs.Requests;
using VAcademyConnect.DTOs.Responses;

namespace VAcademyConnect.Application.Interfaces.ScoreStatistics
{
    public interface IUpdateScoreStatisticHandler
    {
        Task<AppResponse<ScoreStatisticResponse>> HandleAsync(ScoreStatisticRequest request);
    }
}
