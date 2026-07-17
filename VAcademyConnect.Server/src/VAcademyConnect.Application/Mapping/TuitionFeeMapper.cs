using VAcademyConnect.DTOs.Requests;
using VAcademyConnect.DTOs.Responses;
using VAcademyConnect.Domain.Entities;

namespace VAcademyConnect.Application.Mapping
{
    public static class TuitionFeeMapper
    {
        public static TuitionFee ToEntity(TuitionFeeRequest request)
        {
            return new TuitionFee
            {
                StudentId = request.StudentId,
                Month = request.Month,
                Amount = request.Amount,
                DueDate = request.DueDate,
            };
        }

        public static TuitionFeeResponse ToResponse(TuitionFee entity)
        {
            return new TuitionFeeResponse
            {
                Id = entity.Id,
                StudentId = entity.StudentId,
                StudentName = entity.Student?.FullName,
                Email = entity.Student?.Email,
                Month = entity.Month,
                Amount = entity.Amount,
                DueDate = entity.DueDate,
            };
        }
    }
}
