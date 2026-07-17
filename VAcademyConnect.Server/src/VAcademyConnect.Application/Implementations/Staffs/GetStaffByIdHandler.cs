using MayNghien.Infrastructures.Models.Responses;
using VAcademyConnect.Application.Interfaces;
using VAcademyConnect.Application.Interfaces.Repositories;
using VAcademyConnect.Application.Interfaces.Staffs;
using VAcademyConnect.Application.Mapping;
using VAcademyConnect.DTOs.Responses;

namespace VAcademyConnect.Application.Implementations.Staffs
{
    public class GetStaffByIdHandler : IGetStaffByIdHandler
    {
        private readonly IStaffRepository _staffRepository;
        private readonly ICurrentUserService _currentUserService;

        public GetStaffByIdHandler(IStaffRepository staffRepository, ICurrentUserService currentUserService)
        {
            _staffRepository = staffRepository;
            _currentUserService = currentUserService;
        }

        public async Task<AppResponse<StaffResponse>> HandleAsync(Guid id)
        {
            var result = new AppResponse<StaffResponse>();
            try
            {
                var currentUser = await _currentUserService.GetCurrentUserAsync();
                if (currentUser == null)
                    return result.BuildError("Unauthorized");

                var staff = await _staffRepository.GetAsync(id);
                if (staff == null || staff.IsDeleted)
                    return result.BuildError("Staff not found");

                var response = StaffMapper.ToResponse(staff);
                return result.BuildResult(response);
            }
            catch (Exception ex)
            {
                return result.BuildError(ex.Message + " " + ex.StackTrace);
            }
        }
    }
}
