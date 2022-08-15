using ETMS.Entity.Database.Manage;
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
    public class AnalyzeTenantCommonJob : BaseTenantHandle
    {
        private readonly IEventPublisher _eventPublisher;

        private DateTime _analyzeDate { get; set; }

        public AnalyzeTenantCommonJob(ISysTenantDAL sysTenantDAL, IEventPublisher eventPublisher)
            : base(sysTenantDAL)
        {
            this._eventPublisher = eventPublisher;
            _analyzeDate = DateTime.Now.AddDays(-1).Date;
        }

        public override async Task ProcessTenant(SysTenant tenant)
        {
            _eventPublisher.Publish(new StudentCheckOnAutoGenerateClassRecordEvent(tenant.Id)
            {
                AnalyzeDate = _analyzeDate
            });

            _eventPublisher.Publish(new AutoSyncTenantClassEvent(tenant.Id));
        }
    }
}
