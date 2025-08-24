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
        Task<AppResponse<StaffResponse>> CreateAsync(StaffRequest request);
        Task<AppResponse<StaffResponse>> UpdateAsync(StaffRequest request);
        Task<AppResponse<string>> Delete(Guid id);
        Task<SearchResponse<StaffResponse>> SearchAsync(SearchRequest request);
    }
}
