using MayNghien.Infrastructures.Models.Responses;
using VAcademyConnect.DTOs.Responses;

namespace VAcademyConnect.Application.Interfaces.Staffs
{
    public interface IGetStaffByIdHandler
    {
        Task<AppResponse<StaffResponse>> HandleAsync(Guid id);
    }
}
