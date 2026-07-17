using MayNghien.Infrastructures.Models.Responses;

namespace VAcademyConnect.Application.Interfaces.ScoreStatistics
{
    public interface IDeleteScoreStatisticHandler
    {
        Task<AppResponse<string>> HandleAsync(Guid id);
    }
}
