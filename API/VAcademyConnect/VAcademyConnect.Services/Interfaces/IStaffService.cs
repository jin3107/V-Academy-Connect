using MayNghien.Infrastructures.Models.Requests;
using MayNghien.Infrastructures.Models.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VAcademyConnect.DTOs.Requests;
using VAcademyConnect.DTOs.Responses;

namespace VAcademyConnect.Services.Interfaces
{
    public interface IStaffService
    {
        Task<AppResponse<StaffResponse>> GetByIdAsync(Guid id);
        Task<AppResponse<StaffResponse>> CreateAsync(CreateStaffRequest request);
        Task<AppResponse<StaffResponse>> UpdateAsync(UpdateStaffRequest request);
        Task<AppResponse<string>> DeleteAsync(Guid id);
        Task<AppResponse<SearchResponse<StaffResponse>>> SearchAsync(SearchRequest request);
    }
}
