using MayNghien.Infrastructures.Models.Requests;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VAcademyConnect.Application.Interfaces.Subscriptions;
using VAcademyConnect.DTOs.Requests;

namespace VAcademyConnect.API.Controllers
{
    [Route("subscriptions")]
    [ApiController]
    [Authorize(Roles = "SuperAdmin, Admin")]
    public class SubscriptionsController : ControllerBase
    {
        private readonly ICreateSubscriptionHandler _createSubscriptionHandler;
        private readonly IUpdateSubscriptionHandler _updateSubscriptionHandler;
        private readonly IDeleteSubscriptionHandler _deleteSubscriptionHandler;
        private readonly IGetSubscriptionByIdHandler _getSubscriptionByIdHandler;
        private readonly ISearchSubscriptionHandler _searchSubscriptionHandler;

        public SubscriptionsController(ICreateSubscriptionHandler createSubscriptionHandler,
            IUpdateSubscriptionHandler updateSubscriptionHandler, IDeleteSubscriptionHandler deleteSubscriptionHandler,
            IGetSubscriptionByIdHandler getSubscriptionByIdHandler, ISearchSubscriptionHandler searchSubscriptionHandler)
        {
            _createSubscriptionHandler = createSubscriptionHandler;
            _updateSubscriptionHandler = updateSubscriptionHandler;
            _deleteSubscriptionHandler = deleteSubscriptionHandler;
            _getSubscriptionByIdHandler = getSubscriptionByIdHandler;
            _searchSubscriptionHandler = searchSubscriptionHandler;
        }

        [HttpGet]
        [Route("{id}")]
        public async Task<IActionResult> GetById([FromRoute] Guid id)
        {
            var result = await _getSubscriptionByIdHandler.HandleAsync(id);
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] SubscriptionRequest request)
        {
            var result = await _createSubscriptionHandler.HandleAsync(request);
            return Ok(result);
        }

        [HttpPut]
        public async Task<IActionResult> Update([FromBody] SubscriptionRequest request)
        {
            var result = await _updateSubscriptionHandler.HandleAsync(request);
            return Ok(result);
        }

        [HttpDelete]
        [Route("{id}")]
        public async Task<IActionResult> Delete([FromRoute] Guid id)
        {
            var result = await _deleteSubscriptionHandler.HandleAsync(id);
            return Ok(result);
        }

        [HttpPost]
        [Route("search")]
        public async Task<IActionResult> Search([FromBody] SearchRequest request)
        {
            var result = await _searchSubscriptionHandler.HandleAsync(request);
            return Ok(result);
        }
    }
}
