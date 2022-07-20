using ETMS.Entity.Database.Manage;
using ETMS.Event.DataContract;
using ETMS.Event.DataContract.Activity;
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
    public class EveryEvening8Job : BaseTenantHandle
    {
        private readonly IEventPublisher _eventPublisher;

        private readonly DateTime MyTime;

        public EveryEvening8Job(ISysTenantDAL sysTenantDAL, IEventPublisher eventPublisher)
            : base(sysTenantDAL)
        {
            this._eventPublisher = eventPublisher;
            this.MyTime = DateTime.Now.AddDays(-1).Date;
        }

        public override async Task ProcessTenant(SysTenant tenant)
        {
            _eventPublisher.Publish(new EveryDayStatisticsEvent(tenant.Id)
            {
                Time = MyTime
            });
        }
    }
}
