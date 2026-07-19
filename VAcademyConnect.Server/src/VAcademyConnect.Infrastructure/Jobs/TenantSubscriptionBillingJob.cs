using Quartz;
using VAcademyConnect.Application.Interfaces.BackgroundJobs;

namespace VAcademyConnect.Infrastructure.Jobs
{
    // Quartz-specific wrapper: chỉ có nhiệm vụ trigger đúng giờ và gọi vào Application layer.
    // Toàn bộ business logic (tenant nào hết hạn, xử lý ra sao) nằm ở RunTenantSubscriptionBillingJobHandler.
    public class TenantSubscriptionBillingJob : IJob
    {
        private readonly IRunTenantSubscriptionBillingJobHandler _handler;

        public TenantSubscriptionBillingJob(IRunTenantSubscriptionBillingJobHandler handler)
        {
            _handler = handler;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            await _handler.HandleAsync();
        }
    }
}
