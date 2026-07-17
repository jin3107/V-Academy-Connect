using MayNghien.Infrastructures.Models.Responses;
using VAcademyConnect.DTOs.Authentication.Responses;

namespace VAcademyConnect.Application.Interfaces.Authentication
{
    public interface IGetRolesHandler
    {
        Task<AppResponse<List<RoleResponse>>> HandleAsync();
    }
}
