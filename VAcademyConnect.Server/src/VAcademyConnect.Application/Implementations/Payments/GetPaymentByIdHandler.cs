using MayNghien.Infrastructures.Models.Responses;
using Microsoft.EntityFrameworkCore;
using VAcademyConnect.Application.Interfaces;
using VAcademyConnect.Application.Interfaces.Payments;
using VAcademyConnect.Application.Interfaces.Repositories;
using VAcademyConnect.Application.Mapping;
using VAcademyConnect.DTOs.Responses;

namespace VAcademyConnect.Application.Implementations.Payments
{
    public class GetPaymentByIdHandler : IGetPaymentByIdHandler
    {
        private readonly IPaymentRepository _paymentRepository;
        private readonly ICurrentUserService _currentUserService;

        public GetPaymentByIdHandler(IPaymentRepository paymentRepository, ICurrentUserService currentUserService)
        {
            _paymentRepository = paymentRepository;
            _currentUserService = currentUserService;
        }

        public async Task<AppResponse<PaymentResponse>> HandleAsync(Guid id)
        {
            var result = new AppResponse<PaymentResponse>();
            try
            {
                var user = await _currentUserService.GetCurrentUserAsync();
                if (user == null)
                    return result.BuildError("Unauthorize.");

                var payment = await _paymentRepository.FindByAsync(x => x.Id == id)
                    .Include(x => x.Student).FirstOrDefaultAsync();
                if (payment == null || payment.IsDeleted)
                    return result.BuildError("Payment record was not found or has been deleted.");

                var response = PaymentMapper.ToResponse(payment);
                result.BuildResult(response, "Payment has been found.");
            }
            catch (Exception ex)
            {
                result.BuildError(ex.Message + " " + ex.StackTrace);
            }
            return result;
        }
    }
}
