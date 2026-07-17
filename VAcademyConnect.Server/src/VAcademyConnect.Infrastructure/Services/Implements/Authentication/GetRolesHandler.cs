using MayNghien.Infrastructures.Models.Responses;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using VAcademyConnect.Application.Interfaces.Authentication;
using VAcademyConnect.DTOs.Authentication.Responses;
using VAcademyConnect.Infrastructure.Services.Mapping;

namespace VAcademyConnect.Infrastructure.Services.Implements.Authentication
{
    public class GetRolesHandler : IGetRolesHandler
    {
        private readonly RoleManager<IdentityRole> _roleManager;

        public GetRolesHandler(RoleManager<IdentityRole> roleManager)
        {
            _roleManager = roleManager;
        }

        public async Task<AppResponse<List<RoleResponse>>> HandleAsync()
        {
            var result = new AppResponse<List<RoleResponse>>();
            try
            {
                var roles = await _roleManager.Roles.ToListAsync();
                var response = roles.Select(RoleMapper.ToResponse).ToList();
                result.BuildResult(response);
            }
            catch (Exception ex)
            {
                result.BuildError(ex.Message + " " + ex.StackTrace);
            }
            return result;
        }
    }
}
