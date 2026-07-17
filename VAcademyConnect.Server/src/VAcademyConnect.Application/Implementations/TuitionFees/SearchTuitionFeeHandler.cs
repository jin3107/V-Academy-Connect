using LinqKit;
using MayNghien.Infrastructures.Models.Requests;
using MayNghien.Infrastructures.Models.Responses;
using Microsoft.EntityFrameworkCore;
using VAcademyConnect.Application.Interfaces;
using VAcademyConnect.Application.Interfaces.Repositories;
using VAcademyConnect.Application.Interfaces.TuitionFees;
using VAcademyConnect.Application.Mapping;
using VAcademyConnect.DTOs.Responses;
using VAcademyConnect.Domain.Entities;
using static MayNghien.Infrastructures.Helpers.SearchHelper;

namespace VAcademyConnect.Application.Implementations.TuitionFees
{
    public class SearchTuitionFeeHandler : ISearchTuitionFeeHandler
    {
        private readonly ITuitionFeeRepository _tuitionFeeRepository;
        private readonly ICurrentUserService _currentUserService;

        public SearchTuitionFeeHandler(ITuitionFeeRepository tuitionFeeRepository, ICurrentUserService currentUserService)
        {
            _tuitionFeeRepository = tuitionFeeRepository;
            _currentUserService = currentUserService;
        }

        public async Task<AppResponse<SearchResponse<TuitionFeeResponse>>> HandleAsync(SearchRequest request)
        {
            var result = new AppResponse<SearchResponse<TuitionFeeResponse>>();
            try
            {
                var user = await _currentUserService.GetCurrentUserAsync();
                if (user == null)
                    return result.BuildError("Unauthorize");

                var query = BuildFilterExpression(request.Filters!);
                var numOfRecords = await _tuitionFeeRepository.CountRecordsAsync(query);
                var tuitionFees = _tuitionFeeRepository.FindByPredicate(query)
                    .Include(x => x.Student).AsQueryable();

                if (request.SortBy != null)
                    tuitionFees = _tuitionFeeRepository.AddSort(tuitionFees, request.SortBy);
                else
                    tuitionFees = tuitionFees.OrderBy(x => x.DueDate);

                int pageIndex = request.PageIndex ?? 1;
                int pageSize = request.PageSize ?? 10;
                int startIndex = (pageIndex - 1) * pageSize;
                var tuitionFeeList = await tuitionFees.Skip(startIndex).Take(pageSize).ToListAsync();
                var dtoList = tuitionFeeList.Select(TuitionFeeMapper.ToResponse).ToList();

                var searchResponse = new SearchResponse<TuitionFeeResponse>
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

        private ExpressionStarter<TuitionFee> BuildFilterExpression(List<Filter> filters)
        {
            try
            {
                var predicate = PredicateBuilder.New<TuitionFee>(true);
                if (filters != null)
                {
                    foreach (var filter in filters)
                    {
                        switch (filter.FieldName)
                        {
                            case "StudentId":
                                if (!string.IsNullOrEmpty(filter.Value))
                                    predicate = predicate.And(x => x.StudentId == filter.Value);
                                break;

                            case "Month":
                                if (!string.IsNullOrEmpty(filter.Value))
                                    predicate = predicate.And(x => x.Month == filter.Value);
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
