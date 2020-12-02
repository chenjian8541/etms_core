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
    public class NoticeUserOfClassTodayJob : BaseJob
    {
        private readonly ISysTenantDAL _sysTenantDAL;

        private readonly IEventPublisher _eventPublisher;

        public NoticeUserOfClassTodayJob(ISysTenantDAL sysTenantDAL, IEventPublisher eventPublisher)
        {
            this._sysTenantDAL = sysTenantDAL;
            this._eventPublisher = eventPublisher;
        }

        public override async Task Process(JobExecutionContext context)
        {
            var now = DateTime.Now;
            var classOt = now.Date;
            var nowTime = new DateTime(now.Year, now.Month, now.Day, now.Hour, now.Minute, 1);
            var tenantList = await _sysTenantDAL.GetTenantsNormal();
            foreach (var tenant in tenantList)
            {
                _eventPublisher.Publish(new NoticeTeacherOfClassTodayTenantEvent(tenant.Id)
                {
                    ClassOt = classOt,
                    NowTime = nowTime
                });
            }
        }
    }
}