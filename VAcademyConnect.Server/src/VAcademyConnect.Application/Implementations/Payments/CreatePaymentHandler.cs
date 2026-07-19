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
    public class CreatePaymentHandler : ICreatePaymentHandler
    {
        private readonly IPaymentRepository _paymentRepository;
        private readonly ICurrentUserService _currentUserService;

        public CreatePaymentHandler(IPaymentRepository paymentRepository, ICurrentUserService currentUserService)
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

                var payment = PaymentMapper.ToEntity(request);
                payment.Id = Guid.NewGuid();
                payment.CreatedBy = user.Email;
                payment.CreatedOn = DateTime.UtcNow;

                await _paymentRepository.AddAsync(payment);

                var createdEntity = await _paymentRepository.FindByAsync(x => x.Id == payment.Id)
                    .Include(x => x.Student).FirstOrDefaultAsync();
                var response = PaymentMapper.ToResponse(createdEntity ?? payment);
                result.BuildResult(response, "Payment has been recorded successfully.");
            }
            catch (Exception ex)
            {
                result.BuildError(ex.Message + " " + ex.StackTrace);
            }
            return result;
        }
    }
}
