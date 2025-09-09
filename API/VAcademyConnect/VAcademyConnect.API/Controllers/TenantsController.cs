using MayNghien.Infrastructures.Models.Requests;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using VAcademyConnect.Services.Implements;
using VAcademyConnect.Services.Interfaces;

namespace VAcademyConnect.API.Controllers
{
    [Route("tenants")]
    [ApiController]
    [Authorize(Roles = "SuperAdmin, Admin")]
    public class TenantsController : ControllerBase
    {
        private readonly ITenantService _tenantService;

        public TenantsController(ITenantService tenantService)
        {
            _tenantService = tenantService;
        }

        [HttpPost]
        [Route("search")]
        public async Task<IActionResult> Search([FromBody] SearchRequest request)
        {
            var result = await _tenantService.SearchAsync(request);
            return Ok(result);
        }
    }
}
