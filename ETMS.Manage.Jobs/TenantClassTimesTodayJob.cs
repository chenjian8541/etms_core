using ETMS.Event.DataContract;
using ETMS.IDataAccess.EtmsManage;
using ETMS.IEventProvider;
using ETMS.Manage.Entity.Config;
using System.Threading.Tasks;
using System;
using System.Collections.Generic;
using ETMS.Entity.Database.Manage;
using System.Linq;
using ETMS.Utility;

namespace ETMS.Manage.Jobs
{
    public class TenantClassTimesTodayJob : BaseTenantHandle
    {
        private readonly IEventPublisher _eventPublisher;

        private DateTime _classOt;

        public TenantClassTimesTodayJob(ISysTenantDAL sysTenantDAL, IEventPublisher eventPublisher)
            : base(sysTenantDAL)
        {
            this._eventPublisher = eventPublisher;
            _classOt = DateTime.Now;
        }

        public override async Task ProcessTenant(SysTenant tenant)
        {
            _eventPublisher.Publish(new TenantClassTimesTodayEvent(tenant.Id)
            {
                ClassOt = _classOt
            });
        }
    }
}