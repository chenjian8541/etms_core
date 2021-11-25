using ETMS.Entity.Database.Manage;
using ETMS.Event.DataContract;
using ETMS.Event.DataContract.Statistics;
using ETMS.IDataAccess.EtmsManage;
using ETMS.IEventProvider;
using ETMS.Manage.Entity.Config;
using ETMS.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.Manage.Jobs
{
    public class StatisticsTenantDataJob : BaseTenantHandle
    {
        private readonly IEventPublisher _eventPublisher;

        private DateTime _now;

        public StatisticsTenantDataJob(ISysTenantDAL sysTenantDAL, IEventPublisher eventPublisher)
            : base(sysTenantDAL)
        {
            this._eventPublisher = eventPublisher;
            _now = DateTime.Now.AddDays(-1).Date;
        }
        public override async Task ProcessTenant(SysTenant tenant)
        {
            _eventPublisher.Publish(new StatisticsEducationEvent(tenant.Id)
            {
                Time = _now
            });
            _eventPublisher.Publish(new StatisticsFinanceIncomeMonthEvent(tenant.Id)
            {
                Time = _now
            });
            _eventPublisher.Publish(new StatisticsTeacherSalaryClassDayEvent(tenant.Id)
            {
                Time = _now,
                IsJobRequest = true
            });
            _eventPublisher.Publish(new StatisticsLcsPayEvent(tenant.Id)
            {
                StatisticsDate = _now
            });
            _eventPublisher.Publish(new SyncTenantLastOpTimeEvent(tenant.Id));
        }
    }
}
