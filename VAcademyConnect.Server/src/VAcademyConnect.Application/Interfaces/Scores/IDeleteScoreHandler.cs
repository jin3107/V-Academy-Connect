using MayNghien.Infrastructures.Models.Responses;

namespace VAcademyConnect.Application.Interfaces.Scores
{
    public interface IDeleteScoreHandler
    {
        Task<AppResponse<string>> HandleAsync(Guid id);
    }
}
