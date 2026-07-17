using MayNghien.Infrastructures.Models.Requests;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VAcademyConnect.Application.Interfaces.ClassStudents;
using VAcademyConnect.DTOs.Requests;

namespace VAcademyConnect.API.Controllers
{
    [Route("class-students")]
    [ApiController]
    [Authorize(Roles = "SuperAdmin, Admin, TenantAdmin, Teacher")]
    public class ClassStudentsController : ControllerBase
    {
        private readonly ICreateClassStudentHandler _createClassStudentHandler;
        private readonly IGetClassStudentByIdHandler _getClassStudentByIdHandler;
        private readonly ISearchClassStudentHandler _searchClassStudentHandler;

        public ClassStudentsController(ICreateClassStudentHandler createClassStudentHandler,
            IGetClassStudentByIdHandler getClassStudentByIdHandler, ISearchClassStudentHandler searchClassStudentHandler)
        {
            _createClassStudentHandler = createClassStudentHandler;
            _getClassStudentByIdHandler = getClassStudentByIdHandler;
            _searchClassStudentHandler = searchClassStudentHandler;
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] ClassStudentRequest request)
        {
            var result = await _createClassStudentHandler.HandleAsync(request);
            return Ok(result);
        }

        [HttpGet]
        [Route("{id}")]
        public async Task<IActionResult> GetById([FromRoute] Guid id)
        {
            var result = await _getClassStudentByIdHandler.HandleAsync(id);
            return Ok(result);
        }

        [HttpPost]
        [Route("search")]
        public async Task<IActionResult> Search(SearchRequest request)
        {
            var result = await _searchClassStudentHandler.HandleAsync(request);
            return Ok(result);
        }
    }
}
