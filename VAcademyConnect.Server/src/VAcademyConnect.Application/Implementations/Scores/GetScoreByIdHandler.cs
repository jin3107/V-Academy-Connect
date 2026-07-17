using MayNghien.Infrastructures.Models.Responses;
using Microsoft.EntityFrameworkCore;
using VAcademyConnect.Application.Interfaces;
using VAcademyConnect.Application.Interfaces.Repositories;
using VAcademyConnect.Application.Interfaces.Scores;
using VAcademyConnect.Application.Mapping;
using VAcademyConnect.DTOs.Responses;

namespace VAcademyConnect.Application.Implementations.Scores
{
    public class GetScoreByIdHandler : IGetScoreByIdHandler
    {
        private readonly IScoreRepository _scoreRepository;
        private readonly ICurrentUserService _currentUserService;

        public GetScoreByIdHandler(IScoreRepository scoreRepository, ICurrentUserService currentUserService)
        {
            _scoreRepository = scoreRepository;
            _currentUserService = currentUserService;
        }

        public async Task<AppResponse<ScoreResponse>> HandleAsync(Guid id)
        {
            var result = new AppResponse<ScoreResponse>();
            try
            {
                var user = await _currentUserService.GetCurrentUserAsync();
                if (user == null)
                    return result.BuildError("Unauthorize.");

                var score = await _scoreRepository.FindByAsync(x => x.Id == id)
                    .Include(x => x.Class).Include(x => x.Student).FirstOrDefaultAsync();
                if (score == null || score.IsDeleted)
                    return result.BuildError("Score record was not found or has been deleted.");

                var response = ScoreMapper.ToResponse(score);
                result.BuildResult(response, "Score has been found.");
            }
            catch (Exception ex)
            {
                result.BuildError(ex.Message + " " + ex.StackTrace);
            }
            return result;
        }
    }
}
