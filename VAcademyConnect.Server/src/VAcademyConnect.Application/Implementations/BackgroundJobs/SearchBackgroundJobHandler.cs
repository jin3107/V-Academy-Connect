using LinqKit;
using MayNghien.Infrastructures.Models.Requests;
using MayNghien.Infrastructures.Models.Responses;
using Microsoft.EntityFrameworkCore;
using VAcademyConnect.Application.Interfaces;
using VAcademyConnect.Application.Interfaces.BackgroundJobs;
using VAcademyConnect.Application.Interfaces.Repositories;
using VAcademyConnect.Application.Mapping;
using VAcademyConnect.DTOs.Responses;
using VAcademyConnect.Domain.Entities;
using static MayNghien.Infrastructures.Helpers.SearchHelper;

namespace VAcademyConnect.Application.Implementations.BackgroundJobs
{
    public class SearchBackgroundJobHandler : ISearchBackgroundJobHandler
    {
        private readonly IBackgroundJobRepository _backgroundJobRepository;
        private readonly ICurrentUserService _currentUserService;

        public SearchBackgroundJobHandler(IBackgroundJobRepository backgroundJobRepository, ICurrentUserService currentUserService)
        {
            _backgroundJobRepository = backgroundJobRepository;
            _currentUserService = currentUserService;
        }

        public async Task<AppResponse<SearchResponse<BackgroundJobResponse>>> HandleAsync(SearchRequest request)
        {
            var result = new AppResponse<SearchResponse<BackgroundJobResponse>>();
            try
            {
                var user = await _currentUserService.GetCurrentUserAsync();
                if (user == null)
                    return result.BuildError("Unauthorize");

                var query = BuildFilterExpression(request.Filters!);
                var numOfRecords = await _backgroundJobRepository.CountRecordsAsync(query);
                var backgroundJobs = _backgroundJobRepository.FindByPredicate(query)
                    .Include(x => x.Tenant).AsQueryable();

                if (request.SortBy != null)
                    backgroundJobs = _backgroundJobRepository.AddSort(backgroundJobs, request.SortBy);
                else
                    backgroundJobs = backgroundJobs.OrderByDescending(x => x.RunDate);

                int pageIndex = request.PageIndex ?? 1;
                int pageSize = request.PageSize ?? 10;
                int startIndex = (pageIndex - 1) * pageSize;
                var backgroundJobList = await backgroundJobs.Skip(startIndex).Take(pageSize).ToListAsync();
                var dtoList = backgroundJobList.Select(BackgroundJobMapper.ToResponse).ToList();

                var searchResponse = new SearchResponse<BackgroundJobResponse>
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

        private ExpressionStarter<BackgroundJob> BuildFilterExpression(List<Filter> filters)
        {
            try
            {
                var predicate = PredicateBuilder.New<BackgroundJob>(true);
                if (filters != null)
                {
                    foreach (var filter in filters)
                    {
                        switch (filter.FieldName)
                        {
                            case "TenantId":
                                if (!string.IsNullOrEmpty(filter.Value) && Guid.TryParse(filter.Value, out var tenantId))
                                    predicate = predicate.And(x => x.TenantId == tenantId);
                                break;

                            case "Result":
                                if (!string.IsNullOrEmpty(filter.Value))
                                    predicate = predicate.And(x => x.Result == filter.Value);
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
