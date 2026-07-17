using MayNghien.Infrastructures.Models.Responses;
using VAcademyConnect.Application.Interfaces;
using VAcademyConnect.Application.Interfaces.Repositories;
using VAcademyConnect.Application.Interfaces.ScoreStatistics;

namespace VAcademyConnect.Application.Implementations.ScoreStatistics
{
    public class DeleteScoreStatisticHandler : IDeleteScoreStatisticHandler
    {
        private readonly IScoreStatisticRepository _scoreStatisticRepository;
        private readonly ICurrentUserService _currentUserService;

        public DeleteScoreStatisticHandler(IScoreStatisticRepository scoreStatisticRepository, ICurrentUserService currentUserService)
        {
            _scoreStatisticRepository = scoreStatisticRepository;
            _currentUserService = currentUserService;
        }

        public async Task<AppResponse<string>> HandleAsync(Guid id)
        {
            var result = new AppResponse<string>();
            try
            {
                var user = await _currentUserService.GetCurrentUserAsync();
                if (user == null)
                    return result.BuildError("Unauthorize");

                var scoreStatistic = await _scoreStatisticRepository.GetAsync(id);
                if (scoreStatistic == null || scoreStatistic.IsDeleted)
                    return result.BuildError("Score statistic record was not found or has been deleted.");

                scoreStatistic.IsDeleted = true;
                scoreStatistic.ModifiedBy = user.Email;
                scoreStatistic.ModifiedOn = DateTime.UtcNow;

                await _scoreStatisticRepository.EditAsync(scoreStatistic);
                result.BuildResult("Score statistic has been deleted successfully.");
            }
            catch (Exception ex)
            {
                result.BuildError(ex.Message + " " + ex.StackTrace);
            }
            return result;
        }
    }
}
