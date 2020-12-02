using ETMS.Event.DataContract;
using ETMS.IDataAccess.EtmsManage;
using ETMS.IEventProvider;
using ETMS.Manage.Entity.Config;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.Manage.Jobs
{
    public class NoticeUserOfClassTodayGenerateJob : BaseJob
    {
        private readonly ISysTenantDAL _sysTenantDAL;

        private readonly IEventPublisher _eventPublisher;

        public NoticeUserOfClassTodayGenerateJob(ISysTenantDAL sysTenantDAL, IEventPublisher eventPublisher)
        {
            this._sysTenantDAL = sysTenantDAL;
            this._eventPublisher = eventPublisher;
        }

        public override async Task Process(JobExecutionContext context)
        {
            var classOt = DateTime.Now.AddDays(1).Date;
            var tenantList = await _sysTenantDAL.GetTenantsNormal();
            foreach (var tenant in tenantList)
            {
                _eventPublisher.Publish(new NoticeUserOfClassTodayGenerateEvent(tenant.Id)
                {
                    ClassOt = classOt
                });
            }
        }
    }
}
