using VAcademyConnect.DTOs.Requests;
using VAcademyConnect.DTOs.Responses;
using VAcademyConnect.Domain.Entities;

namespace VAcademyConnect.Application.Mapping
{
    public static class LessonEvaluationMapper
    {
        public static LessonEvaluation ToEntity(LessonEvaluationRequest request)
        {
            return new LessonEvaluation
            {
                ClassId = request.ClassId,
                StudentId = request.StudentId,
                Date = request.Date,
                Comment = request.Comment,
                Rating = request.Rating,
            };
        }

        public static LessonEvaluationResponse ToResponse(LessonEvaluation entity)
        {
            return new LessonEvaluationResponse
            {
                Id = entity.Id,
                ClassId = entity.ClassId,
                ClassName = entity.Class?.Name,
                StudentId = entity.StudentId,
                StudentName = entity.Student?.FullName,
                Date = entity.Date,
                Comment = entity.Comment,
                Rate = entity.Rating,
            };
        }
    }
}
