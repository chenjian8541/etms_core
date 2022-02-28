using ETMS.Entity.Database.Manage;
using ETMS.Event.DataContract;
using ETMS.IDataAccess.EtmsManage;
using ETMS.IEventProvider;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.Manage.Jobs
{
    public class CloudStorageAnalyzeJob : BaseTenantALLHandle
    {
        private readonly IEventPublisher _eventPublisher;

        public CloudStorageAnalyzeJob(ISysTenantDAL sysTenantDAL, IEventPublisher eventPublisher)
            : base(sysTenantDAL)
        {
            this._eventPublisher = eventPublisher;
        }

        public override async Task ProcessTenant(SysTenant tenant)
        {
            _eventPublisher.Publish(new CloudStorageAnalyzeEvent(tenant.Id)
            {
                AgentId = tenant.AgentId
            });
        }
    }
}
