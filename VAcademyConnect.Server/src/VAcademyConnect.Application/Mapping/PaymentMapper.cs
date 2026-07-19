using VAcademyConnect.DTOs.Requests;
using VAcademyConnect.DTOs.Responses;
using VAcademyConnect.Domain.Entities;

namespace VAcademyConnect.Application.Mapping
{
    public static class PaymentMapper
    {
        public static Payment ToEntity(PaymentRequest request)
        {
            return new Payment
            {
                StudentId = request.StudentId,
                Amount = request.Amount,
                Note = request.Note,
                TransferTime = request.TransferTime,
                QRInfo = request.QRInfo,
            };
        }

        public static PaymentResponse ToResponse(Payment entity)
        {
            return new PaymentResponse
            {
                Id = entity.Id,
                StudentId = entity.StudentId,
                StudentName = entity.Student?.FullName,
                Amount = entity.Amount,
                Note = entity.Note,
                TransferTime = entity.TransferTime,
                QRInfo = entity.QRInfo,
            };
        }
    }
}
