using MayNghien.Infrastructures.Models.Responses;
using VAcademyConnect.DTOs.Responses;

namespace VAcademyConnect.Application.Interfaces.ScoreStatistics
{
    public interface IGetScoreStatisticByIdHandler
    {
        Task<AppResponse<ScoreStatisticResponse>> HandleAsync(Guid id);
    }
}
