using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using VAcademyConnect.DTOs.Requests;
using VAcademyConnect.Services.Interfaces;

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
    }
}
