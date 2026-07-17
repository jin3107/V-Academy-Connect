using MayNghien.Infrastructures.Models.Responses;
using VAcademyConnect.DTOs.Responses;

namespace VAcademyConnect.Application.Interfaces.Attendances
{
    public interface IGetAttendanceByIdHandler
    {
        Task<AppResponse<AttendanceResponse>> HandleAsync(Guid id);
    }
}
