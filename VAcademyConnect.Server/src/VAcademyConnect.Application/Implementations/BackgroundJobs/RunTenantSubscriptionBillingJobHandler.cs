using MayNghien.Infrastructures.Models.Responses;
using Microsoft.EntityFrameworkCore;
using VAcademyConnect.Application.Interfaces.BackgroundJobs;
using VAcademyConnect.Application.Interfaces.Repositories;
using VAcademyConnect.Application.Interfaces.Tenants;
using VAcademyConnect.Domain.Entities;

namespace VAcademyConnect.Application.Implementations.BackgroundJobs
{
    // Chạy hằng ngày (Quartz): quét mọi tenant có TenantSubscription đã hết hạn (EndDate < now)
    // và áp dụng logic hạ xuống gói Free hiện có (TenantStatusService), thay vì chỉ chạy bị động
    // khi có người gọi GetTenantById/SearchTenant. Mỗi tenant bị xử lý được ghi 1 dòng BackgroundJob.
    public class RunTenantSubscriptionBillingJobHandler : IRunTenantSubscriptionBillingJobHandler
    {
        private readonly ITenantRepository _tenantRepository;
        private readonly ITenantStatusService _tenantStatusService;
        private readonly IBackgroundJobRepository _backgroundJobRepository;

        public RunTenantSubscriptionBillingJobHandler(ITenantRepository tenantRepository,
            ITenantStatusService tenantStatusService, IBackgroundJobRepository backgroundJobRepository)
        {
            _tenantRepository = tenantRepository;
            _tenantStatusService = tenantStatusService;
            _backgroundJobRepository = backgroundJobRepository;
        }

        public async Task<AppResponse<string>> HandleAsync()
        {
            var result = new AppResponse<string>();
            try
            {
                var runDate = DateTime.UtcNow;

                var expiredTenants = await _tenantRepository.FindByAsync(t => !t.IsDeleted &&
                        t.TenantSubscription != null && t.TenantSubscription.EndDate != null &&
                        t.TenantSubscription.EndDate < runDate)
                    .Include(t => t.TenantSubscription)
                    .ToListAsync();

                var processedCount = 0;
                foreach (var tenant in expiredTenants)
                {
                    var jobRecord = new BackgroundJob
                    {
                        Id = Guid.NewGuid(),
                        RunDate = runDate,
                        TenantId = tenant.Id,
                        CreatedBy = "System",
                        CreatedOn = DateTime.UtcNow,
                    };

                    try
                    {
                        var statusChanged = await _tenantStatusService.CheckAndUpdateExpiredSubscriptionAsync(tenant);
                        jobRecord.Result = statusChanged ? "Downgraded to Free" : "No change";
                        jobRecord.LogDetail = $"Subscription end date was {tenant.TenantSubscription?.EndDate:yyyy-MM-dd}.";
                        processedCount++;
                    }
                    catch (Exception ex)
                    {
                        jobRecord.Result = "Error";
                        jobRecord.LogDetail = ex.Message;
                    }

                    await _backgroundJobRepository.AddAsync(jobRecord);
                }

                result.BuildResult($"Processed {processedCount}/{expiredTenants.Count} expired tenant subscription(s).");
            }
            catch (Exception ex)
            {
                result.BuildError(ex.Message + " " + ex.StackTrace);
            }
            return result;
        }
    }
}
