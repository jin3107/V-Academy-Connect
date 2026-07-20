using MayNghien.Infrastructures.Models.Requests;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VAcademyConnect.Application.Interfaces.Payments;
using VAcademyConnect.DTOs.Requests;

namespace VAcademyConnect.API.Controllers
{
    [Route("payments")]
    [ApiController]
    [Authorize(Roles = "SuperAdmin, Admin, TenantAdmin, Teacher")]
    public class PaymentsController : ControllerBase
    {
        private readonly ICreatePaymentHandler _createPaymentHandler;
        private readonly IUpdatePaymentHandler _updatePaymentHandler;
        private readonly IDeletePaymentHandler _deletePaymentHandler;
        private readonly IGetPaymentByIdHandler _getPaymentByIdHandler;
        private readonly ISearchPaymentHandler _searchPaymentHandler;

        public PaymentsController(ICreatePaymentHandler createPaymentHandler, IUpdatePaymentHandler updatePaymentHandler,
            IDeletePaymentHandler deletePaymentHandler, IGetPaymentByIdHandler getPaymentByIdHandler,
            ISearchPaymentHandler searchPaymentHandler)
        {
            _createPaymentHandler = createPaymentHandler;
            _updatePaymentHandler = updatePaymentHandler;
            _deletePaymentHandler = deletePaymentHandler;
            _getPaymentByIdHandler = getPaymentByIdHandler;
            _searchPaymentHandler = searchPaymentHandler;
        }

        [HttpGet]
        [Route("{id}")]
        public async Task<IActionResult> GetById([FromRoute] Guid id)
        {
            var result = await _getPaymentByIdHandler.HandleAsync(id);
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] PaymentRequest request)
        {
            var result = await _createPaymentHandler.HandleAsync(request);
            return Ok(result);
        }

        [HttpPut]
        public async Task<IActionResult> Update([FromBody] PaymentRequest request)
        {
            var result = await _updatePaymentHandler.HandleAsync(request);
            return Ok(result);
        }

        [HttpDelete]
        [Route("{id}")]
        public async Task<IActionResult> Delete([FromRoute] Guid id)
        {
            var result = await _deletePaymentHandler.HandleAsync(id);
            return Ok(result);
        }

        [HttpPost]
        [Route("search")]
        public async Task<IActionResult> Search([FromBody] SearchRequest request)
        {
            var result = await _searchPaymentHandler.HandleAsync(request);
            return Ok(result);
        }
    }
}
