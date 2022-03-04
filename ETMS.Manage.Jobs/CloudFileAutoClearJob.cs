using ETMS.Entity.Database.Manage;
using ETMS.Entity.Database.Source;
using ETMS.Entity.Enum;
using ETMS.Entity.Temp;
using ETMS.Event.DataContract;
using ETMS.IBusiness;
using ETMS.IDataAccess;
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
    public class CloudFileAutoClearJob : BaseTenantHandle
    {
        private readonly IEventPublisher _eventPublisher;

        public CloudFileAutoClearJob(ISysTenantDAL sysTenantDAL, IEventPublisher eventPublisher) : base(sysTenantDAL)
        {
            this._eventPublisher = eventPublisher;
        }

        public override async Task ProcessTenant(SysTenant tenant)
        {
            var myDate = DateTime.Now.AddDays(-1);
            foreach (var itemFile in EmTenantCloudStorageType.TenantCloudStorageTypeTags)
            {
                _eventPublisher.Publish(new CloudFileAutoDelDayEvent(tenant.Id)
                {
                    DelDate = myDate,
                    FileTag = itemFile.Tag
                });
            }
        }
    }
}
