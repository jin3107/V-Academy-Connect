using Microsoft.Extensions.DependencyInjection;
using VAcademyConnect.Application.Implementations.Attendances;
using VAcademyConnect.Application.Implementations.BackgroundJobs;
using VAcademyConnect.Application.Implementations.Classes;
using VAcademyConnect.Application.Implementations.ClassStudents;
using VAcademyConnect.Application.Implementations.LessonEvaluations;
using VAcademyConnect.Application.Implementations.PaymentChecks;
using VAcademyConnect.Application.Implementations.Payments;
using VAcademyConnect.Application.Implementations.Scores;
using VAcademyConnect.Application.Implementations.ScoreStatistics;
using VAcademyConnect.Application.Implementations.Staffs;
using VAcademyConnect.Application.Implementations.Students;
using VAcademyConnect.Application.Implementations.Subscriptions;
using VAcademyConnect.Application.Implementations.Tenants;
using VAcademyConnect.Application.Implementations.TuitionFees;
using VAcademyConnect.Application.Interfaces.Attendances;
using VAcademyConnect.Application.Interfaces.BackgroundJobs;
using VAcademyConnect.Application.Interfaces.Classes;
using VAcademyConnect.Application.Interfaces.ClassStudents;
using VAcademyConnect.Application.Interfaces.LessonEvaluations;
using VAcademyConnect.Application.Interfaces.PaymentChecks;
using VAcademyConnect.Application.Interfaces.Payments;
using VAcademyConnect.Application.Interfaces.Scores;
using VAcademyConnect.Application.Interfaces.ScoreStatistics;
using VAcademyConnect.Application.Interfaces.Staffs;
using VAcademyConnect.Application.Interfaces.Students;
using VAcademyConnect.Application.Interfaces.Subscriptions;
using VAcademyConnect.Application.Interfaces.Tenants;
using VAcademyConnect.Application.Interfaces.TuitionFees;

namespace VAcademyConnect.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            #region Attendances
            services.AddScoped<ICreateAttendanceHandler, CreateAttendanceHandler>();
            services.AddScoped<IUpdateAttendanceHandler, UpdateAttendanceHandler>();
            services.AddScoped<IDeleteAttendanceHandler, DeleteAttendanceHandler>();
            services.AddScoped<IGetAttendanceByIdHandler, GetAttendanceByIdHandler>();
            services.AddScoped<ISearchAttendanceHandler, SearchAttendanceHandler>();
            #endregion

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

            #region LessonEvaluations
            services.AddScoped<ICreateLessonEvaluationHandler, CreateLessonEvaluationHandler>();
            services.AddScoped<IUpdateLessonEvaluationHandler, UpdateLessonEvaluationHandler>();
            services.AddScoped<IDeleteLessonEvaluationHandler, DeleteLessonEvaluationHandler>();
            services.AddScoped<IGetLessonEvaluationByIdHandler, GetLessonEvaluationByIdHandler>();
            services.AddScoped<ISearchLessonEvaluationHandler, SearchLessonEvaluationHandler>();
            #endregion

            #region Scores
            services.AddScoped<ICreateScoreHandler, CreateScoreHandler>();
            services.AddScoped<IUpdateScoreHandler, UpdateScoreHandler>();
            services.AddScoped<IDeleteScoreHandler, DeleteScoreHandler>();
            services.AddScoped<IGetScoreByIdHandler, GetScoreByIdHandler>();
            services.AddScoped<ISearchScoreHandler, SearchScoreHandler>();
            #endregion

            #region ScoreStatistics
            services.AddScoped<ICreateScoreStatisticHandler, CreateScoreStatisticHandler>();
            services.AddScoped<IUpdateScoreStatisticHandler, UpdateScoreStatisticHandler>();
            services.AddScoped<IDeleteScoreStatisticHandler, DeleteScoreStatisticHandler>();
            services.AddScoped<IGetScoreStatisticByIdHandler, GetScoreStatisticByIdHandler>();
            services.AddScoped<ISearchScoreStatisticHandler, SearchScoreStatisticHandler>();
            services.AddScoped<ICalculateScoreStatisticHandler, CalculateScoreStatisticHandler>();
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

            #region TuitionFees
            services.AddScoped<ICreateTuitionFeeHandler, CreateTuitionFeeHandler>();
            services.AddScoped<IUpdateTuitionFeeHandler, UpdateTuitionFeeHandler>();
            services.AddScoped<IDeleteTuitionFeeHandler, DeleteTuitionFeeHandler>();
            services.AddScoped<IGetTuitionFeeByIdHandler, GetTuitionFeeByIdHandler>();
            services.AddScoped<ISearchTuitionFeeHandler, SearchTuitionFeeHandler>();
            services.AddScoped<IEstimateTuitionFeeHandler, EstimateTuitionFeeHandler>();
            #endregion

            #region Payments
            services.AddScoped<ICreatePaymentHandler, CreatePaymentHandler>();
            services.AddScoped<IUpdatePaymentHandler, UpdatePaymentHandler>();
            services.AddScoped<IDeletePaymentHandler, DeletePaymentHandler>();
            services.AddScoped<IGetPaymentByIdHandler, GetPaymentByIdHandler>();
            services.AddScoped<ISearchPaymentHandler, SearchPaymentHandler>();
            #endregion

            #region PaymentChecks
            services.AddScoped<ICreatePaymentCheckHandler, CreatePaymentCheckHandler>();
            services.AddScoped<IGetPaymentCheckByIdHandler, GetPaymentCheckByIdHandler>();
            services.AddScoped<ISearchPaymentCheckHandler, SearchPaymentCheckHandler>();
            #endregion

            #region BackgroundJobs
            services.AddScoped<IRunTenantSubscriptionBillingJobHandler, RunTenantSubscriptionBillingJobHandler>();
            services.AddScoped<IGetBackgroundJobByIdHandler, GetBackgroundJobByIdHandler>();
            services.AddScoped<ISearchBackgroundJobHandler, SearchBackgroundJobHandler>();
            #endregion

            #region Students
            services.AddScoped<ICreateStudentHandler, CreateStudentHandler>();
            services.AddScoped<ISearchStudentHandler, SearchStudentHandler>();
            #endregion

            return services;
        }
    }
}
