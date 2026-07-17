using MayNghien.Infrastructures.Models.Responses;
using Microsoft.EntityFrameworkCore;
using VAcademyConnect.Application.Interfaces;
using VAcademyConnect.Application.Interfaces.Attendances;
using VAcademyConnect.Application.Interfaces.Repositories;
using VAcademyConnect.Application.Mapping;
using VAcademyConnect.DTOs.Responses;

namespace VAcademyConnect.Application.Implementations.Attendances
{
    public class GetAttendanceByIdHandler : IGetAttendanceByIdHandler
    {
        private readonly IAttendanceRepository _attendanceRepository;
        private readonly ICurrentUserService _currentUserService;

        public GetAttendanceByIdHandler(IAttendanceRepository attendanceRepository, ICurrentUserService currentUserService)
        {
            _attendanceRepository = attendanceRepository;
            _currentUserService = currentUserService;
        }

        public async Task<AppResponse<AttendanceResponse>> HandleAsync(Guid id)
        {
            var result = new AppResponse<AttendanceResponse>();
            try
            {
                var user = await _currentUserService.GetCurrentUserAsync();
                if (user == null)
                    return result.BuildError("Unauthorize.");

                var attendance = await _attendanceRepository.FindByAsync(a => a.Id == id)
                    .Include(a => a.Class).Include(a => a.Student).FirstOrDefaultAsync();
                if (attendance == null || attendance.IsDeleted)
                    return result.BuildError("Attendance record was not found or has been deleted.");

                var response = AttendanceMapper.ToResponse(attendance);
                result.BuildResult(response, "Attendance has been found.");
            }
            catch (Exception ex)
            {
                result.BuildError(ex.Message + " " + ex.StackTrace);
            }
            return result;
        }
    }
}
