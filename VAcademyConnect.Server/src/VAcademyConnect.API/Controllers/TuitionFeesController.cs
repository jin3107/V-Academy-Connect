using MayNghien.Infrastructures.Models.Requests;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VAcademyConnect.Application.Interfaces.TuitionFees;
using VAcademyConnect.DTOs.Requests;

namespace VAcademyConnect.API.Controllers
{
    [Route("tuition-fees")]
    [ApiController]
    [Authorize(Roles = "SuperAdmin, Admin, TenantAdmin, Teacher")]
    public class TuitionFeesController : ControllerBase
    {
        private readonly ICreateTuitionFeeHandler _createTuitionFeeHandler;
        private readonly IUpdateTuitionFeeHandler _updateTuitionFeeHandler;
        private readonly IDeleteTuitionFeeHandler _deleteTuitionFeeHandler;
        private readonly IGetTuitionFeeByIdHandler _getTuitionFeeByIdHandler;
        private readonly ISearchTuitionFeeHandler _searchTuitionFeeHandler;
        private readonly IEstimateTuitionFeeHandler _estimateTuitionFeeHandler;

        public TuitionFeesController(ICreateTuitionFeeHandler createTuitionFeeHandler,
            IUpdateTuitionFeeHandler updateTuitionFeeHandler, IDeleteTuitionFeeHandler deleteTuitionFeeHandler,
            IGetTuitionFeeByIdHandler getTuitionFeeByIdHandler, ISearchTuitionFeeHandler searchTuitionFeeHandler,
            IEstimateTuitionFeeHandler estimateTuitionFeeHandler)
        {
            _createTuitionFeeHandler = createTuitionFeeHandler;
            _updateTuitionFeeHandler = updateTuitionFeeHandler;
            _deleteTuitionFeeHandler = deleteTuitionFeeHandler;
            _getTuitionFeeByIdHandler = getTuitionFeeByIdHandler;
            _searchTuitionFeeHandler = searchTuitionFeeHandler;
            _estimateTuitionFeeHandler = estimateTuitionFeeHandler;
        }

        [HttpGet]
        [Route("{id}")]
        public async Task<IActionResult> GetById([FromRoute] Guid id)
        {
            var result = await _getTuitionFeeByIdHandler.HandleAsync(id);
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] TuitionFeeRequest request)
        {
            var result = await _createTuitionFeeHandler.HandleAsync(request);
            return Ok(result);
        }

        [HttpPut]
        public async Task<IActionResult> Update([FromBody] TuitionFeeRequest request)
        {
            var result = await _updateTuitionFeeHandler.HandleAsync(request);
            return Ok(result);
        }

        [HttpDelete]
        [Route("{id}")]
        public async Task<IActionResult> Delete([FromRoute] Guid id)
        {
            var result = await _deleteTuitionFeeHandler.HandleAsync(id);
            return Ok(result);
        }

        [HttpPost]
        [Route("search")]
        public async Task<IActionResult> Search([FromBody] SearchRequest request)
        {
            var result = await _searchTuitionFeeHandler.HandleAsync(request);
            return Ok(result);
        }

        [HttpPost]
        [Route("estimate")]
        public async Task<IActionResult> Estimate([FromBody] TuitionFeeEstimateRequest request)
        {
            var result = await _estimateTuitionFeeHandler.HandleAsync(request);
            return Ok(result);
        }
    }
}
