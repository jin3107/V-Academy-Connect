using MayNghien.Infrastructures.Repository;
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
    public class TenantSubscriptionRepository : GenericRepository<TenantSubscription, ApplicationDbContext, ApplicationUser>, ITenantSubscriptionRepository
    {
        public TenantSubscriptionRepository(ApplicationDbContext unitOfWork) : base(unitOfWork)
        {
        }
    }
}
