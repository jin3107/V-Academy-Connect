using Microsoft.Extensions.DependencyInjection;
using VAcademyConnect.Application.Interfaces;
using VAcademyConnect.Application.Interfaces.Repositories;
using VAcademyConnect.Application.Interfaces.Services;
using VAcademyConnect.Infrastructure.Identity;
using VAcademyConnect.Infrastructure.Persistence.Repositories;
using VAcademyConnect.Infrastructure.Services.Implements;

namespace VAcademyConnect.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructureServices(this IServiceCollection services)
        {
            #region Repositories
            services.AddScoped<ITenantRepository, TenantRepository>();
            services.AddScoped<ISubscriptionRepository, SubscriptionRepository>();
            services.AddScoped<ITenantSubscriptionRepository, TenantSubscriptionRepository>();
            services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();
            services.AddScoped<IStaffRepository, StaffRepository>();
            services.AddScoped<IClassRepository, ClassRepository>();
            services.AddScoped<IClassStudentRepository, ClassStudentRepository>();
            #endregion

            #region Identity
            services.AddScoped<ICurrentUserService, CurrentUserService>();
            services.AddScoped<IUserAccountService, UserAccountService>();
            #endregion

            #region Authentication
            services.AddScoped<IAccountService, AccountService>();
            #endregion

            return services;
        }
    }
}
