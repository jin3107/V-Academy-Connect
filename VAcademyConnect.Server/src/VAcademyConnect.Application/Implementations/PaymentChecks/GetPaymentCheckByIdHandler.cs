using MayNghien.Infrastructures.Models.Responses;
using Microsoft.EntityFrameworkCore;
using VAcademyConnect.Application.Interfaces;
using VAcademyConnect.Application.Interfaces.PaymentChecks;
using VAcademyConnect.Application.Interfaces.Repositories;
using VAcademyConnect.Application.Mapping;
using VAcademyConnect.DTOs.Responses;

namespace VAcademyConnect.Application.Implementations.PaymentChecks
{
    public class GetPaymentCheckByIdHandler : IGetPaymentCheckByIdHandler
    {
        private readonly IPaymentCheckRepository _paymentCheckRepository;
        private readonly ICurrentUserService _currentUserService;

        public GetPaymentCheckByIdHandler(IPaymentCheckRepository paymentCheckRepository, ICurrentUserService currentUserService)
        {
            _paymentCheckRepository = paymentCheckRepository;
            _currentUserService = currentUserService;
        }

        public async Task<AppResponse<PaymentCheckResponse>> HandleAsync(Guid id)
        {
            var result = new AppResponse<PaymentCheckResponse>();
            try
            {
                var user = await _currentUserService.GetCurrentUserAsync();
                if (user == null)
                    return result.BuildError("Unauthorize.");

                var paymentCheck = await _paymentCheckRepository.FindByAsync(x => x.Id == id)
                    .Include(x => x.CheckedByUser).FirstOrDefaultAsync();
                if (paymentCheck == null || paymentCheck.IsDeleted)
                    return result.BuildError("Payment check record was not found or has been deleted.");

                var response = PaymentCheckMapper.ToResponse(paymentCheck);
                result.BuildResult(response, "Payment check has been found.");
            }
            catch (Exception ex)
            {
                result.BuildError(ex.Message + " " + ex.StackTrace);
            }
            return result;
        }
    }
}
