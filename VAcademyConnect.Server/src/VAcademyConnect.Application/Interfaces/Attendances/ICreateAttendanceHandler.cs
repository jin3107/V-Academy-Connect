using MayNghien.Infrastructures.Models.Responses;
using VAcademyConnect.DTOs.Requests;
using VAcademyConnect.DTOs.Responses;

namespace VAcademyConnect.Application.Interfaces.Attendances
{
    public interface ICreateAttendanceHandler
    {
        Task<AppResponse<AttendanceResponse>> HandleAsync(AttendanceRequest request);
    }
}
