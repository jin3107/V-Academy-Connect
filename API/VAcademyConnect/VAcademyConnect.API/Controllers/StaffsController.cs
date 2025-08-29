using MayNghien.Infrastructures.Models.Requests;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using VAcademyConnect.DTOs.Requests;
using VAcademyConnect.Services.Interfaces;

namespace VAcademyConnect.API.Controllers
{
    [Route("staffs")]
    [ApiController]
    [Authorize(Roles = "TenantAdmin, SuperAdmin")]
    public class StaffsController : ControllerBase
    {
        private readonly IStaffService _staffService;

        public StaffsController(IStaffService staffService)
        {
            _staffService = staffService;
        }

        [HttpGet]
        [Route("{id}")]
        public async Task<IActionResult> GetById([FromRoute] Guid id)
        {
            var result = await _staffService.GetByIdAsync(id);
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] StaffRequest request)
        {
            var result = await _staffService.CreateAsync(request);
            return Ok(result);
        }

        [HttpPut]
        public async Task<IActionResult> Update([FromBody] StaffRequest request)
        {
            var result = await _staffService.UpdateAsync(request);
            return Ok(result);
        }

        [HttpDelete]
        [Route("{id}")]
        public async Task<IActionResult> Delete([FromRoute] Guid id)
        {
            var result = await _staffService.DeleteAsync(id);
            return Ok(result);
        }

        [HttpPost]
        [Route("search")]
        public async Task<IActionResult> Search([FromBody] SearchRequest request)
        {
            var result = await _staffService.SearchAsync(request);
            return Ok(result);
        }
    }
}
