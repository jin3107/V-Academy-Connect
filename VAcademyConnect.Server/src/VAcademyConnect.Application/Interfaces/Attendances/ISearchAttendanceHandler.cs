using MayNghien.Infrastructures.Models.Requests;
using MayNghien.Infrastructures.Models.Responses;
using VAcademyConnect.DTOs.Responses;

namespace VAcademyConnect.Application.Interfaces.Attendances
{
    public interface ISearchAttendanceHandler
    {
        Task<AppResponse<SearchResponse<AttendanceResponse>>> HandleAsync(SearchRequest request);
    }
}
