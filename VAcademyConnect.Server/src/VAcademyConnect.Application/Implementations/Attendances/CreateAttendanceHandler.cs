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
    public class CreateAttendanceHandler : ICreateAttendanceHandler
    {
        private readonly IAttendanceRepository _attendanceRepository;
        private readonly ICurrentUserService _currentUserService;

        public CreateAttendanceHandler(IAttendanceRepository attendanceRepository, ICurrentUserService currentUserService)
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

                var attendance = AttendanceMapper.ToEntity(request);
                attendance.Id = Guid.NewGuid();
                attendance.CreatedBy = user.Email;
                attendance.CreatedOn = DateTime.UtcNow;

                await _attendanceRepository.AddAsync(attendance);

                var createdEntity = await _attendanceRepository.FindByAsync(a => a.Id == attendance.Id)
                    .Include(a => a.Class).Include(a => a.Student).FirstOrDefaultAsync();
                var response = AttendanceMapper.ToResponse(createdEntity ?? attendance);
                result.BuildResult(response, "Attendance has been recorded successfully.");
            }
            catch (Exception ex)
            {
                result.BuildError(ex.Message + " " + ex.StackTrace);
            }
            return result;
        }
    }
}
