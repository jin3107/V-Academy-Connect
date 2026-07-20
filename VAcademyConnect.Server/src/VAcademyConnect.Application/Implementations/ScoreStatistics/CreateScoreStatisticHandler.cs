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
    public class CreateScoreStatisticHandler : ICreateScoreStatisticHandler
    {
        private readonly IScoreStatisticRepository _scoreStatisticRepository;
        private readonly ICurrentUserService _currentUserService;

        public CreateScoreStatisticHandler(IScoreStatisticRepository scoreStatisticRepository, ICurrentUserService currentUserService)
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

                var scoreStatistic = ScoreStatisticMapper.ToEntity(request);
                scoreStatistic.Id = Guid.NewGuid();
                scoreStatistic.CreatedBy = user.Email;
                scoreStatistic.CreatedOn = DateTime.UtcNow;

                await _scoreStatisticRepository.AddAsync(scoreStatistic);

                var createdEntity = await _scoreStatisticRepository.FindByAsync(x => x.Id == scoreStatistic.Id)
                    .Include(x => x.Student).FirstOrDefaultAsync();
                var response = ScoreStatisticMapper.ToResponse(createdEntity ?? scoreStatistic);
                result.BuildResult(response, "Score statistic has been recorded successfully.");
            }
            catch (Exception ex)
            {
                result.BuildError(ex.Message + " " + ex.StackTrace);
            }
            return result;
        }
    }
}
