using MayNghien.Infrastructures.Models.Responses;
using Microsoft.EntityFrameworkCore;
using VAcademyConnect.Application.Interfaces;
using VAcademyConnect.Application.Interfaces.Repositories;
using VAcademyConnect.Application.Interfaces.ScoreStatistics;
using VAcademyConnect.Application.Mapping;
using VAcademyConnect.DTOs.Responses;

namespace VAcademyConnect.Application.Implementations.ScoreStatistics
{
    public class GetScoreStatisticByIdHandler : IGetScoreStatisticByIdHandler
    {
        private readonly IScoreStatisticRepository _scoreStatisticRepository;
        private readonly ICurrentUserService _currentUserService;

        public GetScoreStatisticByIdHandler(IScoreStatisticRepository scoreStatisticRepository, ICurrentUserService currentUserService)
        {
            _scoreStatisticRepository = scoreStatisticRepository;
            _currentUserService = currentUserService;
        }

        public async Task<AppResponse<ScoreStatisticResponse>> HandleAsync(Guid id)
        {
            var result = new AppResponse<ScoreStatisticResponse>();
            try
            {
                var user = await _currentUserService.GetCurrentUserAsync();
                if (user == null)
                    return result.BuildError("Unauthorize.");

                var scoreStatistic = await _scoreStatisticRepository.FindByAsync(x => x.Id == id)
                    .Include(x => x.Student).FirstOrDefaultAsync();
                if (scoreStatistic == null || scoreStatistic.IsDeleted)
                    return result.BuildError("Score statistic record was not found or has been deleted.");

                var response = ScoreStatisticMapper.ToResponse(scoreStatistic);
                result.BuildResult(response, "Score statistic has been found.");
            }
            catch (Exception ex)
            {
                result.BuildError(ex.Message + " " + ex.StackTrace);
            }
            return result;
        }
    }
}
