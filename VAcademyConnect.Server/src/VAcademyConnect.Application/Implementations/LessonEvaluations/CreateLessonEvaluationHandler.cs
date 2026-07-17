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
    public class CreateLessonEvaluationHandler : ICreateLessonEvaluationHandler
    {
        private readonly ILessonEvaluationRepository _lessonEvaluationRepository;
        private readonly ICurrentUserService _currentUserService;

        public CreateLessonEvaluationHandler(ILessonEvaluationRepository lessonEvaluationRepository, ICurrentUserService currentUserService)
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

                var lessonEvaluation = LessonEvaluationMapper.ToEntity(request);
                lessonEvaluation.Id = Guid.NewGuid();
                lessonEvaluation.CreatedBy = user.Email;
                lessonEvaluation.CreatedOn = DateTime.UtcNow;

                await _lessonEvaluationRepository.AddAsync(lessonEvaluation);

                var createdEntity = await _lessonEvaluationRepository.FindByAsync(x => x.Id == lessonEvaluation.Id)
                    .Include(x => x.Class).Include(x => x.Student).FirstOrDefaultAsync();
                var response = LessonEvaluationMapper.ToResponse(createdEntity ?? lessonEvaluation);
                result.BuildResult(response, "Lesson evaluation has been recorded successfully.");
            }
            catch (Exception ex)
            {
                result.BuildError(ex.Message + " " + ex.StackTrace);
            }
            return result;
        }
    }
}
