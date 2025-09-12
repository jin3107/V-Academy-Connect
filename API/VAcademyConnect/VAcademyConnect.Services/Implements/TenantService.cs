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
using VAcademyConnect.Repositories.Implements;
using VAcademyConnect.Repositories.Interfaces;
using VAcademyConnect.Services.Interfaces;
using VAcademyConnect.Services.Mapping;
using static MayNghien.Infrastructures.Helpers.SearchHelper;

namespace VAcademyConnect.Services.Implements
{
    public class TenantService : ITenantService
    {
        private readonly ITenantRepository _tenantRepository;
        private readonly ISubscriptionRepository _subscriptionRepository;
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly ITenantSubscriptionRepository _tenantSubscriptionRepository;
        private readonly UserManager<ApplicationUser> _userManager;

        public TenantService(ITenantRepository tenantRepository,
            IHttpContextAccessor contextAccessor,
            UserManager<ApplicationUser> userManager,
            ITenantSubscriptionRepository tenantSubscriptionRepository,
            ISubscriptionRepository subscriptionRepository)
        {
            _tenantRepository = tenantRepository;
            _contextAccessor = contextAccessor;
            _userManager = userManager;
            _tenantSubscriptionRepository = tenantSubscriptionRepository;
            _subscriptionRepository = subscriptionRepository;
        }

        public async Task<AppResponse<string>> DeleteAsync(Guid id)
        {
            var result = new AppResponse<string>();
            try
            {
                var user = await _userManager.FindByEmailAsync(_contextAccessor.HttpContext?.User.Identity?.Name!);
                if (user == null)
                    return result.BuildError("Unauthorize");

                var tenant = await _tenantRepository.GetAsync(id);
                if (tenant == null || tenant.IsDeleted == true)
                    return result.BuildError($"Tenant with ID {id} was not found or has been deleted.");
                tenant.IsDeleted = true;
                tenant.ModifiedBy = user.Email;

                await _tenantRepository.EditAsync(tenant);
                result.BuildResult($"Tenant with ID {id} has been deleted successfully.");
            }
            catch (Exception ex)
            {
                result.BuildError(ex.Message + " " + ex.StackTrace);
            }
            return result;
        }

        public async Task<AppResponse<TenantResponse>> GetByIdAsync(Guid id)
        {
            var result = new AppResponse<TenantResponse>();
            try
            {
                var user = await _userManager.FindByEmailAsync(_contextAccessor.HttpContext?.User.Identity?.Name!);
                if (user == null)
                    return result.BuildError("Unauthorize.");

                var tenant = await _tenantRepository.FindByAsync(sub => sub.Id == id).FirstOrDefaultAsync();
                if (tenant == null || tenant.IsDeleted == true)
                    return result.BuildError($"Tenant with ID {id} was not found or has been deleted.");

                var response = TenantMapper.ToResponse(tenant);
                result.BuildResult(response, $"Tenant with ID {id} has been found.");
            }
            catch (Exception ex)
            {
                result.BuildError(ex.Message + " " + ex.StackTrace);
            }
            return result;
        }

        public async Task<AppResponse<SearchResponse<TenantResponse>>> SearchAsync(SearchRequest request)
        {
            var result = new AppResponse<SearchResponse<TenantResponse>>();
            try
            {
                var user = await _userManager.FindByEmailAsync(_contextAccessor.HttpContext?.User.Identity?.Name!);
                if (user == null)
                    return result.BuildError("Unauthorize");

                var query = BuildFilterExpression(request.Filters!);
                var numOfRecords = await _tenantRepository.CountRecordsAsync(query);
                var tenants = _tenantRepository.FindByPredicate(query).AsQueryable();
                if (request.SortBy != null)
                    tenants = _tenantRepository.AddSort(tenants, request.SortBy);
                else
                    tenants = tenants.OrderBy(x => x.Name);

                int pageIndex = request.PageIndex ?? 1;
                int pageSize = request.PageSize ?? 1;
                int startIndex = (pageIndex - 1) * pageSize;
                var tenantfList = await tenants.Skip(startIndex).Take(pageSize).ToListAsync();
                var dtoList = tenantfList.Select(TenantMapper.ToResponse).ToList();
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

        public async Task<AppResponse<TenantResponse>> UpdateAsync(TenantRequest request)
        {
            var result = new AppResponse<TenantResponse>();
            try
            {
                var user = await _userManager.FindByEmailAsync(_contextAccessor.HttpContext?.User.Identity?.Name!);
                if (user == null)
                    return result.BuildError("Unauthorize");

                var tenant = await _tenantRepository.GetAsync(request.Id!.Value);
                if (tenant == null || tenant.IsDeleted == true)
                    return result.BuildError($"Tenant with ID {request.Id!.Value} was not found or has been deleted.");

                tenant.Name = request.Name;
                tenant.ContactEmail = request.ContactEmail;
                tenant.ContactPhone = request.ContactPhone;
                tenant.Status = request.Status;
                tenant.ModifiedBy = user.Email;
                tenant.ModifiedOn = DateTime.UtcNow;

                await _tenantRepository.EditAsync(tenant);
                var response = TenantMapper.ToResponse(tenant);
                result.BuildResult(response, $"Tenant with ID {request.Id!.Value} has been updated successfully.");
            }
            catch (Exception ex)
            {
                result.BuildError(ex.Message + " " + ex.StackTrace);
            }
            return result;
        }

        public async Task<AppResponse<TenantResponse>> CreateAsync(TenantRequest request)
        {
            var result = new AppResponse<TenantResponse>();
            try
            {
                var user = await _userManager.FindByEmailAsync(_contextAccessor.HttpContext?.User.Identity?.Name!);
                if (user == null)
                    return result.BuildError("Unauthorize.");

                var newTenant = TenantMapper.ToEntity(request);
                newTenant.Id = Guid.NewGuid();
                newTenant.Name = request.Name;
                newTenant.ContactEmail = request.ContactEmail;
                newTenant.ContactPhone = request.ContactPhone;
                newTenant.Status = request.Status;
                newTenant.CreatedOn = DateTime.UtcNow;
                newTenant.CreatedBy = user.Email;

                await _tenantRepository.AddAsync(newTenant);
                var response = TenantMapper.ToResponse(newTenant);
                result.BuildResult(response, "Tenant has been created successfully.");
            }
            catch (Exception ex)
            {
                result.BuildError(ex.Message + " " + ex.StackTrace);
            }
            return result;
        }
    }
}
