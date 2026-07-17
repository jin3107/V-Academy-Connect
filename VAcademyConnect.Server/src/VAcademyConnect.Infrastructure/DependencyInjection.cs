using Microsoft.Extensions.DependencyInjection;
using VAcademyConnect.Application.Interfaces;
using VAcademyConnect.Application.Interfaces.Authentication;
using VAcademyConnect.Application.Interfaces.Repositories;
using VAcademyConnect.Infrastructure.Email;
using VAcademyConnect.Infrastructure.Identity;
using VAcademyConnect.Infrastructure.Persistence.Repositories;
using VAcademyConnect.Infrastructure.Services.Implements.Authentication;

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
            services.AddScoped<IOtpCodeRepository, OtpCodeRepository>();
            services.AddScoped<IAttendanceRepository, AttendanceRepository>();
            services.AddScoped<ILessonEvaluationRepository, LessonEvaluationRepository>();
            #endregion

            #region Identity
            services.AddScoped<ICurrentUserService, CurrentUserService>();
            services.AddScoped<IUserAccountService, UserAccountService>();
            services.AddScoped<ITokenService, TokenService>();
            #endregion

            #region Email
            services.AddScoped<IEmailService, EmailService>();
            #endregion

            #region Authentication
            services.AddScoped<ILoginHandler, LoginHandler>();
            services.AddScoped<IRegisterHandler, RegisterHandler>();
            services.AddScoped<ILogoutHandler, LogoutHandler>();
            services.AddScoped<IRefreshTokenHandler, RefreshTokenHandler>();
            services.AddScoped<IChangePasswordHandler, ChangePasswordHandler>();
            services.AddScoped<ISendOtpHandler, SendOtpHandler>();
            services.AddScoped<IVerifyOtpHandler, VerifyOtpHandler>();
            services.AddScoped<ILoginByGoogleHandler, LoginByGoogleHandler>();
            services.AddScoped<IGetRolesHandler, GetRolesHandler>();
            #endregion

            return services;
        }
    }
}
