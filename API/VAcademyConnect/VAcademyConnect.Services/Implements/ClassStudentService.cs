using LinqKit;
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
using static MayNghien.Infrastructures.Helpers.SearchHelper;

namespace VAcademyConnect.Services.Implements
{
    public class ClassStudentService : IClassStudentService
    {
        private readonly IClassStudentRepository _classStudentRepository;
        private  readonly UserManager<ApplicationUser> _userManager;
        private readonly IHttpContextAccessor _contextAccessor;

        public ClassStudentService(IClassStudentRepository classStudentRepository, UserManager<ApplicationUser> userManager, 
            IHttpContextAccessor contextAccessor)
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

        public async Task<AppResponse<string>> DeleteAsync(Guid id)
        {
            var result = new AppResponse<string>();
            try
            {
                var user = _userManager.FindByEmailAsync(_contextAccessor.HttpContext?.User.Identity?.Name!);
                if (user == null)
                    return result.BuildError("Unauthorize");

                var classStudent = await _classStudentRepository.GetAsync(id);
                if (classStudent == null || classStudent.IsDeleted == true)
                    return result.BuildError("Student in class was not found or has been deleted.");

                classStudent.IsDeleted = true;
                await _classStudentRepository.EditAsync(classStudent);
                result.BuildResult("Student in class has been deleted successfully.");
            }
            catch (Exception ex)
            {
                result.BuildError(ex.Message + " " + ex.StackTrace);
            }
            return result;
        }

        public async Task<AppResponse<ClassStudentResponse>> GetByIdAsync(Guid id)
        {
            var result = new AppResponse<ClassStudentResponse>();
            try
            {
                var user = await _userManager.FindByEmailAsync(_contextAccessor.HttpContext?.User.Identity?.Name!);
                if (user == null)
                    return result.BuildError("Unauthorize.");

                var classStudent = await _classStudentRepository.FindByAsync(c => c.Id == id)
                    .Include(c => c.Student).Include(c => c.Class).FirstOrDefaultAsync();
                if (classStudent == null || classStudent.IsDeleted == true)
                    return result.BuildError("Student in class was not found or has been deleted.");

                var response = ClassStudentMapper.ToResponse(classStudent);
                return result.BuildResult(response, "Student in class has been found");
            }
            catch (Exception ex)
            {
                result.BuildError(ex.Message + " " + ex.StackTrace);
            }
            return result;
        }

        public async Task<AppResponse<SearchResponse<ClassStudentResponse>>> SearchAsync(SearchRequest request)
        {
            var result = new AppResponse<SearchResponse<ClassStudentResponse>>();
            try
            {
                var user = await _userManager.FindByEmailAsync(_contextAccessor.HttpContext?.User.Identity?.Name!);
                if (user == null)
                    return result.BuildError("Unauthorize.");

                var query = BuildFilterExpression(request.Filters!);
                var numOfRecords = await _classStudentRepository.CountRecordsAsync(query);
                var classStudents = _classStudentRepository.FindByPredicate(query)
                    .Include(cs => cs.Student).Include(cs => cs.Class).AsQueryable();

                if (request.SortBy != null)
                    classStudents = _classStudentRepository.AddSort(classStudents, request.SortBy);
                else
                    classStudents = classStudents.OrderByDescending(x => x.CreatedOn);

                int pageIndex = request.PageIndex ?? 1;
                int pageSize = request.PageSize ?? 10;
                int startIndex = (pageIndex - 1) * pageSize;

                var classStudentList = await classStudents.Skip(startIndex).Take(pageSize).ToListAsync();
                var dtoList = classStudentList.Select(ClassStudentMapper.ToResponse).ToList();

                var searchResponse = new SearchResponse<ClassStudentResponse>
                {
                    TotalPages = CalculateNumOfPages(numOfRecords, pageSize),
                    TotalRows = numOfRecords,
                    CurrentPage = pageIndex,
                    Data = dtoList,
                    RowsPerPage = pageSize,
                };
                result.Data = searchResponse;
                result.IsSuccess = true;
            }
            catch (Exception ex)
            {
                result.BuildError(ex.Message + " " + ex.StackTrace);
            }
            return result;
        }

        private ExpressionStarter<ClassStudent> BuildFilterExpression(List<Filter> filters)
        {
            try
            {
                var predicate = PredicateBuilder.New<ClassStudent>(true);
                if (filters != null)
                {
                    foreach (var filter in filters)
                    {
                        switch (filter.FieldName)
                        {
                            case "StudentName":
                                if (!string.IsNullOrEmpty(filter.Value))
                                    predicate = predicate.And(x => x.Student!.FullName!.Contains(filter.Value));
                                break;

                            case "ClassName":
                                if (!string.IsNullOrEmpty(filter.Value))
                                    predicate = predicate.And(x => x.Class!.Name!.Contains(filter.Value));
                                break;

                           default: break;
                        }
                    }
                }

                predicate = predicate.And(x => x.IsDeleted == false);
                return predicate;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message + " " + ex.StackTrace);
            }
        }

        public Task<AppResponse<ClassStudentResponse>> UpdateAsync(ClassStudentRequest request)
        {
            throw new NotImplementedException();
        }
    }
}
