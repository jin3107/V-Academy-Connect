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
using VAcademyConnect.Commons.Enums;
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

        public async Task<AppResponse<TenantResponse>> GetByIdAsync(Guid id)
        {
            var result = new AppResponse<TenantResponse>();
            try
            {
                var user = await _userManager.FindByEmailAsync(_contextAccessor.HttpContext?.User.Identity?.Name!);
                if (user == null)
                    return result.BuildError("Unauthorize.");

                var tenant = await _tenantRepository.FindByAsync(sub => sub.Id == id)
                    .Include(t => t.TenantSubscription)
                    .ThenInclude(ts => ts!.Subscription)
                    .FirstOrDefaultAsync();

                if (tenant == null || tenant.IsDeleted == true)
                    return result.BuildError($"Tenant with ID {id} was not found or has been deleted.");

                // Kiểm tra và cập nhật status nếu subscription hết hạn
                var statusChanged = await CheckAndUpdateTenantStatus(tenant);

                // Nếu có thay đổi, reload tenant để lấy subscription mới
                if (statusChanged)
                {
                    tenant = await _tenantRepository.FindByAsync(sub => sub.Id == id)
                        .Include(t => t.TenantSubscription)
                            .ThenInclude(ts => ts!.Subscription)
                        .FirstOrDefaultAsync();
                }

                var response = TenantMapper.ToResponse(tenant!);
                var message = $"Tenant with ID {id} has been found.";
                if (statusChanged)
                    message += " Tenant has been switched to Free plan due to expired subscription.";
                result.BuildResult(response, message);
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

                // Kiểm tra và cập nhật status cho mỗi tenant
                var changedTenantIds = new List<Guid>();
                foreach (var tenant in tenantList)
                {
                    var statusChanged = await CheckAndUpdateTenantStatus(tenant);
                    if (statusChanged)
                        changedTenantIds.Add(tenant.Id);
                }

                // Reload những tenant đã thay đổi
                if (changedTenantIds.Any())
                {
                    for (int i = 0; i < tenantList.Count; i++)
                    {
                        if (changedTenantIds.Contains(tenantList[i].Id))
                        {
                            tenantList[i] = await _tenantRepository.FindByAsync(t => t.Id == tenantList[i].Id)
                                .Include(t => t.TenantSubscription)
                                .ThenInclude(ts => ts!.Subscription)
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

        public async Task<AppResponse<TenantResponse>> UpdateAsync(TenantRequest request)
        {
            var result = new AppResponse<TenantResponse>();
            try
            {
                var user = await _userManager.FindByEmailAsync(_contextAccessor.HttpContext?.User.Identity?.Name!);
                if (user == null)
                    return result.BuildError("Unauthorize");

                var tenant = await _tenantRepository.FindByAsync(t => t.Id == request.Id)
                    .Include(ts => ts.TenantSubscription).ThenInclude(t => t!.Subscription).FirstOrDefaultAsync();
                //var tenant = await _tenantRepository.GetAsync(request.Id!.Value); // chưa include subscription và tenantsubscription

                if (tenant == null || tenant.IsDeleted == true)
                    return result.BuildError($"Tenant with ID {request.Id} was not found or has been deleted.");

                // Kiểm tra quyền thay đổi status
                if (!await ValidateStatusChange(tenant, request.Status))
                {
                    return result.BuildError("You do not have permission to change tenant status from Inactive to Active. Only administrators can perform this action.");
                }

                // Nếu đang chuyển từ Inactive sang Active, kiểm tra subscription
                if (tenant.Status == Status.Inactive && request.Status == Status.Active)
                {
                    if (tenant.TenantSubscription == null || tenant.TenantSubscription.EndDate <= DateTime.UtcNow)
                    {
                        return result.BuildError("Cannot activate tenant: No active subscription found or subscription has expired. Please renew the subscription first.");
                    }
                }

                tenant.Name = request.Name;
                tenant.ContactEmail = request.ContactEmail;
                tenant.ContactPhone = request.ContactPhone;
                tenant.Status = request.Status;
                tenant.ModifiedBy = user.Email;
                tenant.ModifiedOn = DateTime.UtcNow;

                await _tenantRepository.EditAsync(tenant);
                var updatedResponse = TenantMapper.ToResponse(tenant);
                
                string message = $"Tenant has been updated successfully.";
                if (tenant.Status == Status.Active && tenant.TenantSubscription?.EndDate != null)
                {
                    message += $" Subscription is valid until {tenant.TenantSubscription.EndDate.Value:yyyy-MM-dd}.";
                }
                
                result.BuildResult(updatedResponse, message);
            }
            catch (Exception ex)
            {
                result.BuildError(ex.Message + " " + ex.StackTrace);
            }
            return result;
        }

        //private async Task<string?> ValidateSubscriptionStatus(Tenant tenant)
        //{
        //    // Check if status is Inactive
        //    if (tenant.Status == Status.Inactive)
        //        return "Cannot set tenant status to Inactive. Please subscribe to a plan to activate the tenant.";

        //    if (tenant.TenantSubscription == null)
        //        return "No active subscription found. Please subscribe to a plan to update tenant information.";

        //    // If subscription end date is within 30 days or has passed
        //    if (tenant.TenantSubscription.EndDate.HasValue)
        //    {
        //        var now = DateTime.UtcNow;
        //        if (tenant.TenantSubscription.EndDate.Value <= now)
        //            return "Your subscription has expired. Please renew your subscription to continue using the service.";
        //        if (tenant.TenantSubscription.EndDate.Value <= now.AddDays(30))
        //            return $"Your subscription will expire on " +
        //                $"{tenant.TenantSubscription.EndDate.Value:yyyy-MM-dd}. " +
        //                $"Please renew your subscription to avoid service interruption.";
        //    }

        //    return null;
        //}

        private async Task<bool> CheckAndUpdateTenantStatus(Tenant tenant, bool saveChanges = true)
        {
            try
            {
                bool statusChanged = false;

                if (tenant.TenantSubscription?.EndDate < DateTime.UtcNow && tenant.TenantSubscription?.EndDate != null)
                {
                    // Subscription hết hạn - chuyển về Free plan
                    var freePlan = await _subscriptionRepository.FindByAsync(s => s.Name == "Free" && !s.IsDeleted)
                        .FirstOrDefaultAsync();

                    if (freePlan != null)
                    {
                        var oldSubscription = await _tenantSubscriptionRepository.GetAsync(tenant.TenantSubscription.Id);
                        if (oldSubscription != null)
                        {
                            // Cập nhật subscription hiện tại thành Free plan
                            oldSubscription.SubscriptionId = freePlan.Id;
                            oldSubscription.Subscription = freePlan;
                            oldSubscription.IsTrial = false;
                            oldSubscription.StartDate = DateTime.UtcNow;
                            oldSubscription.EndDate = null; // Free plan không có hạn
                            oldSubscription.ModifiedOn = DateTime.UtcNow;
                            oldSubscription.ModifiedBy = _contextAccessor.HttpContext?.User.Identity?.Name ?? "System";

                            await _tenantSubscriptionRepository.EditAsync(oldSubscription);
                        }

                        // Chuyển tenant về Active (Free plan vẫn được dùng)
                        tenant.Status = Status.Active;
                        tenant.ModifiedOn = DateTime.UtcNow;
                        tenant.ModifiedBy = "System - Switched to Free Plan";
                        statusChanged = true;

                        if (saveChanges)
                        {
                            await _tenantRepository.EditAsync(tenant);
                        }
                    }
                }
                return statusChanged;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message + " " + ex.StackTrace);
            }
            return false;
        }

        private async Task<bool> ValidateStatusChange(Tenant tenant, Status newStatus)
        {
            var user = await _userManager.FindByEmailAsync(_contextAccessor.HttpContext?.User.Identity?.Name!);
            if (user == null)
                return false;

            // Chỉ cho phép admin thay đổi status từ Inactive sang Active
            if (tenant.Status == Status.Inactive && newStatus == Status.Active)
            {
                var roles = await _userManager.GetRolesAsync(user);
                return roles.Any(r => r == "SuperAdmin" || r == "Admin");
            }

            return true;
        }

        // Gia hạn subscription Standard cho tenant (chỉ admin mới được gọi)
        public async Task<AppResponse<TenantResponse>> RenewStandardSubscriptionAsync(Guid tenantId)
        {
            var result = new AppResponse<TenantResponse>();
            try
            {
                var user = await _userManager.FindByEmailAsync(_contextAccessor.HttpContext?.User.Identity?.Name!);
                if (user == null)
                    return result.BuildError("Unauthorized");

                var roles = await _userManager.GetRolesAsync(user);
                if (!roles.Any(r => r == "SuperAdmin" || r == "Admin"))
                    return result.BuildError("Only administrators can renew subscriptions.");

                var standardPlan = await _subscriptionRepository
                    .FindByAsync(s => s.Name == "Standard" && !s.IsDeleted)
                    .FirstOrDefaultAsync();

                if (standardPlan == null)
                    return result.BuildError("Standard subscription plan not found.");

                var currentTenantSubscription = await _tenantSubscriptionRepository
                    .FindByAsync(s => s.TenantId == tenantId && !s.IsDeleted)
                    .FirstOrDefaultAsync();

                if (currentTenantSubscription == null)
                    return result.BuildError("No active subscription found for this tenant.");

                currentTenantSubscription.SubscriptionId = standardPlan.Id;
                currentTenantSubscription.StartDate = DateTime.UtcNow;
                currentTenantSubscription.EndDate = DateTime.UtcNow.AddMonths(1);
                currentTenantSubscription.IsTrial = false;
                currentTenantSubscription.ModifiedOn = DateTime.UtcNow;
                currentTenantSubscription.ModifiedBy = user.Email;
                await _tenantSubscriptionRepository.EditAsync(currentTenantSubscription);

                var tenant = await _tenantRepository.FindByAsync(t => t.Id == tenantId)
                    .FirstOrDefaultAsync();

                if (tenant != null)
                {
                    tenant.Status = Status.Active;
                    tenant.ModifiedBy = user.Email;
                    tenant.ModifiedOn = DateTime.UtcNow;
                    await _tenantRepository.EditAsync(tenant);
                }

                tenant = await _tenantRepository.FindByAsync(t => t.Id == tenantId)
                    .Include(t => t.TenantSubscription)
                    .ThenInclude(ts => ts!.Subscription)
                    .FirstOrDefaultAsync();

                var response = TenantMapper.ToResponse(tenant!);
                result.BuildResult(response, $"Subscription renewed successfully for 1 month(s). Tenant is now active.");
            }
            catch (Exception ex)
            {
                result.BuildError(ex.Message + " " + ex.StackTrace);
            }
            return result;
        }
    }
}
