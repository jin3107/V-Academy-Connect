using MayNghien.Infrastructures.Models.Responses;
using Microsoft.EntityFrameworkCore;
using VAcademyConnect.Application.Interfaces;
using VAcademyConnect.Application.Interfaces.Repositories;
using VAcademyConnect.Application.Interfaces.TuitionFees;
using VAcademyConnect.Application.Mapping;
using VAcademyConnect.DTOs.Responses;

namespace VAcademyConnect.Application.Implementations.TuitionFees
{
    public class GetTuitionFeeByIdHandler : IGetTuitionFeeByIdHandler
    {
        private readonly ITuitionFeeRepository _tuitionFeeRepository;
        private readonly ICurrentUserService _currentUserService;

        public GetTuitionFeeByIdHandler(ITuitionFeeRepository tuitionFeeRepository, ICurrentUserService currentUserService)
        {
            _tuitionFeeRepository = tuitionFeeRepository;
            _currentUserService = currentUserService;
        }

        public async Task<AppResponse<TuitionFeeResponse>> HandleAsync(Guid id)
        {
            var result = new AppResponse<TuitionFeeResponse>();
            try
            {
                var user = await _currentUserService.GetCurrentUserAsync();
                if (user == null)
                    return result.BuildError("Unauthorize.");

                var tuitionFee = await _tuitionFeeRepository.FindByAsync(x => x.Id == id)
                    .Include(x => x.Student).FirstOrDefaultAsync();
                if (tuitionFee == null || tuitionFee.IsDeleted)
                    return result.BuildError("Tuition fee record was not found or has been deleted.");

                var response = TuitionFeeMapper.ToResponse(tuitionFee);
                result.BuildResult(response, "Tuition fee has been found.");
            }
            catch (Exception ex)
            {
                result.BuildError(ex.Message + " " + ex.StackTrace);
            }
            return result;
        }
    }
}
