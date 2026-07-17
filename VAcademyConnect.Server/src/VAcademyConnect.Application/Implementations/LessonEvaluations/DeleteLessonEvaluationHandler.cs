using MayNghien.Infrastructures.Models.Responses;
using VAcademyConnect.Application.Interfaces;
using VAcademyConnect.Application.Interfaces.LessonEvaluations;
using VAcademyConnect.Application.Interfaces.Repositories;

namespace VAcademyConnect.Application.Implementations.LessonEvaluations
{
    public class DeleteLessonEvaluationHandler : IDeleteLessonEvaluationHandler
    {
        private readonly ILessonEvaluationRepository _lessonEvaluationRepository;
        private readonly ICurrentUserService _currentUserService;

        public DeleteLessonEvaluationHandler(ILessonEvaluationRepository lessonEvaluationRepository, ICurrentUserService currentUserService)
        {
            _lessonEvaluationRepository = lessonEvaluationRepository;
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

                var lessonEvaluation = await _lessonEvaluationRepository.GetAsync(id);
                if (lessonEvaluation == null || lessonEvaluation.IsDeleted)
                    return result.BuildError("Lesson evaluation record was not found or has been deleted.");

                lessonEvaluation.IsDeleted = true;
                lessonEvaluation.ModifiedBy = user.Email;
                lessonEvaluation.ModifiedOn = DateTime.UtcNow;

                await _lessonEvaluationRepository.EditAsync(lessonEvaluation);
                result.BuildResult("Lesson evaluation has been deleted successfully.");
            }
            catch (Exception ex)
            {
                result.BuildError(ex.Message + " " + ex.StackTrace);
            }
            return result;
        }
    }
}
