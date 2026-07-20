using MayNghien.Infrastructures.Models.Requests;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VAcademyConnect.Application.Interfaces.Tenants;
using VAcademyConnect.DTOs.Requests;

namespace VAcademyConnect.API.Controllers
{
    [Route("tenants")]
    [ApiController]
    [Authorize(Roles = "SuperAdmin, Admin")]
    public class TenantsController : ControllerBase
    {
        private readonly IUpdateTenantHandler _updateTenantHandler;
        private readonly IGetTenantByIdHandler _getTenantByIdHandler;
        private readonly ISearchTenantHandler _searchTenantHandler;
        private readonly IRenewTenantStandardSubscriptionHandler _renewTenantStandardSubscriptionHandler;

        public TenantsController(IUpdateTenantHandler updateTenantHandler, IGetTenantByIdHandler getTenantByIdHandler,
            ISearchTenantHandler searchTenantHandler, IRenewTenantStandardSubscriptionHandler renewTenantStandardSubscriptionHandler)
        {
            _updateTenantHandler = updateTenantHandler;
            _getTenantByIdHandler = getTenantByIdHandler;
            _searchTenantHandler = searchTenantHandler;
            _renewTenantStandardSubscriptionHandler = renewTenantStandardSubscriptionHandler;
        }

        [HttpGet]
        [Route("{id}")]
        public async Task<IActionResult> GetById([FromRoute] Guid id)
        {
            var result = await _getTenantByIdHandler.HandleAsync(id);
            return Ok(result);
        }

        [HttpPut]
        public async Task<IActionResult> Update([FromBody] TenantRequest request)
        {
            var result = await _updateTenantHandler.HandleAsync(request);
            return Ok(result);
        }

        [HttpPost]
        [Route("search")]
        public async Task<IActionResult> Search([FromBody] SearchRequest request)
        {
            var result = await _searchTenantHandler.HandleAsync(request);
            return Ok(result);
        }

        [HttpPut]
        [Route("{tenantId}/renew-subscription")]
        public async Task<IActionResult> RenewSubscription([FromRoute] Guid tenantId, [FromQuery] int months = 1)
        {
            var result = await _renewTenantStandardSubscriptionHandler.HandleAsync(tenantId);
            return Ok(result);
        }
    }
}
