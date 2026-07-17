using MayNghien.Infrastructures.Models.Responses;
using Microsoft.EntityFrameworkCore;
using VAcademyConnect.Application.Interfaces;
using VAcademyConnect.Application.Interfaces.Repositories;
using VAcademyConnect.Application.Interfaces.TuitionFees;
using VAcademyConnect.Application.Mapping;
using VAcademyConnect.DTOs.Requests;
using VAcademyConnect.DTOs.Responses;

namespace VAcademyConnect.Application.Implementations.TuitionFees
{
    public class CreateTuitionFeeHandler : ICreateTuitionFeeHandler
    {
        private readonly ITuitionFeeRepository _tuitionFeeRepository;
        private readonly ICurrentUserService _currentUserService;

        public CreateTuitionFeeHandler(ITuitionFeeRepository tuitionFeeRepository, ICurrentUserService currentUserService)
        {
            _tuitionFeeRepository = tuitionFeeRepository;
            _currentUserService = currentUserService;
        }

        public async Task<AppResponse<TuitionFeeResponse>> HandleAsync(TuitionFeeRequest request)
        {
            var result = new AppResponse<TuitionFeeResponse>();
            try
            {
                var user = await _currentUserService.GetCurrentUserAsync();
                if (user == null)
                    return result.BuildError("Unauthorize.");

                var tuitionFee = TuitionFeeMapper.ToEntity(request);
                tuitionFee.Id = Guid.NewGuid();
                tuitionFee.CreatedBy = user.Email;
                tuitionFee.CreatedOn = DateTime.UtcNow;

                await _tuitionFeeRepository.AddAsync(tuitionFee);

                var createdEntity = await _tuitionFeeRepository.FindByAsync(x => x.Id == tuitionFee.Id)
                    .Include(x => x.Student).FirstOrDefaultAsync();
                var response = TuitionFeeMapper.ToResponse(createdEntity ?? tuitionFee);
                result.BuildResult(response, "Tuition fee has been recorded successfully.");
            }
            catch (Exception ex)
            {
                result.BuildError(ex.Message + " " + ex.StackTrace);
            }
            return result;
        }
    }
}
