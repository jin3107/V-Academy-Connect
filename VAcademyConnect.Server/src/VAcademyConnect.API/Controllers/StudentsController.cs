using MayNghien.Infrastructures.Models.Requests;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VAcademyConnect.Application.Interfaces.Students;
using VAcademyConnect.DTOs.Requests;

namespace VAcademyConnect.API.Controllers
{
    [Route("students")]
    [ApiController]
    [Authorize(Roles = "SuperAdmin, Admin, TenantAdmin, Teacher")]
    public class StudentsController : ControllerBase
    {
        private readonly ICreateStudentHandler _createStudentHandler;
        private readonly ISearchStudentHandler _searchStudentHandler;

        public StudentsController(ICreateStudentHandler createStudentHandler, ISearchStudentHandler searchStudentHandler)
        {
            _createStudentHandler = createStudentHandler;
            _searchStudentHandler = searchStudentHandler;
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] StudentRequest request)
        {
            var result = await _createStudentHandler.HandleAsync(request);
            return Ok(result);
        }

        [HttpPost]
        [Route("search")]
        public async Task<IActionResult> Search([FromBody] SearchRequest request)
        {
            var result = await _searchStudentHandler.HandleAsync(request);
            return Ok(result);
        }
    }
}
