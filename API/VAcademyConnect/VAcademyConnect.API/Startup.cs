using VAcademyConnect.Repositories.Implements;
using VAcademyConnect.Repositories.Interfaces;
using VAcademyConnect.Services.Implements;
using VAcademyConnect.Services.Interfaces;

namespace VAcademyConnect.API
{
    public class Startup
    {
        public void Mapping(WebApplicationBuilder builder)
        {
            #region Repositories
            builder.Services.AddScoped<ITenantRepository, TenantRepository>();
            builder.Services.AddScoped<ISubscriptionRepository, SubscriptionRepository>();
            builder.Services.AddScoped<ITenantSubscriptionRepository, TenantSubscriptionRepository>();
            builder.Services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();
            builder.Services.AddScoped<IStaffRepository, StaffRepository>();
            builder.Services.AddScoped<IClassRepository, ClassRepository>();
            #endregion

            #region Services
            builder.Services.AddScoped<IAccountService, AccountService>();
            builder.Services.AddScoped<ITenantService, TenantService>();
            builder.Services.AddScoped<ISubscriptionService, SubscriptionService>();
            builder.Services.AddScoped<IStaffService, StaffService>();
            builder.Services.AddScoped<IClassService, ClassService>();
            #endregion
        }
    }
}
