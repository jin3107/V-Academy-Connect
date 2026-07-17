using MayNghien.Infrastructures.Models.Responses;
using VAcademyConnect.Application.Interfaces;
using VAcademyConnect.Application.Interfaces.Repositories;
using VAcademyConnect.Application.Interfaces.Scores;

namespace VAcademyConnect.Application.Implementations.Scores
{
    public class DeleteScoreHandler : IDeleteScoreHandler
    {
        private readonly IScoreRepository _scoreRepository;
        private readonly ICurrentUserService _currentUserService;

        public DeleteScoreHandler(IScoreRepository scoreRepository, ICurrentUserService currentUserService)
        {
            _scoreRepository = scoreRepository;
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

                var score = await _scoreRepository.GetAsync(id);
                if (score == null || score.IsDeleted)
                    return result.BuildError("Score record was not found or has been deleted.");

                score.IsDeleted = true;
                score.ModifiedBy = user.Email;
                score.ModifiedOn = DateTime.UtcNow;

                await _scoreRepository.EditAsync(score);
                result.BuildResult("Score has been deleted successfully.");
            }
            catch (Exception ex)
            {
                result.BuildError(ex.Message + " " + ex.StackTrace);
            }
            return result;
        }
    }
}
