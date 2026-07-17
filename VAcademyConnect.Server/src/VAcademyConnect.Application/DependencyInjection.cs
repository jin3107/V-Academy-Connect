using Microsoft.Extensions.DependencyInjection;
using VAcademyConnect.Application.Interfaces.Services;
using VAcademyConnect.Application.Services.Implements;

namespace VAcademyConnect.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            services.AddScoped<IClassService, ClassService>();
            services.AddScoped<IClassStudentService, ClassStudentService>();
            services.AddScoped<IStaffService, StaffService>();
            services.AddScoped<ISubscriptionService, SubscriptionService>();
            services.AddScoped<ITenantService, TenantService>();

            return services;
        }
    }
}
