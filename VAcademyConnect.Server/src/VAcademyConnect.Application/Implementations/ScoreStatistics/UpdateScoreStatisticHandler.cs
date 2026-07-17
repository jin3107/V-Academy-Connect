using MayNghien.Infrastructures.Models.Responses;
using Microsoft.EntityFrameworkCore;
using VAcademyConnect.Application.Interfaces;
using VAcademyConnect.Application.Interfaces.Repositories;
using VAcademyConnect.Application.Interfaces.ScoreStatistics;
using VAcademyConnect.Application.Mapping;
using VAcademyConnect.DTOs.Requests;
using VAcademyConnect.DTOs.Responses;

namespace VAcademyConnect.Application.Implementations.ScoreStatistics
{
    public class UpdateScoreStatisticHandler : IUpdateScoreStatisticHandler
    {
        private readonly IScoreStatisticRepository _scoreStatisticRepository;
        private readonly ICurrentUserService _currentUserService;

        public UpdateScoreStatisticHandler(IScoreStatisticRepository scoreStatisticRepository, ICurrentUserService currentUserService)
        {
            _scoreStatisticRepository = scoreStatisticRepository;
            _currentUserService = currentUserService;
        }

        public async Task<AppResponse<ScoreStatisticResponse>> HandleAsync(ScoreStatisticRequest request)
        {
            var result = new AppResponse<ScoreStatisticResponse>();
            try
            {
                var user = await _currentUserService.GetCurrentUserAsync();
                if (user == null)
                    return result.BuildError("Unauthorize.");

                var scoreStatistic = await _scoreStatisticRepository.GetAsync(request.Id);
                if (scoreStatistic == null || scoreStatistic.IsDeleted)
                    return result.BuildError("Score statistic record was not found or has been deleted.");

                scoreStatistic.StudentId = request.StudentId;
                scoreStatistic.Month = request.Month;
                scoreStatistic.AvarageScore = request.AvarageScore;
                scoreStatistic.ModifiedBy = user.Email;
                scoreStatistic.ModifiedOn = DateTime.UtcNow;

                await _scoreStatisticRepository.EditAsync(scoreStatistic);

                var updatedEntity = await _scoreStatisticRepository.FindByAsync(x => x.Id == scoreStatistic.Id)
                    .Include(x => x.Student).FirstOrDefaultAsync();
                var response = ScoreStatisticMapper.ToResponse(updatedEntity ?? scoreStatistic);
                result.BuildResult(response, "Score statistic has been updated successfully.");
            }
            catch (Exception ex)
            {
                result.BuildError(ex.Message + " " + ex.StackTrace);
            }
            return result;
        }
    }
}
