using MayNghien.Infrastructures.Models.Requests;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VAcademyConnect.Application.Interfaces.Attendances;
using VAcademyConnect.DTOs.Requests;

namespace VAcademyConnect.API.Controllers
{
    [Route("attendances")]
    [ApiController]
    [Authorize(Roles = "SuperAdmin, Admin, TenantAdmin, Teacher")]
    public class AttendancesController : ControllerBase
    {
        private readonly ICreateAttendanceHandler _createAttendanceHandler;
        private readonly IUpdateAttendanceHandler _updateAttendanceHandler;
        private readonly IDeleteAttendanceHandler _deleteAttendanceHandler;
        private readonly IGetAttendanceByIdHandler _getAttendanceByIdHandler;
        private readonly ISearchAttendanceHandler _searchAttendanceHandler;

        public AttendancesController(ICreateAttendanceHandler createAttendanceHandler,
            IUpdateAttendanceHandler updateAttendanceHandler, IDeleteAttendanceHandler deleteAttendanceHandler,
            IGetAttendanceByIdHandler getAttendanceByIdHandler, ISearchAttendanceHandler searchAttendanceHandler)
        {
            _createAttendanceHandler = createAttendanceHandler;
            _updateAttendanceHandler = updateAttendanceHandler;
            _deleteAttendanceHandler = deleteAttendanceHandler;
            _getAttendanceByIdHandler = getAttendanceByIdHandler;
            _searchAttendanceHandler = searchAttendanceHandler;
        }

        [HttpGet]
        [Route("{id}")]
        public async Task<IActionResult> GetById([FromRoute] Guid id)
        {
            var result = await _getAttendanceByIdHandler.HandleAsync(id);
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] AttendanceRequest request)
        {
            var result = await _createAttendanceHandler.HandleAsync(request);
            return Ok(result);
        }

        [HttpPut]
        public async Task<IActionResult> Update([FromBody] AttendanceRequest request)
        {
            var result = await _updateAttendanceHandler.HandleAsync(request);
            return Ok(result);
        }

        [HttpDelete]
        [Route("{id}")]
        public async Task<IActionResult> Delete([FromRoute] Guid id)
        {
            var result = await _deleteAttendanceHandler.HandleAsync(id);
            return Ok(result);
        }

        [HttpPost]
        [Route("search")]
        public async Task<IActionResult> Search([FromBody] SearchRequest request)
        {
            var result = await _searchAttendanceHandler.HandleAsync(request);
            return Ok(result);
        }
    }
}
