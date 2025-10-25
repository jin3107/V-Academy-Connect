using MayNghien.Infrastructures.Models.Requests;
using MayNghien.Infrastructures.Models.Responses;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VAcademyConnect.DTOs.Requests;
using VAcademyConnect.DTOs.Responses;
using VAcademyConnect.Models.Entities;
using VAcademyConnect.Repositories.Interfaces;
using VAcademyConnect.Services.Interfaces;
using VAcademyConnect.Services.Mapping;

namespace VAcademyConnect.Services.Implements
{
    public class ClassStudentService : IClassStudentService
    {
        private readonly IClassStudentRepository _classStudentRepository;
        private  readonly UserManager<ApplicationUser> _userManager;
        private readonly IHttpContextAccessor _contextAccessor;

        public ClassStudentService(IClassStudentRepository classStudentRepository, UserManager<ApplicationUser> userManager, IHttpContextAccessor contextAccessor)
        {
            _classStudentRepository = classStudentRepository;
            _userManager = userManager;
            _contextAccessor = contextAccessor;
        }

        public async Task<AppResponse<ClassStudentResponse>> CreateAsync(ClassStudentRequest request)
        {
            var result = new AppResponse<ClassStudentResponse>();
            try
            {
                var user = await _userManager.FindByEmailAsync(_contextAccessor.HttpContext?.User.Identity?.Name!);
                if (user == null)
                    return result.BuildError("Unauthorize.");

                var existingEnrollment = await _classStudentRepository
                    .FindByAsync(cs => cs.ClassId == request.ClassId && cs.StudentId == request.StudentId)
                    .FirstOrDefaultAsync();

                if (existingEnrollment != null && !existingEnrollment.IsDeleted)
                    return result.BuildError("Student is already enrolled in this class.");

                var newStudentInClass = ClassStudentMapper.ToEntity(request);
                newStudentInClass.Id = Guid.NewGuid();
                newStudentInClass.CreatedBy = user.Email;
                newStudentInClass.CreatedOn = DateTime.UtcNow;

                await _classStudentRepository.AddAsync(newStudentInClass);

                var createdEntity = await _classStudentRepository.FindByAsync(cs => cs.Id == newStudentInClass.Id)
                    .Include(cs => cs.Student).FirstOrDefaultAsync();

                var response = ClassStudentMapper.ToResponse(createdEntity!);
                result.BuildResult(response, "Student has been added to class successfully.");
            }
            catch (Exception ex)
            {
                result.BuildError(ex.Message + " " + ex.StackTrace);
            }
            return result;
        }

        public Task<AppResponse<string>> DeleteAsync(Guid id)
        {
            throw new NotImplementedException();
        }

        public Task<AppResponse<ClassStudentResponse>> GetByIdAsync(Guid id)
        {
            throw new NotImplementedException();
        }

        public Task<AppResponse<SearchResponse<ClassStudentResponse>>> SearchAsync(SearchRequest request)
        {
            throw new NotImplementedException();
        }

        public Task<AppResponse<ClassStudentResponse>> UpdateAsync(ClassStudentRequest request)
        {
            throw new NotImplementedException();
        }
    }
}
