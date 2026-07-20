using MayNghien.Infrastructures.Models.Responses;
using VAcademyConnect.Application.Interfaces;
using VAcademyConnect.Application.Interfaces.Repositories;
using VAcademyConnect.Application.Interfaces.TuitionFees;

namespace VAcademyConnect.Application.Implementations.TuitionFees
{
    public class DeleteTuitionFeeHandler : IDeleteTuitionFeeHandler
    {
        private readonly ITuitionFeeRepository _tuitionFeeRepository;
        private readonly ICurrentUserService _currentUserService;

        public DeleteTuitionFeeHandler(ITuitionFeeRepository tuitionFeeRepository, ICurrentUserService currentUserService)
        {
            _tuitionFeeRepository = tuitionFeeRepository;
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

                var tuitionFee = await _tuitionFeeRepository.GetAsync(id);
                if (tuitionFee == null || tuitionFee.IsDeleted)
                    return result.BuildError("Tuition fee record was not found or has been deleted.");

                tuitionFee.IsDeleted = true;
                tuitionFee.ModifiedBy = user.Email;
                tuitionFee.ModifiedOn = DateTime.UtcNow;

                await _tuitionFeeRepository.EditAsync(tuitionFee);
                result.BuildResult("Tuition fee has been deleted successfully.");
            }
            catch (Exception ex)
            {
                result.BuildError(ex.Message + " " + ex.StackTrace);
            }
            return result;
        }
    }
}
