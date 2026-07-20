using MayNghien.Infrastructures.Models.Requests;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VAcademyConnect.Application.Interfaces.BackgroundJobs;

namespace VAcademyConnect.API.Controllers
{
    [Route("background-jobs")]
    [ApiController]
    [Authorize(Roles = "SuperAdmin")]
    public class BackgroundJobsController : ControllerBase
    {
        private readonly IGetBackgroundJobByIdHandler _getBackgroundJobByIdHandler;
        private readonly ISearchBackgroundJobHandler _searchBackgroundJobHandler;

        public BackgroundJobsController(IGetBackgroundJobByIdHandler getBackgroundJobByIdHandler,
            ISearchBackgroundJobHandler searchBackgroundJobHandler)
        {
            _getBackgroundJobByIdHandler = getBackgroundJobByIdHandler;
            _searchBackgroundJobHandler = searchBackgroundJobHandler;
        }

        [HttpGet]
        [Route("{id}")]
        public async Task<IActionResult> GetById([FromRoute] Guid id)
        {
            var result = await _getBackgroundJobByIdHandler.HandleAsync(id);
            return Ok(result);
        }

        [HttpPost]
        [Route("search")]
        public async Task<IActionResult> Search([FromBody] SearchRequest request)
        {
            var result = await _searchBackgroundJobHandler.HandleAsync(request);
            return Ok(result);
        }
    }
}
