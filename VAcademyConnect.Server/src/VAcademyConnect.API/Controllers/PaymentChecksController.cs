using MayNghien.Infrastructures.Models.Requests;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VAcademyConnect.Application.Interfaces.PaymentChecks;
using VAcademyConnect.DTOs.Requests;

namespace VAcademyConnect.API.Controllers
{
    [Route("payment-checks")]
    [ApiController]
    [Authorize(Roles = "SuperAdmin, Admin, TenantAdmin, Teacher")]
    public class PaymentChecksController : ControllerBase
    {
        private readonly ICreatePaymentCheckHandler _createPaymentCheckHandler;
        private readonly IGetPaymentCheckByIdHandler _getPaymentCheckByIdHandler;
        private readonly ISearchPaymentCheckHandler _searchPaymentCheckHandler;

        public PaymentChecksController(ICreatePaymentCheckHandler createPaymentCheckHandler,
            IGetPaymentCheckByIdHandler getPaymentCheckByIdHandler, ISearchPaymentCheckHandler searchPaymentCheckHandler)
        {
            _createPaymentCheckHandler = createPaymentCheckHandler;
            _getPaymentCheckByIdHandler = getPaymentCheckByIdHandler;
            _searchPaymentCheckHandler = searchPaymentCheckHandler;
        }

        [HttpGet]
        [Route("{id}")]
        public async Task<IActionResult> GetById([FromRoute] Guid id)
        {
            var result = await _getPaymentCheckByIdHandler.HandleAsync(id);
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] PaymentCheckRequest request)
        {
            var result = await _createPaymentCheckHandler.HandleAsync(request);
            return Ok(result);
        }

        [HttpPost]
        [Route("search")]
        public async Task<IActionResult> Search([FromBody] SearchRequest request)
        {
            var result = await _searchPaymentCheckHandler.HandleAsync(request);
            return Ok(result);
        }
    }
}
