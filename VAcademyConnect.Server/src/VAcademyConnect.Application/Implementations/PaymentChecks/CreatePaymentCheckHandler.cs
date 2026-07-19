using MayNghien.Infrastructures.Models.Responses;
using Microsoft.EntityFrameworkCore;
using VAcademyConnect.Application.Interfaces;
using VAcademyConnect.Application.Interfaces.PaymentChecks;
using VAcademyConnect.Application.Interfaces.Repositories;
using VAcademyConnect.Application.Mapping;
using VAcademyConnect.DTOs.Requests;
using VAcademyConnect.DTOs.Responses;
using VAcademyConnect.Domain.Entities;

namespace VAcademyConnect.Application.Implementations.PaymentChecks
{
    public class CreatePaymentCheckHandler : ICreatePaymentCheckHandler
    {
        private readonly IPaymentCheckRepository _paymentCheckRepository;
        private readonly IPaymentRepository _paymentRepository;
        private readonly ICurrentUserService _currentUserService;

        public CreatePaymentCheckHandler(IPaymentCheckRepository paymentCheckRepository,
            IPaymentRepository paymentRepository, ICurrentUserService currentUserService)
        {
            _paymentCheckRepository = paymentCheckRepository;
            _paymentRepository = paymentRepository;
            _currentUserService = currentUserService;
        }

        public async Task<AppResponse<PaymentCheckResponse>> HandleAsync(PaymentCheckRequest request)
        {
            var result = new AppResponse<PaymentCheckResponse>();
            try
            {
                var user = await _currentUserService.GetCurrentUserAsync();
                if (user == null)
                    return result.BuildError("Unauthorize.");

                var payment = await _paymentRepository.GetAsync(request.PaymentId!.Value);
                if (payment == null || payment.IsDeleted)
                    return result.BuildError("Payment record was not found or has been deleted.");

                var paymentCheck = new PaymentCheck
                {
                    Id = Guid.NewGuid(),
                    PaymentId = request.PaymentId,
                    CheckedBy = user.Id,
                    CheckTime = request.CheckTime ?? DateTime.UtcNow,
                    Note = request.Note,
                    CreatedBy = user.Email,
                    CreatedOn = DateTime.UtcNow,
                };

                await _paymentCheckRepository.AddAsync(paymentCheck);

                var createdEntity = await _paymentCheckRepository.FindByAsync(x => x.Id == paymentCheck.Id)
                    .Include(x => x.CheckedByUser).FirstOrDefaultAsync();
                var response = PaymentCheckMapper.ToResponse(createdEntity ?? paymentCheck);
                result.BuildResult(response, "Payment has been checked successfully.");
            }
            catch (Exception ex)
            {
                result.BuildError(ex.Message + " " + ex.StackTrace);
            }
            return result;
        }
    }
}
