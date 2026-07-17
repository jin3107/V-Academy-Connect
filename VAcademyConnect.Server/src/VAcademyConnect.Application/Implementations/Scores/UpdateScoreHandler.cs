using MayNghien.Infrastructures.Models.Responses;
using Microsoft.EntityFrameworkCore;
using VAcademyConnect.Application.Interfaces;
using VAcademyConnect.Application.Interfaces.Repositories;
using VAcademyConnect.Application.Interfaces.Scores;
using VAcademyConnect.Application.Mapping;
using VAcademyConnect.DTOs.Requests;
using VAcademyConnect.DTOs.Responses;

namespace VAcademyConnect.Application.Implementations.Scores
{
    public class UpdateScoreHandler : IUpdateScoreHandler
    {
        private readonly IScoreRepository _scoreRepository;
        private readonly ICurrentUserService _currentUserService;

        public UpdateScoreHandler(IScoreRepository scoreRepository, ICurrentUserService currentUserService)
        {
            _scoreRepository = scoreRepository;
            _currentUserService = currentUserService;
        }

        public async Task<AppResponse<ScoreResponse>> HandleAsync(ScoreRequest request)
        {
            var result = new AppResponse<ScoreResponse>();
            try
            {
                var user = await _currentUserService.GetCurrentUserAsync();
                if (user == null)
                    return result.BuildError("Unauthorize.");

                var score = await _scoreRepository.GetAsync(request.Id);
                if (score == null || score.IsDeleted)
                    return result.BuildError("Score record was not found or has been deleted.");

                score.StudentId = request.StudentId;
                score.ClassId = request.ClassId;
                score.TestName = request.TestName;
                score.Mark = request.Mark;
                score.ModifiedBy = user.Email;
                score.ModifiedOn = DateTime.UtcNow;

                await _scoreRepository.EditAsync(score);

                var updatedEntity = await _scoreRepository.FindByAsync(x => x.Id == score.Id)
                    .Include(x => x.Class).Include(x => x.Student).FirstOrDefaultAsync();
                var response = ScoreMapper.ToResponse(updatedEntity ?? score);
                result.BuildResult(response, "Score has been updated successfully.");
            }
            catch (Exception ex)
            {
                result.BuildError(ex.Message + " " + ex.StackTrace);
            }
            return result;
        }
    }
}
