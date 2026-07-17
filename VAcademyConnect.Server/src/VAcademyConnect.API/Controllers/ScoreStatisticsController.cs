using MayNghien.Infrastructures.Models.Requests;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VAcademyConnect.Application.Interfaces.ScoreStatistics;
using VAcademyConnect.DTOs.Requests;

namespace VAcademyConnect.API.Controllers
{
    [Route("score-statistics")]
    [ApiController]
    [Authorize(Roles = "SuperAdmin, Admin, TenantAdmin, Teacher")]
    public class ScoreStatisticsController : ControllerBase
    {
        private readonly ICreateScoreStatisticHandler _createScoreStatisticHandler;
        private readonly IUpdateScoreStatisticHandler _updateScoreStatisticHandler;
        private readonly IDeleteScoreStatisticHandler _deleteScoreStatisticHandler;
        private readonly IGetScoreStatisticByIdHandler _getScoreStatisticByIdHandler;
        private readonly ISearchScoreStatisticHandler _searchScoreStatisticHandler;
        private readonly ICalculateScoreStatisticHandler _calculateScoreStatisticHandler;

        public ScoreStatisticsController(ICreateScoreStatisticHandler createScoreStatisticHandler,
            IUpdateScoreStatisticHandler updateScoreStatisticHandler, IDeleteScoreStatisticHandler deleteScoreStatisticHandler,
            IGetScoreStatisticByIdHandler getScoreStatisticByIdHandler, ISearchScoreStatisticHandler searchScoreStatisticHandler,
            ICalculateScoreStatisticHandler calculateScoreStatisticHandler)
        {
            _createScoreStatisticHandler = createScoreStatisticHandler;
            _updateScoreStatisticHandler = updateScoreStatisticHandler;
            _deleteScoreStatisticHandler = deleteScoreStatisticHandler;
            _getScoreStatisticByIdHandler = getScoreStatisticByIdHandler;
            _searchScoreStatisticHandler = searchScoreStatisticHandler;
            _calculateScoreStatisticHandler = calculateScoreStatisticHandler;
        }

        [HttpGet]
        [Route("{id}")]
        public async Task<IActionResult> GetById([FromRoute] Guid id)
        {
            var result = await _getScoreStatisticByIdHandler.HandleAsync(id);
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] ScoreStatisticRequest request)
        {
            var result = await _createScoreStatisticHandler.HandleAsync(request);
            return Ok(result);
        }

        [HttpPut]
        public async Task<IActionResult> Update([FromBody] ScoreStatisticRequest request)
        {
            var result = await _updateScoreStatisticHandler.HandleAsync(request);
            return Ok(result);
        }

        [HttpDelete]
        [Route("{id}")]
        public async Task<IActionResult> Delete([FromRoute] Guid id)
        {
            var result = await _deleteScoreStatisticHandler.HandleAsync(id);
            return Ok(result);
        }

        [HttpPost]
        [Route("search")]
        public async Task<IActionResult> Search([FromBody] SearchRequest request)
        {
            var result = await _searchScoreStatisticHandler.HandleAsync(request);
            return Ok(result);
        }

        [HttpPost]
        [Route("calculate")]
        public async Task<IActionResult> Calculate([FromBody] ScoreStatisticCalculateRequest request)
        {
            var result = await _calculateScoreStatisticHandler.HandleAsync(request);
            return Ok(result);
        }
    }
}
