using VAcademyConnect.DTOs.Requests;
using VAcademyConnect.DTOs.Responses;
using VAcademyConnect.Domain.Entities;

namespace VAcademyConnect.Application.Mapping
{
    public static class PaymentCheckMapper
    {
        public static PaymentCheck ToEntity(PaymentCheckRequest request)
        {
            return new PaymentCheck
            {
                PaymentId = request.PaymentId,
                CheckedBy = request.CheckedBy,
                CheckTime = request.CheckTime,
                Note = request.Note,
            };
        }

        public static PaymentCheckResponse ToResponse(PaymentCheck entity)
        {
            return new PaymentCheckResponse
            {
                Id = entity.Id,
                PaymentId = entity.PaymentId,
                CheckedBy = entity.CheckedBy,
                CheckedByName = entity.CheckedByUser?.FullName,
                CheckTime = entity.CheckTime,
                Note = entity.Note,
            };
        }
    }
}
