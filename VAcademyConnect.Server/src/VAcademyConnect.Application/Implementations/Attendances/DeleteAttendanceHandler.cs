using MayNghien.Infrastructures.Models.Responses;
using VAcademyConnect.Application.Interfaces;
using VAcademyConnect.Application.Interfaces.Attendances;
using VAcademyConnect.Application.Interfaces.Repositories;

namespace VAcademyConnect.Application.Implementations.Attendances
{
    public class DeleteAttendanceHandler : IDeleteAttendanceHandler
    {
        private readonly IAttendanceRepository _attendanceRepository;
        private readonly ICurrentUserService _currentUserService;

        public DeleteAttendanceHandler(IAttendanceRepository attendanceRepository, ICurrentUserService currentUserService)
        {
            _attendanceRepository = attendanceRepository;
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

                var attendance = await _attendanceRepository.GetAsync(id);
                if (attendance == null || attendance.IsDeleted)
                    return result.BuildError("Attendance record was not found or has been deleted.");

                attendance.IsDeleted = true;
                attendance.ModifiedBy = user.Email;
                attendance.ModifiedOn = DateTime.UtcNow;

                await _attendanceRepository.EditAsync(attendance);
                result.BuildResult("Attendance has been deleted successfully.");
            }
            catch (Exception ex)
            {
                result.BuildError(ex.Message + " " + ex.StackTrace);
            }
            return result;
        }
    }
}
