using MayNghien.Infrastructures.Models.Responses;
using VAcademyConnect.DTOs.Requests;
using VAcademyConnect.DTOs.Responses;

namespace VAcademyConnect.Application.Interfaces.Staffs
{
    public interface IUpdateStaffHandler
    {
        Task<AppResponse<StaffResponse>> HandleAsync(StaffRequest request);
    }
}
