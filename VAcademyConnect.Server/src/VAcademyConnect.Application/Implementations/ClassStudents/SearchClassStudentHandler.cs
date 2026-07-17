using LinqKit;
using MayNghien.Infrastructures.Models.Requests;
using MayNghien.Infrastructures.Models.Responses;
using Microsoft.EntityFrameworkCore;
using VAcademyConnect.Application.Interfaces;
using VAcademyConnect.Application.Interfaces.ClassStudents;
using VAcademyConnect.Application.Interfaces.Repositories;
using VAcademyConnect.Application.Mapping;
using VAcademyConnect.DTOs.Responses;
using VAcademyConnect.Domain.Entities;
using static MayNghien.Infrastructures.Helpers.SearchHelper;

namespace VAcademyConnect.Application.Implementations.ClassStudents
{
    public class SearchClassStudentHandler : ISearchClassStudentHandler
    {
        private readonly IClassStudentRepository _classStudentRepository;
        private readonly ICurrentUserService _currentUserService;

        public SearchClassStudentHandler(IClassStudentRepository classStudentRepository, ICurrentUserService currentUserService)
        {
            _classStudentRepository = classStudentRepository;
            _currentUserService = currentUserService;
        }

        public async Task<AppResponse<SearchResponse<ClassStudentResponse>>> HandleAsync(SearchRequest request)
        {
            var result = new AppResponse<SearchResponse<ClassStudentResponse>>();
            try
            {
                var user = await _currentUserService.GetCurrentUserAsync();
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

                            case "ClassId":
                                if (Guid.TryParse(filter.Value, out var classId))
                                    predicate = predicate.And(x => x.ClassId == classId);
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
