using MayNghien.Infrastructures.Models.Requests;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VAcademyConnect.Application.Interfaces.LessonEvaluations;
using VAcademyConnect.DTOs.Requests;

namespace VAcademyConnect.API.Controllers
{
    [Route("lesson-evaluations")]
    [ApiController]
    [Authorize(Roles = "SuperAdmin, Admin, TenantAdmin, Teacher")]
    public class LessonEvaluationsController : ControllerBase
    {
        private readonly ICreateLessonEvaluationHandler _createLessonEvaluationHandler;
        private readonly IUpdateLessonEvaluationHandler _updateLessonEvaluationHandler;
        private readonly IDeleteLessonEvaluationHandler _deleteLessonEvaluationHandler;
        private readonly IGetLessonEvaluationByIdHandler _getLessonEvaluationByIdHandler;
        private readonly ISearchLessonEvaluationHandler _searchLessonEvaluationHandler;

        public LessonEvaluationsController(ICreateLessonEvaluationHandler createLessonEvaluationHandler,
            IUpdateLessonEvaluationHandler updateLessonEvaluationHandler, IDeleteLessonEvaluationHandler deleteLessonEvaluationHandler,
            IGetLessonEvaluationByIdHandler getLessonEvaluationByIdHandler, ISearchLessonEvaluationHandler searchLessonEvaluationHandler)
        {
            _createLessonEvaluationHandler = createLessonEvaluationHandler;
            _updateLessonEvaluationHandler = updateLessonEvaluationHandler;
            _deleteLessonEvaluationHandler = deleteLessonEvaluationHandler;
            _getLessonEvaluationByIdHandler = getLessonEvaluationByIdHandler;
            _searchLessonEvaluationHandler = searchLessonEvaluationHandler;
        }

        [HttpGet]
        [Route("{id}")]
        public async Task<IActionResult> GetById([FromRoute] Guid id)
        {
            var result = await _getLessonEvaluationByIdHandler.HandleAsync(id);
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] LessonEvaluationRequest request)
        {
            var result = await _createLessonEvaluationHandler.HandleAsync(request);
            return Ok(result);
        }

        [HttpPut]
        public async Task<IActionResult> Update([FromBody] LessonEvaluationRequest request)
        {
            var result = await _updateLessonEvaluationHandler.HandleAsync(request);
            return Ok(result);
        }

        [HttpDelete]
        [Route("{id}")]
        public async Task<IActionResult> Delete([FromRoute] Guid id)
        {
            var result = await _deleteLessonEvaluationHandler.HandleAsync(id);
            return Ok(result);
        }

        [HttpPost]
        [Route("search")]
        public async Task<IActionResult> Search([FromBody] SearchRequest request)
        {
            var result = await _searchLessonEvaluationHandler.HandleAsync(request);
            return Ok(result);
        }
    }
}
