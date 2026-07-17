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
    public class CreateScoreHandler : ICreateScoreHandler
    {
        private readonly IScoreRepository _scoreRepository;
        private readonly ICurrentUserService _currentUserService;

        public CreateScoreHandler(IScoreRepository scoreRepository, ICurrentUserService currentUserService)
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

                var score = ScoreMapper.ToEntity(request);
                score.Id = Guid.NewGuid();
                score.CreatedBy = user.Email;
                score.CreatedOn = DateTime.UtcNow;

                await _scoreRepository.AddAsync(score);

                var createdEntity = await _scoreRepository.FindByAsync(x => x.Id == score.Id)
                    .Include(x => x.Class).Include(x => x.Student).FirstOrDefaultAsync();
                var response = ScoreMapper.ToResponse(createdEntity ?? score);
                result.BuildResult(response, "Score has been recorded successfully.");
            }
            catch (Exception ex)
            {
                result.BuildError(ex.Message + " " + ex.StackTrace);
            }
            return result;
        }
    }
}
