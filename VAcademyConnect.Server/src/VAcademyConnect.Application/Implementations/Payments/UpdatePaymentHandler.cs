using MayNghien.Infrastructures.Models.Responses;
using Microsoft.EntityFrameworkCore;
using VAcademyConnect.Application.Interfaces;
using VAcademyConnect.Application.Interfaces.Payments;
using VAcademyConnect.Application.Interfaces.Repositories;
using VAcademyConnect.Application.Mapping;
using VAcademyConnect.DTOs.Requests;
using VAcademyConnect.DTOs.Responses;

namespace VAcademyConnect.Application.Implementations.Payments
{
    public class UpdatePaymentHandler : IUpdatePaymentHandler
    {
        private readonly IPaymentRepository _paymentRepository;
        private readonly ICurrentUserService _currentUserService;

        public UpdatePaymentHandler(IPaymentRepository paymentRepository, ICurrentUserService currentUserService)
        {
            _paymentRepository = paymentRepository;
            _currentUserService = currentUserService;
        }

        public async Task<AppResponse<PaymentResponse>> HandleAsync(PaymentRequest request)
        {
            var result = new AppResponse<PaymentResponse>();
            try
            {
                var user = await _currentUserService.GetCurrentUserAsync();
                if (user == null)
                    return result.BuildError("Unauthorize.");

                var payment = await _paymentRepository.GetAsync(request.Id);
                if (payment == null || payment.IsDeleted)
                    return result.BuildError("Payment record was not found or has been deleted.");

                payment.StudentId = request.StudentId;
                payment.Amount = request.Amount;
                payment.Note = request.Note;
                payment.TransferTime = request.TransferTime;
                payment.QRInfo = request.QRInfo;
                payment.ModifiedBy = user.Email;
                payment.ModifiedOn = DateTime.UtcNow;

                await _paymentRepository.EditAsync(payment);

                var updatedEntity = await _paymentRepository.FindByAsync(x => x.Id == payment.Id)
                    .Include(x => x.Student).FirstOrDefaultAsync();
                var response = PaymentMapper.ToResponse(updatedEntity ?? payment);
                result.BuildResult(response, "Payment has been updated successfully.");
            }
            catch (Exception ex)
            {
                result.BuildError(ex.Message + " " + ex.StackTrace);
            }
            return result;
        }
    }
}
