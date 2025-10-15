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
    public interface IClassService
    {
        Task<AppResponse<ClassResponse>> CreateAsync(ClassRequest request);
        Task<AppResponse<ClassResponse>> GetByIdAsync(Guid id);
        Task<AppResponse<string>> DeleteAsync(Guid id);
        Task<AppResponse<ClassResponse>> UpdateAsync(ClassRequest request);
        Task<AppResponse<SearchResponse<ClassResponse>>> SearchAsync(SearchRequest request);
    }
}
