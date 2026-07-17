using MayNghien.Infrastructures.Models.Responses;
using Microsoft.EntityFrameworkCore;
using VAcademyConnect.Application.Interfaces;
using VAcademyConnect.Application.Interfaces.Attendances;
using VAcademyConnect.Application.Interfaces.Repositories;
using VAcademyConnect.Application.Mapping;
using VAcademyConnect.DTOs.Requests;
using VAcademyConnect.DTOs.Responses;

namespace VAcademyConnect.Application.Implementations.Attendances
{
    public class UpdateAttendanceHandler : IUpdateAttendanceHandler
    {
        private readonly IAttendanceRepository _attendanceRepository;
        private readonly ICurrentUserService _currentUserService;

        public UpdateAttendanceHandler(IAttendanceRepository attendanceRepository, ICurrentUserService currentUserService)
        {
            _attendanceRepository = attendanceRepository;
            _currentUserService = currentUserService;
        }

        public async Task<AppResponse<AttendanceResponse>> HandleAsync(AttendanceRequest request)
        {
            var result = new AppResponse<AttendanceResponse>();
            try
            {
                var user = await _currentUserService.GetCurrentUserAsync();
                if (user == null)
                    return result.BuildError("Unauthorize.");

                var attendance = await _attendanceRepository.GetAsync(request.Id);
                if (attendance == null || attendance.IsDeleted)
                    return result.BuildError("Attendance record was not found or has been deleted.");

                attendance.ClassId = request.ClassId;
                attendance.StudentId = request.StudentId;
                attendance.Date = request.Date;
                attendance.IsPresent = request.IsPresent;
                attendance.CountAsPaid = request.CountAsPaid;
                attendance.ModifiedBy = user.Email;
                attendance.ModifiedOn = DateTime.UtcNow;

                await _attendanceRepository.EditAsync(attendance);

                var updatedEntity = await _attendanceRepository.FindByAsync(a => a.Id == attendance.Id)
                    .Include(a => a.Class).Include(a => a.Student).FirstOrDefaultAsync();
                var response = AttendanceMapper.ToResponse(updatedEntity ?? attendance);
                result.BuildResult(response, "Attendance has been updated successfully.");
            }
            catch (Exception ex)
            {
                result.BuildError(ex.Message + " " + ex.StackTrace);
            }
            return result;
        }
    }
}
