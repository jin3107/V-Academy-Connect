using MayNghien.Infrastructures.Repository;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VAcademyConnect.Models.Data;
using VAcademyConnect.Models.Entities;
using VAcademyConnect.Repositories.Interfaces;

namespace VAcademyConnect.Repositories.Implements
{
    public class SubscriptionRepository : GenericRepository<Subscription, ApplicationDbContext, ApplicationUser>, ISubscriptionRepository
    {
        public SubscriptionRepository(ApplicationDbContext unitOfWork) : base(unitOfWork)
        {
        }

        public async Task<Subscription?> GetTrialPlanAsync()
        {
            var trialPlan = await _context.Subscriptions
                .Where(s => s.TrialDays > 0 && s.Name == "Standard").FirstOrDefaultAsync();
            return trialPlan;
        }
    }
}
