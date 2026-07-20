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
    public class UpdateTuitionFeeHandler : IUpdateTuitionFeeHandler
    {
        private readonly ITuitionFeeRepository _tuitionFeeRepository;
        private readonly ICurrentUserService _currentUserService;

        public UpdateTuitionFeeHandler(ITuitionFeeRepository tuitionFeeRepository, ICurrentUserService currentUserService)
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

                var tuitionFee = await _tuitionFeeRepository.GetAsync(request.Id);
                if (tuitionFee == null || tuitionFee.IsDeleted)
                    return result.BuildError("Tuition fee record was not found or has been deleted.");

                tuitionFee.StudentId = request.StudentId;
                tuitionFee.Month = request.Month;
                tuitionFee.Amount = request.Amount;
                tuitionFee.DueDate = request.DueDate;
                tuitionFee.ModifiedBy = user.Email;
                tuitionFee.ModifiedOn = DateTime.UtcNow;

                await _tuitionFeeRepository.EditAsync(tuitionFee);

                var updatedEntity = await _tuitionFeeRepository.FindByAsync(x => x.Id == tuitionFee.Id)
                    .Include(x => x.Student).FirstOrDefaultAsync();
                var response = TuitionFeeMapper.ToResponse(updatedEntity ?? tuitionFee);
                result.BuildResult(response, "Tuition fee has been updated successfully.");
            }
            catch (Exception ex)
            {
                result.BuildError(ex.Message + " " + ex.StackTrace);
            }
            return result;
        }
    }
}
