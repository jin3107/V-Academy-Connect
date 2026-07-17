using VAcademyConnect.DTOs.Requests;
using VAcademyConnect.DTOs.Responses;
using VAcademyConnect.Domain.Entities;

namespace VAcademyConnect.Application.Mapping
{
    public static class ScoreMapper
    {
        public static Score ToEntity(ScoreRequest request)
        {
            return new Score
            {
                StudentId = request.StudentId,
                ClassId = request.ClassId,
                TestName = request.TestName,
                Mark = request.Mark,
            };
        }

        public static ScoreResponse ToResponse(Score entity)
        {
            return new ScoreResponse
            {
                Id = entity.Id,
                StudentId = entity.StudentId,
                StudentName = entity.Student?.FullName,
                ClassId = entity.ClassId,
                ClassName = entity.Class?.Name,
                TestName = entity.TestName,
                Mark = entity.Mark,
            };
        }
    }
}
