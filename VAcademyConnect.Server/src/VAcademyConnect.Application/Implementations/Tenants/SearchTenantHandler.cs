using LinqKit;
using MayNghien.Infrastructures.Models.Requests;
using MayNghien.Infrastructures.Models.Responses;
using Microsoft.EntityFrameworkCore;
using VAcademyConnect.Application.Interfaces;
using VAcademyConnect.Application.Interfaces.Repositories;
using VAcademyConnect.Application.Interfaces.Tenants;
using VAcademyConnect.Application.Mapping;
using VAcademyConnect.DTOs.Responses;
using VAcademyConnect.Domain.Entities;
using static MayNghien.Infrastructures.Helpers.SearchHelper;

namespace VAcademyConnect.Application.Implementations.Tenants
{
    public class SearchTenantHandler : ISearchTenantHandler
    {
        private readonly ITenantRepository _tenantRepository;
        private readonly ICurrentUserService _currentUserService;
        private readonly ITenantStatusService _tenantStatusService;

        public SearchTenantHandler(ITenantRepository tenantRepository, ICurrentUserService currentUserService,
            ITenantStatusService tenantStatusService)
        {
            _tenantRepository = tenantRepository;
            _currentUserService = currentUserService;
            _tenantStatusService = tenantStatusService;
        }

        public async Task<AppResponse<SearchResponse<TenantResponse>>> HandleAsync(SearchRequest request)
        {
            var result = new AppResponse<SearchResponse<TenantResponse>>();
            try
            {
                var user = await _currentUserService.GetCurrentUserAsync();
                if (user == null)
                    return result.BuildError("Unauthorize");

                var query = BuildFilterExpression(request.Filters!);
                var numOfRecords = await _tenantRepository.CountRecordsAsync(query);
                var tenants = _tenantRepository.FindByPredicate(query)
                    .Include(t => t.TenantSubscription)
                        .ThenInclude(ts => ts!.Subscription)
                    .AsQueryable();

                if (request.SortBy != null)
                    tenants = _tenantRepository.AddSort(tenants, request.SortBy);
                else
                    tenants = tenants.OrderBy(x => x.Name);

                int pageIndex = request.PageIndex ?? 1;
                int pageSize = request.PageSize ?? 1;
                int startIndex = (pageIndex - 1) * pageSize;
                var tenantList = await tenants.Skip(startIndex).Take(pageSize).ToListAsync();

                var changedTenantIds = new List<Guid>();
                foreach (var tenant in tenantList)
                {
                    var statusChanged = await _tenantStatusService.CheckAndUpdateExpiredSubscriptionAsync(tenant);
                    if (statusChanged)
                        changedTenantIds.Add(tenant.Id);
                }

                if (changedTenantIds.Any())
                {
                    for (int i = 0; i < tenantList.Count; i++)
                    {
                        if (changedTenantIds.Contains(tenantList[i].Id))
                        {
                            tenantList[i] = await _tenantRepository.FindByAsync(t => t.Id == tenantList[i].Id)
                                .Include(t => t.TenantSubscription).ThenInclude(ts => ts!.Subscription)
                                .FirstOrDefaultAsync() ?? tenantList[i];
                        }
                    }
                }

                var dtoList = tenantList.Select(TenantMapper.ToResponse).ToList();
                var searchResponse = new SearchResponse<TenantResponse>
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

        private ExpressionStarter<Tenant> BuildFilterExpression(List<Filter> filters)
        {
            try
            {
                var predicate = PredicateBuilder.New<Tenant>(true);
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

                            case "Status":
                                if (!string.IsNullOrEmpty(filter.Value))
                                    predicate = predicate.And(x => x.Status.ToString().Contains(filter.Value));
                                break;

                            case "Subscription":
                                {
                                    var key = (filter.Value ?? "").Trim();
                                    if (!string.IsNullOrEmpty(key))
                                    {
                                        predicate = predicate.And(x =>
                                            x.TenantSubscription != null &&
                                            x.TenantSubscription.Subscription != null &&
                                            EF.Functions.Like(x.TenantSubscription.Subscription.Name, $"%{key}%"));
                                    }
                                    break;
                                }

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
