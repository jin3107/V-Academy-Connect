using MayNghien.Infrastructures.Models.Responses;

namespace VAcademyConnect.Application.Interfaces.Attendances
{
    public interface IDeleteAttendanceHandler
    {
        Task<AppResponse<string>> HandleAsync(Guid id);
    }
}
