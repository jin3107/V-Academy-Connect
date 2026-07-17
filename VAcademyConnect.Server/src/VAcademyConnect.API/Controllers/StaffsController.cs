using MayNghien.Infrastructures.Models.Requests;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VAcademyConnect.Application.Interfaces.Staffs;
using VAcademyConnect.DTOs.Requests;

namespace VAcademyConnect.API.Controllers
{
    [Route("staffs")]
    [ApiController]
    [Authorize(Roles = "TenantAdmin, SuperAdmin")]
    public class StaffsController : ControllerBase
    {
        private readonly ICreateStaffHandler _createStaffHandler;
        private readonly IUpdateStaffHandler _updateStaffHandler;
        private readonly IDeleteStaffHandler _deleteStaffHandler;
        private readonly IGetStaffByIdHandler _getStaffByIdHandler;
        private readonly ISearchStaffHandler _searchStaffHandler;

        public StaffsController(ICreateStaffHandler createStaffHandler, IUpdateStaffHandler updateStaffHandler,
            IDeleteStaffHandler deleteStaffHandler, IGetStaffByIdHandler getStaffByIdHandler,
            ISearchStaffHandler searchStaffHandler)
        {
            _createStaffHandler = createStaffHandler;
            _updateStaffHandler = updateStaffHandler;
            _deleteStaffHandler = deleteStaffHandler;
            _getStaffByIdHandler = getStaffByIdHandler;
            _searchStaffHandler = searchStaffHandler;
        }

        [HttpGet]
        [Route("{id}")]
        public async Task<IActionResult> GetById([FromRoute] Guid id)
        {
            var result = await _getStaffByIdHandler.HandleAsync(id);
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] StaffRequest request)
        {
            var result = await _createStaffHandler.HandleAsync(request);
            return Ok(result);
        }

        [HttpPut]
        public async Task<IActionResult> Update([FromBody] StaffRequest request)
        {
            var result = await _updateStaffHandler.HandleAsync(request);
            return Ok(result);
        }

        [HttpDelete]
        [Route("{id}")]
        public async Task<IActionResult> Delete([FromRoute] Guid id)
        {
            var result = await _deleteStaffHandler.HandleAsync(id);
            return Ok(result);
        }

        [HttpPost]
        [Route("search")]
        public async Task<IActionResult> Search([FromBody] SearchRequest request)
        {
            var result = await _searchStaffHandler.HandleAsync(request);
            return Ok(result);
        }
    }
}
