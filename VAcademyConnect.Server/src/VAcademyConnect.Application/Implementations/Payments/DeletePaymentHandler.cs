using MayNghien.Infrastructures.Models.Responses;
using VAcademyConnect.Application.Interfaces;
using VAcademyConnect.Application.Interfaces.Payments;
using VAcademyConnect.Application.Interfaces.Repositories;

namespace VAcademyConnect.Application.Implementations.Payments
{
    public class DeletePaymentHandler : IDeletePaymentHandler
    {
        private readonly IPaymentRepository _paymentRepository;
        private readonly ICurrentUserService _currentUserService;

        public DeletePaymentHandler(IPaymentRepository paymentRepository, ICurrentUserService currentUserService)
        {
            _paymentRepository = paymentRepository;
            _currentUserService = currentUserService;
        }

        public async Task<AppResponse<string>> HandleAsync(Guid id)
        {
            var result = new AppResponse<string>();
            try
            {
                var user = await _currentUserService.GetCurrentUserAsync();
                if (user == null)
                    return result.BuildError("Unauthorize");

                var payment = await _paymentRepository.GetAsync(id);
                if (payment == null || payment.IsDeleted)
                    return result.BuildError("Payment record was not found or has been deleted.");

                payment.IsDeleted = true;
                payment.ModifiedBy = user.Email;
                payment.ModifiedOn = DateTime.UtcNow;

                await _paymentRepository.EditAsync(payment);
                result.BuildResult("Payment has been deleted successfully.");
            }
            catch (Exception ex)
            {
                result.BuildError(ex.Message + " " + ex.StackTrace);
            }
            return result;
        }
    }
}
