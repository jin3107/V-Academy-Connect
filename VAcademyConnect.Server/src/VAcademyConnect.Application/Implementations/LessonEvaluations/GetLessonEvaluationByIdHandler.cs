using MayNghien.Infrastructures.Models.Responses;
using Microsoft.EntityFrameworkCore;
using VAcademyConnect.Application.Interfaces;
using VAcademyConnect.Application.Interfaces.LessonEvaluations;
using VAcademyConnect.Application.Interfaces.Repositories;
using VAcademyConnect.Application.Mapping;
using VAcademyConnect.DTOs.Responses;

namespace VAcademyConnect.Application.Implementations.LessonEvaluations
{
    public class GetLessonEvaluationByIdHandler : IGetLessonEvaluationByIdHandler
    {
        private readonly ILessonEvaluationRepository _lessonEvaluationRepository;
        private readonly ICurrentUserService _currentUserService;

        public GetLessonEvaluationByIdHandler(ILessonEvaluationRepository lessonEvaluationRepository, ICurrentUserService currentUserService)
        {
            _lessonEvaluationRepository = lessonEvaluationRepository;
            _currentUserService = currentUserService;
        }

        public async Task<AppResponse<LessonEvaluationResponse>> HandleAsync(Guid id)
        {
            var result = new AppResponse<LessonEvaluationResponse>();
            try
            {
                var user = await _currentUserService.GetCurrentUserAsync();
                if (user == null)
                    return result.BuildError("Unauthorize.");

                var lessonEvaluation = await _lessonEvaluationRepository.FindByAsync(x => x.Id == id)
                    .Include(x => x.Class).Include(x => x.Student).FirstOrDefaultAsync();
                if (lessonEvaluation == null || lessonEvaluation.IsDeleted)
                    return result.BuildError("Lesson evaluation record was not found or has been deleted.");

                var response = LessonEvaluationMapper.ToResponse(lessonEvaluation);
                result.BuildResult(response, "Lesson evaluation has been found.");
            }
            catch (Exception ex)
            {
                result.BuildError(ex.Message + " " + ex.StackTrace);
            }
            return result;
        }
    }
}
