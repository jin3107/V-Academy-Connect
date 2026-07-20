using Quartz;
using VAcademyConnect.Application.Interfaces.BackgroundJobs;

namespace VAcademyConnect.Infrastructure.Jobs
{
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
