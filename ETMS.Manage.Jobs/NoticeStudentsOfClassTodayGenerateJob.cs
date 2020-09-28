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
    /// <summary>
    /// 定时生成上课提醒分析记录
    /// </summary>
    public class NoticeStudentsOfClassTodayGenerateJob : BaseJob
    {
        private readonly ISysTenantDAL _sysTenantDAL;

        private readonly IEventPublisher _eventPublisher;

        public NoticeStudentsOfClassTodayGenerateJob(ISysTenantDAL sysTenantDAL, IEventPublisher eventPublisher)
        {
            this._sysTenantDAL = sysTenantDAL;
            this._eventPublisher = eventPublisher;
        }

        public override async Task Process(JobExecutionContext context)
        {
            var classOt = DateTime.Now.AddDays(1).Date;
            var tenantList = await _sysTenantDAL.GetTenants();
            foreach (var tenant in tenantList)
            {
                _eventPublisher.Publish(new NoticeStudentsOfClassTodayGenerateEvent(tenant.Id)
                {
                    ClassOt = classOt
                });
            }
        }
    }
}
