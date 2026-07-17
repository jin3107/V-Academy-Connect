using MayNghien.Infrastructures.Models.Requests;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VAcademyConnect.Application.Interfaces.Scores;
using VAcademyConnect.DTOs.Requests;

namespace VAcademyConnect.API.Controllers
{
    [Route("scores")]
    [ApiController]
    [Authorize(Roles = "SuperAdmin, Admin, TenantAdmin, Teacher")]
    public class ScoresController : ControllerBase
    {
        private readonly ICreateScoreHandler _createScoreHandler;
        private readonly IUpdateScoreHandler _updateScoreHandler;
        private readonly IDeleteScoreHandler _deleteScoreHandler;
        private readonly IGetScoreByIdHandler _getScoreByIdHandler;
        private readonly ISearchScoreHandler _searchScoreHandler;

        public ScoresController(ICreateScoreHandler createScoreHandler,
            IUpdateScoreHandler updateScoreHandler, IDeleteScoreHandler deleteScoreHandler,
            IGetScoreByIdHandler getScoreByIdHandler, ISearchScoreHandler searchScoreHandler)
        {
            _createScoreHandler = createScoreHandler;
            _updateScoreHandler = updateScoreHandler;
            _deleteScoreHandler = deleteScoreHandler;
            _getScoreByIdHandler = getScoreByIdHandler;
            _searchScoreHandler = searchScoreHandler;
        }

        [HttpGet]
        [Route("{id}")]
        public async Task<IActionResult> GetById([FromRoute] Guid id)
        {
            var result = await _getScoreByIdHandler.HandleAsync(id);
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] ScoreRequest request)
        {
            var result = await _createScoreHandler.HandleAsync(request);
            return Ok(result);
        }

        [HttpPut]
        public async Task<IActionResult> Update([FromBody] ScoreRequest request)
        {
            var result = await _updateScoreHandler.HandleAsync(request);
            return Ok(result);
        }

        [HttpDelete]
        [Route("{id}")]
        public async Task<IActionResult> Delete([FromRoute] Guid id)
        {
            var result = await _deleteScoreHandler.HandleAsync(id);
            return Ok(result);
        }

        [HttpPost]
        [Route("search")]
        public async Task<IActionResult> Search([FromBody] SearchRequest request)
        {
            var result = await _searchScoreHandler.HandleAsync(request);
            return Ok(result);
        }
    }
}
