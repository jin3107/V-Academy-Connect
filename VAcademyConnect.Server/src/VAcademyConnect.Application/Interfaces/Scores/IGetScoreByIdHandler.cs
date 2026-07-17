using MayNghien.Infrastructures.Models.Responses;
using VAcademyConnect.DTOs.Responses;

namespace VAcademyConnect.Application.Interfaces.Scores
{
    public interface IGetScoreByIdHandler
    {
        Task<AppResponse<ScoreResponse>> HandleAsync(Guid id);
    }
}
