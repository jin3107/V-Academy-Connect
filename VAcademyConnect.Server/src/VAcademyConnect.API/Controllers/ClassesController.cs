using MayNghien.Infrastructures.Models.Requests;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VAcademyConnect.Application.Interfaces.Classes;
using VAcademyConnect.DTOs.Requests;

namespace VAcademyConnect.API.Controllers
{
    [Route("classes")]
    [ApiController]
    [Authorize(Roles = "SuperAdmin, Admin, TenantAdmin, Teacher")]
    public class ClassesController : ControllerBase
    {
        private readonly ICreateClassHandler _createClassHandler;
        private readonly IUpdateClassHandler _updateClassHandler;
        private readonly IDeleteClassHandler _deleteClassHandler;
        private readonly IGetClassByIdHandler _getClassByIdHandler;
        private readonly ISearchClassHandler _searchClassHandler;

        public ClassesController(ICreateClassHandler createClassHandler, IUpdateClassHandler updateClassHandler,
            IDeleteClassHandler deleteClassHandler, IGetClassByIdHandler getClassByIdHandler,
            ISearchClassHandler searchClassHandler)
        {
            _createClassHandler = createClassHandler;
            _updateClassHandler = updateClassHandler;
            _deleteClassHandler = deleteClassHandler;
            _getClassByIdHandler = getClassByIdHandler;
            _searchClassHandler = searchClassHandler;
        }

        [HttpGet]
        [Route("{id}")]
        public async Task<IActionResult> GetById([FromRoute] Guid id)
        {
            var result = await _getClassByIdHandler.HandleAsync(id);
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] ClassRequest request)
        {
            var result = await _createClassHandler.HandleAsync(request);
            return Ok(result);
        }

        [HttpPost]
        [Route("search")]
        public async Task<IActionResult> Search([FromBody] SearchRequest request)
        {
            var result = await _searchClassHandler.HandleAsync(request);
            return Ok(result);
        }

        [HttpPut]
        public async Task<IActionResult> Update([FromBody] ClassRequest request)
        {
            var result = await _updateClassHandler.HandleAsync(request);
            return Ok(result);
        }

        [HttpDelete]
        [Route("{id}")]
        public async Task<IActionResult> Delete([FromRoute] Guid id)
        {
            var result = await _deleteClassHandler.HandleAsync(id);
            return Ok(result);
        }
    }
}
