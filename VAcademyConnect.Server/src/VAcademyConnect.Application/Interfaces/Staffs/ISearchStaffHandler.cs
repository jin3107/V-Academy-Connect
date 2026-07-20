using MayNghien.Infrastructures.Models.Requests;
using MayNghien.Infrastructures.Models.Responses;
using VAcademyConnect.DTOs.Responses;

namespace VAcademyConnect.Application.Interfaces.Staffs
{
    public interface ISearchStaffHandler
    {
        Task<AppResponse<SearchResponse<StaffResponse>>> HandleAsync(SearchRequest request);
    }
}
