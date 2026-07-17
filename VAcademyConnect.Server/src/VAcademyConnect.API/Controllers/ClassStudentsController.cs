using MayNghien.Infrastructures.Models.Requests;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using VAcademyConnect.DTOs.Requests;
using VAcademyConnect.Application.Interfaces.Services;

namespace VAcademyConnect.API.Controllers
{
    [Route("class-students")]
    [ApiController]
    [Authorize(Roles = "SuperAdmin, Admin, TenantAdmin, Teacher")]
    public class ClassStudentsController : ControllerBase
    {
        private readonly IClassStudentService _classStudentService;

        public ClassStudentsController(IClassStudentService classStudentService)
        {
            _classStudentService = classStudentService;
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] ClassStudentRequest request)
        {
            var result = await _classStudentService.CreateAsync(request);
            return Ok(result);
        }

        [HttpGet]
        [Route("{id}")]
        public async Task<IActionResult> GetById([FromRoute] Guid id)
        {
            var result = await _classStudentService.GetByIdAsync(id);
            return Ok(result);
        }

        [HttpPost]
        [Route("search")]
        public async Task<IActionResult> Search(SearchRequest request)
        {
            var result = await _classStudentService.SearchAsync(request);
            return Ok(result);
        }
    }
}
