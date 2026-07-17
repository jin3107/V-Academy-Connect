using MayNghien.Infrastructures.Models.Requests;
using MayNghien.Infrastructures.Models.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VAcademyConnect.DTOs.Requests;
using VAcademyConnect.DTOs.Responses;

namespace VAcademyConnect.Application.Interfaces.Services
{
    public interface IClassStudentService
    {
        Task<AppResponse<ClassStudentResponse>> CreateAsync(ClassStudentRequest request);
        Task<AppResponse<ClassStudentResponse>> GetByIdAsync(Guid id);
        Task<AppResponse<string>> DeleteAsync(Guid id);
        Task<AppResponse<ClassStudentResponse>> UpdateAsync(ClassStudentRequest request);
        Task<AppResponse<SearchResponse<ClassStudentResponse>>> SearchAsync(SearchRequest request);
    }
}
