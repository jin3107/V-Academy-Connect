using MayNghien.Infrastructures.Models.Responses;
using VAcademyConnect.Application.Interfaces;
using VAcademyConnect.Application.Interfaces.Repositories;
using VAcademyConnect.Application.Interfaces.Staffs;

namespace VAcademyConnect.Application.Implementations.Staffs
{
    public class DeleteStaffHandler : IDeleteStaffHandler
    {
        private readonly IStaffRepository _staffRepository;
        private readonly ICurrentUserService _currentUserService;

        public DeleteStaffHandler(IStaffRepository staffRepository, ICurrentUserService currentUserService)
        {
            _staffRepository = staffRepository;
            _currentUserService = currentUserService;
        }

        public async Task<AppResponse<string>> HandleAsync(Guid id)
        {
            var result = new AppResponse<string>();
            try
            {
                var currentUser = await _currentUserService.GetCurrentUserAsync();
                if (currentUser == null)
                    return result.BuildError("Unauthorized");

                var staff = await _staffRepository.GetAsync(id);
                if (staff == null)
                    return result.BuildError("Staff not found");

                staff.IsDeleted = true;
                staff.ModifiedBy = currentUser.Email;
                staff.ModifiedOn = DateTime.UtcNow;

                await _staffRepository.EditAsync(staff);
                return result.BuildResult("Successfully deleted", "Staff deleted successfully");
            }
            catch (Exception ex)
            {
                return result.BuildError(ex.Message + " " + ex.StackTrace);
            }
        }
    }
}
