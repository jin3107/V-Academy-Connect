using LinqKit;
using MayNghien.Infrastructures.Models.Requests;
using MayNghien.Infrastructures.Models.Responses;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VAcademyConnect.DTOs.Requests;
using VAcademyConnect.DTOs.Responses;
using VAcademyConnect.Domain.Entities;
using VAcademyConnect.Application.Interfaces;
using VAcademyConnect.Application.Interfaces.Repositories;
using VAcademyConnect.Application.Interfaces.Services;
using VAcademyConnect.Application.Services.Mapping;
using static MayNghien.Infrastructures.Helpers.SearchHelper;

namespace VAcademyConnect.Application.Services.Implements
{

    public class ClassService : IClassService
    {
        private readonly ICurrentUserService _currentUserService;
        private readonly IClassRepository _classRepository;
        private readonly ITenantRepository _tenantRepository;

        public ClassService(ICurrentUserService currentUserService, IClassRepository classRepository,
            ITenantRepository tenantRepository)
        {
            _currentUserService = currentUserService;
            _classRepository = classRepository;
            _tenantRepository = tenantRepository;
        }

        public async Task<AppResponse<ClassResponse>> CreateAsync(ClassRequest request)
        {
            var result = new AppResponse<ClassResponse>();
            try
            {
                var user = await _currentUserService.GetCurrentUserAsync();
                if (user == null)
                    return result.BuildError("Unauthorize.");

                var newClass = ClassMapper.ToEntity(request);
                newClass.Id = Guid.NewGuid();
                newClass.Name = request.Name;
                newClass.TenantId = request.TenantId;
                newClass.Tuition = request.Tuition;
                newClass.CreatedBy = user.Email;
                newClass.CreatedOn = DateTime.UtcNow;

                await _classRepository.AddAsync(newClass);
                var createdEntity = await _classRepository.FindByAsync(c => c.Id == newClass.Id)
                    .Include(c => c.ClassStudents).FirstOrDefaultAsync();
                var response = ClassMapper.ToResponse(newClass);
                if (request.TenantId.HasValue)
                {
                    var tenantName = _tenantRepository.FindByAsync(c => c.Id == newClass.TenantId).FirstOrDefault();
                    response.TenantName = tenantName?.Name ?? "None";
                }
                result.BuildResult(response, "Class has been created successfully.");
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
                var user = await _currentUserService.GetCurrentUserAsync();
                if (user == null)
                    return result.BuildError("Unauthorize");

                var classes = await _classRepository.GetAsync(id);
                if (classes == null || classes.IsDeleted == true)
                    return result.BuildError("Class was not found or has been deleted.");

                classes.IsDeleted = true;
                await _classRepository.EditAsync(classes);
                result.BuildResult("Class has been deleted successfully.");
            }
            catch (Exception ex)
            {
                result.BuildError(ex.Message + " " + ex.StackTrace);
            }
            return result;
        }

        public async Task<AppResponse<ClassResponse>> GetByIdAsync(Guid id)
        {
            var result = new AppResponse<ClassResponse>();
            try
            {
                var user = await _currentUserService.GetCurrentUserAsync();
                if (user == null)
                    return result.BuildError("Unthorize.");

                var classes = await _classRepository.FindByAsync(c => c.Id == id)
                    .Include(c => c.ClassStudents)!.ThenInclude(cs => cs.Student).FirstOrDefaultAsync();
                if (classes == null || classes.IsDeleted == true)
                    return result.BuildError("Class was not found or has been deleted.");

                var response = ClassMapper.ToResponse(classes);
                if (response.TenantId.HasValue)
                {
                    var tenantName = _tenantRepository.FindByAsync(c => c.Id == classes.TenantId).FirstOrDefault();
                    response.TenantName = tenantName?.Name ?? "None";
                }
                result.BuildResult(response, "Class has been found.");
            }
            catch (Exception ex)
            {
                result.BuildError(ex.Message + " " + ex.StackTrace);
            }
            return result;
        }

        public async Task<AppResponse<SearchResponse<ClassResponse>>> SearchAsync(SearchRequest request)
        {
            var result = new AppResponse<SearchResponse<ClassResponse>>();
            try
            {
                var user = await _currentUserService.GetCurrentUserAsync();
                if (user == null)
                    return result.BuildError("Unauthorize");

                var query = BuildFilterExpression(request.Filters!);
                if (user.Role != Commons.Enums.Role.SuperAdmin && user.TenantId.HasValue)
                    query = query.And(x => x.TenantId == user.TenantId);

                var numOfRecords = await _classRepository.CountRecordsAsync(query);
                var classes = _classRepository.FindByPredicate(query)
                    .Include(c => c.ClassStudents).ThenInclude(cs => cs.Student).AsQueryable();

                if (request.SortBy != null)
                    classes = _classRepository.AddSort(classes, request.SortBy);
                else
                    classes = classes.OrderBy(x => x.Name);

                int pageIndex = request.PageIndex ?? 1;
                int pageSize = request.PageSize ?? 10;
                int startIndex = (pageIndex - 1) * pageSize;
                var classList = await classes.Skip(startIndex).Take(pageSize).ToListAsync();
                var dtoList = classList.Select(ClassMapper.ToResponse).ToList();
                foreach (var classDto in dtoList)
                {
                    if (classDto.TenantId.HasValue)
                    {
                        var tenant = _tenantRepository.FindByAsync(c => c.Id == classDto.TenantId).FirstOrDefault();
                        classDto.TenantName = tenant?.Name ?? "None";
                    }
                }
                var searchResponse = new SearchResponse<ClassResponse>
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

        private ExpressionStarter<Class> BuildFilterExpression(List<Filter> filters)
        {
            try
            {
                var predicate = PredicateBuilder.New<Class>(true);
                if (filters != null)
                {
                    foreach (var filter in filters)
                    {
                        switch (filter.FieldName)
                        {
                            case "Name":
                                if (!string.IsNullOrEmpty(filter.Value))
                                    predicate = predicate.And(x => x.Name.Contains(filter.Value));
                                break;

                            case "StudentName":
                                if (!string.IsNullOrEmpty(filter.Value))
                                    predicate = predicate.And(x => x.ClassStudents!.Any(cs =>
                                        cs.Student!.FullName!.Contains(filter.Value)));
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

        public async Task<AppResponse<ClassResponse>> UpdateAsync(ClassRequest request)
        {
            var result = new AppResponse<ClassResponse>();
            try
            {
                var user = await _currentUserService.GetCurrentUserAsync();
                if (user == null)
                    return result.BuildError("Unauthorize.");

                var classes = await _classRepository.GetAsync(request.Id);
                if (classes == null || classes.IsDeleted == true)
                    return result.BuildError("Class was not found or has been deleted.");

                classes.Name = request.Name;
                classes.TenantId = request.TenantId;
                classes.Tuition = request.Tuition;
                classes.ModifiedBy = user.Email;
                classes.ModifiedOn = DateTime.UtcNow;

                await _classRepository.EditAsync(classes);
                var response = ClassMapper.ToResponse(classes);
                if (response.TenantId.HasValue)
                {
                    var tenantName = _tenantRepository.FindByAsync(c => c.Id == classes.TenantId).FirstOrDefault();
                    response.TenantName = tenantName?.Name ?? "None";
                }
                result.BuildResult(response, "Class has been updated successfully.");
            }
            catch (Exception ex)
            {
                result.BuildError(ex.Message + " " + ex.StackTrace);
            }
            return result;
        }
    }
}
