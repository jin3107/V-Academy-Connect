using LinqKit;
using MayNghien.Infrastructures.Models.Requests;
using MayNghien.Infrastructures.Models.Responses;
using Microsoft.EntityFrameworkCore;
using VAcademyConnect.Application.Interfaces;
using VAcademyConnect.Application.Interfaces.Attendances;
using VAcademyConnect.Application.Interfaces.Repositories;
using VAcademyConnect.Application.Mapping;
using VAcademyConnect.DTOs.Responses;
using VAcademyConnect.Domain.Entities;
using static MayNghien.Infrastructures.Helpers.SearchHelper;

namespace VAcademyConnect.Application.Implementations.Attendances
{
    public class SearchAttendanceHandler : ISearchAttendanceHandler
    {
        private readonly IAttendanceRepository _attendanceRepository;
        private readonly ICurrentUserService _currentUserService;

        public SearchAttendanceHandler(IAttendanceRepository attendanceRepository, ICurrentUserService currentUserService)
        {
            _attendanceRepository = attendanceRepository;
            _currentUserService = currentUserService;
        }

        public async Task<AppResponse<SearchResponse<AttendanceResponse>>> HandleAsync(SearchRequest request)
        {
            var result = new AppResponse<SearchResponse<AttendanceResponse>>();
            try
            {
                var user = await _currentUserService.GetCurrentUserAsync();
                if (user == null)
                    return result.BuildError("Unauthorize");

                var query = BuildFilterExpression(request.Filters!);
                var numOfRecords = await _attendanceRepository.CountRecordsAsync(query);
                var attendances = _attendanceRepository.FindByPredicate(query).AsQueryable();

                if (request.SortBy != null)
                    attendances = _attendanceRepository.AddSort(attendances, request.SortBy);
                else
                    attendances = attendances.OrderByDescending(x => x.Date);

                int pageIndex = request.PageIndex ?? 1;
                int pageSize = request.PageSize ?? 10;
                int startIndex = (pageIndex - 1) * pageSize;
                var attendanceList = await attendances.Skip(startIndex).Take(pageSize).ToListAsync();
                var dtoList = attendanceList.Select(AttendanceMapper.ToResponse).ToList();

                var searchResponse = new SearchResponse<AttendanceResponse>
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

        private ExpressionStarter<Attendance> BuildFilterExpression(List<Filter> filters)
        {
            try
            {
                var predicate = PredicateBuilder.New<Attendance>(true);
                if (filters != null)
                {
                    foreach (var filter in filters)
                    {
                        switch (filter.FieldName)
                        {
                            case "ClassId":
                                if (Guid.TryParse(filter.Value, out var classId))
                                    predicate = predicate.And(x => x.ClassId == classId);
                                break;

                            case "StudentId":
                                if (!string.IsNullOrEmpty(filter.Value))
                                    predicate = predicate.And(x => x.StudentId == filter.Value);
                                break;

                            case "Date":
                                if (DateTime.TryParse(filter.Value, out var date))
                                    predicate = predicate.And(x => x.Date != null && x.Date.Value.Date == date.Date);
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
