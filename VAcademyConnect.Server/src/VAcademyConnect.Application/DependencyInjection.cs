using Microsoft.Extensions.DependencyInjection;
using VAcademyConnect.Application.Implementations.Classes;
using VAcademyConnect.Application.Implementations.ClassStudents;
using VAcademyConnect.Application.Implementations.Staffs;
using VAcademyConnect.Application.Implementations.Subscriptions;
using VAcademyConnect.Application.Implementations.Tenants;
using VAcademyConnect.Application.Interfaces.Classes;
using VAcademyConnect.Application.Interfaces.ClassStudents;
using VAcademyConnect.Application.Interfaces.Staffs;
using VAcademyConnect.Application.Interfaces.Subscriptions;
using VAcademyConnect.Application.Interfaces.Tenants;

namespace VAcademyConnect.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            #region Classes
            services.AddScoped<ICreateClassHandler, CreateClassHandler>();
            services.AddScoped<IUpdateClassHandler, UpdateClassHandler>();
            services.AddScoped<IDeleteClassHandler, DeleteClassHandler>();
            services.AddScoped<IGetClassByIdHandler, GetClassByIdHandler>();
            services.AddScoped<ISearchClassHandler, SearchClassHandler>();
            #endregion

            #region ClassStudents
            services.AddScoped<ICreateClassStudentHandler, CreateClassStudentHandler>();
            services.AddScoped<IUpdateClassStudentHandler, UpdateClassStudentHandler>();
            services.AddScoped<IDeleteClassStudentHandler, DeleteClassStudentHandler>();
            services.AddScoped<IGetClassStudentByIdHandler, GetClassStudentByIdHandler>();
            services.AddScoped<ISearchClassStudentHandler, SearchClassStudentHandler>();
            #endregion

            #region Staffs
            services.AddScoped<ICreateStaffHandler, CreateStaffHandler>();
            services.AddScoped<IUpdateStaffHandler, UpdateStaffHandler>();
            services.AddScoped<IDeleteStaffHandler, DeleteStaffHandler>();
            services.AddScoped<IGetStaffByIdHandler, GetStaffByIdHandler>();
            services.AddScoped<ISearchStaffHandler, SearchStaffHandler>();
            #endregion

            #region Subscriptions
            services.AddScoped<ICreateSubscriptionHandler, CreateSubscriptionHandler>();
            services.AddScoped<IUpdateSubscriptionHandler, UpdateSubscriptionHandler>();
            services.AddScoped<IDeleteSubscriptionHandler, DeleteSubscriptionHandler>();
            services.AddScoped<IGetSubscriptionByIdHandler, GetSubscriptionByIdHandler>();
            services.AddScoped<ISearchSubscriptionHandler, SearchSubscriptionHandler>();
            #endregion

            #region Tenants
            services.AddScoped<ITenantStatusService, TenantStatusService>();
            services.AddScoped<IUpdateTenantHandler, UpdateTenantHandler>();
            services.AddScoped<IGetTenantByIdHandler, GetTenantByIdHandler>();
            services.AddScoped<ISearchTenantHandler, SearchTenantHandler>();
            services.AddScoped<IRenewTenantStandardSubscriptionHandler, RenewTenantStandardSubscriptionHandler>();
            #endregion

            return services;
        }
    }
}
