using MayNghien.Infrastructures.Models.Responses;

namespace VAcademyConnect.Application.Interfaces.Staffs
{
    public interface IDeleteStaffHandler
    {
        Task<AppResponse<string>> HandleAsync(Guid id);
    }
}
