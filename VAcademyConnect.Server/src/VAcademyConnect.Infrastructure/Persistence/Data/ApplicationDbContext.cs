using MayNghien.Infrastructures.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VAcademyConnect.Domain.Entities;

namespace VAcademyConnect.Infrastructure.Persistence.Data
{
    public class ApplicationDbContext : BaseContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<Tenant> Tenants { get; set; }
        public DbSet<Staff> Staffs { get; set; }
        public DbSet<Subscription> Subscriptions { get; set; }
        public DbSet<TenantSubscription> TenantSubscriptions { get; set; }
        public DbSet<Class> Classes { get; set; }
        public DbSet<ClassStudent> ClassStudents { get; set; }
        public DbSet<Attendance> Attendances { get; set; }
        public DbSet<LessonEvaluation> LessonEvaluations { get; set; }
        public DbSet<Score> Scores { get; set; }
        public DbSet<TuitionFee> TuitionFees { get; set; }
        public DbSet<Payment> Payments { get; set; }
        public DbSet<PaymentCheck> PaymentChecks { get; set; }
        public DbSet<ScoreStatistic> ScoreStatistics { get; set; }
        public DbSet<BackgroundJob> BackgroundJobs { get; set; }
        public DbSet<RefreshTokenModel> RefreshTokenModels { get; set; }
    }
}
