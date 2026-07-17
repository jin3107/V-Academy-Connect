using LinqKit;
using MayNghien.Infrastructures.Models.Requests;
using MayNghien.Infrastructures.Models.Responses;
using Microsoft.EntityFrameworkCore;
using VAcademyConnect.Application.Interfaces;
using VAcademyConnect.Application.Interfaces.Classes;
using VAcademyConnect.Application.Interfaces.Repositories;
using VAcademyConnect.Application.Mapping;
using VAcademyConnect.DTOs.Responses;
using VAcademyConnect.Domain.Entities;
using static MayNghien.Infrastructures.Helpers.SearchHelper;

namespace VAcademyConnect.Application.Implementations.Classes
{
    public class SearchClassHandler : ISearchClassHandler
    {
        private readonly ICurrentUserService _currentUserService;
        private readonly IClassRepository _classRepository;
        private readonly ITenantRepository _tenantRepository;

        public SearchClassHandler(ICurrentUserService currentUserService, IClassRepository classRepository,
            ITenantRepository tenantRepository)
        {
            _currentUserService = currentUserService;
            _classRepository = classRepository;
            _tenantRepository = tenantRepository;
        }

        public async Task<AppResponse<SearchResponse<ClassResponse>>> HandleAsync(SearchRequest request)
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
                    .Include(c => c.ClassStudents).ThenInclude(cs => cs.Student)
                    .Include(c => c.Teacher).AsQueryable();

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
    }
}
