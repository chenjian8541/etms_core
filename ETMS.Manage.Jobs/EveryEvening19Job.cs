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
    public class EveryEvening19Job : BaseTenantHandle
    {
        private readonly IEventPublisher _eventPublisher;

        public EveryEvening19Job(ISysTenantDAL sysTenantDAL, IEventPublisher eventPublisher)
            : base(sysTenantDAL)
        {
            this._eventPublisher = eventPublisher;
        }

        public override async Task ProcessTenant(SysTenant tenant)
        {
            _eventPublisher.Publish(new ActivityAutoRefundTenantEvent(tenant.Id));
        }
    }
}
