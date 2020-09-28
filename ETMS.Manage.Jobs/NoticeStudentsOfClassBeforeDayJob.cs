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
    /// 上课前一天通知
    /// </summary>
    public class NoticeStudentsOfClassBeforeDayJob : BaseJob
    {
        private readonly ISysTenantDAL _sysTenantDAL;

        private readonly IEventPublisher _eventPublisher;

        public NoticeStudentsOfClassBeforeDayJob(ISysTenantDAL sysTenantDAL, IEventPublisher eventPublisher)
        {
            this._sysTenantDAL = sysTenantDAL;
            this._eventPublisher = eventPublisher;
        }

        public override async Task Process(JobExecutionContext context)
        {
            var classOt = DateTime.Now.AddDays(1).Date;
            var nowTime = Convert.ToInt32(DateTime.Now.ToString("HHmm"));
            var tenantList = await _sysTenantDAL.GetTenants();
            foreach (var tenant in tenantList)
            {
                _eventPublisher.Publish(new NoticeStudentsOfClassBeforeDayTenantEvent(tenant.Id)
                {
                    ClassOt = classOt,
                    NowTime = nowTime
                });
            }
        }
    }
}
