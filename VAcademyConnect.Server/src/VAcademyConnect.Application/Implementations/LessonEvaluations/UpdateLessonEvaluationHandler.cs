using MayNghien.Infrastructures.Models.Responses;
using Microsoft.EntityFrameworkCore;
using VAcademyConnect.Application.Interfaces;
using VAcademyConnect.Application.Interfaces.LessonEvaluations;
using VAcademyConnect.Application.Interfaces.Repositories;
using VAcademyConnect.Application.Mapping;
using VAcademyConnect.DTOs.Requests;
using VAcademyConnect.DTOs.Responses;

namespace VAcademyConnect.Application.Implementations.LessonEvaluations
{
    public class UpdateLessonEvaluationHandler : IUpdateLessonEvaluationHandler
    {
        private readonly ILessonEvaluationRepository _lessonEvaluationRepository;
        private readonly ICurrentUserService _currentUserService;

        public UpdateLessonEvaluationHandler(ILessonEvaluationRepository lessonEvaluationRepository, ICurrentUserService currentUserService)
        {
            _lessonEvaluationRepository = lessonEvaluationRepository;
            _currentUserService = currentUserService;
        }

        public async Task<AppResponse<LessonEvaluationResponse>> HandleAsync(LessonEvaluationRequest request)
        {
            var result = new AppResponse<LessonEvaluationResponse>();
            try
            {
                var user = await _currentUserService.GetCurrentUserAsync();
                if (user == null)
                    return result.BuildError("Unauthorize.");

                var lessonEvaluation = await _lessonEvaluationRepository.GetAsync(request.Id);
                if (lessonEvaluation == null || lessonEvaluation.IsDeleted)
                    return result.BuildError("Lesson evaluation record was not found or has been deleted.");

                lessonEvaluation.ClassId = request.ClassId;
                lessonEvaluation.StudentId = request.StudentId;
                lessonEvaluation.Date = request.Date;
                lessonEvaluation.Comment = request.Comment;
                lessonEvaluation.Rating = request.Rating;
                lessonEvaluation.ModifiedBy = user.Email;
                lessonEvaluation.ModifiedOn = DateTime.UtcNow;

                await _lessonEvaluationRepository.EditAsync(lessonEvaluation);

                var updatedEntity = await _lessonEvaluationRepository.FindByAsync(x => x.Id == lessonEvaluation.Id)
                    .Include(x => x.Class).Include(x => x.Student).FirstOrDefaultAsync();
                var response = LessonEvaluationMapper.ToResponse(updatedEntity ?? lessonEvaluation);
                result.BuildResult(response, "Lesson evaluation has been updated successfully.");
            }
            catch (Exception ex)
            {
                result.BuildError(ex.Message + " " + ex.StackTrace);
            }
            return result;
        }
    }
}
