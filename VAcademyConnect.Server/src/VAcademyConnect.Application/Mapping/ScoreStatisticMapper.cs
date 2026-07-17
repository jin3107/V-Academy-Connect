using VAcademyConnect.DTOs.Requests;
using VAcademyConnect.DTOs.Responses;
using VAcademyConnect.Domain.Entities;

namespace VAcademyConnect.Application.Mapping
{
    public static class ScoreStatisticMapper
    {
        public static ScoreStatistic ToEntity(ScoreStatisticRequest request)
        {
            return new ScoreStatistic
            {
                StudentId = request.StudentId,
                Month = request.Month,
                AvarageScore = request.AvarageScore,
            };
        }

        public static ScoreStatisticResponse ToResponse(ScoreStatistic entity)
        {
            return new ScoreStatisticResponse
            {
                Id = entity.Id,
                StudentId = entity.StudentId,
                StudentName = entity.Student?.FullName,
                Month = entity.Month,
                AvarageScore = entity.AvarageScore,
            };
        }
    }
}
