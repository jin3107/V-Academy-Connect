using MayNghien.Infrastructures.Models.Requests;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using VAcademyConnect.DTOs.Requests;
using VAcademyConnect.Infrastructure.Services.Implements;
using VAcademyConnect.Application.Interfaces.Services;

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

        [HttpGet]
        [Route("{id}")]
        public async Task<IActionResult> GetById([FromRoute] Guid id)
        {
            var result = await _tenantService.GetByIdAsync(id);
            return Ok(result);
        }

        [HttpPut]
        public async Task<IActionResult> Update([FromBody] TenantRequest request)
        {
            var result = await _tenantService.UpdateAsync(request);
            return Ok(result);
        }

        [HttpPost]
        [Route("search")]
        public async Task<IActionResult> Search([FromBody] SearchRequest request)
        {
            var result = await _tenantService.SearchAsync(request);
            return Ok(result);
        }

        [HttpPut]
        [Route("{tenantId}/renew-subscription")]
        public async Task<IActionResult> RenewSubscription([FromRoute] Guid tenantId, [FromQuery] int months = 1)
        {
            var result = await _tenantService.RenewStandardSubscriptionAsync(tenantId);
            return Ok(result);
        }
    }
}
