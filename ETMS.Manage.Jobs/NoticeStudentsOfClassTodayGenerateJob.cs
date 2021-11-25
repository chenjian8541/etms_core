using ETMS.Entity.Database.Manage;
using ETMS.Event.DataContract;
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
    /// <summary>
    /// 定时生成上课提醒分析记录
    /// </summary>
    public class NoticeStudentsOfClassTodayGenerateJob : BaseTenantHandle
    {
        private readonly IEventPublisher _eventPublisher;

        private DateTime _classOt;

        public NoticeStudentsOfClassTodayGenerateJob(ISysTenantDAL sysTenantDAL, 
            IEventPublisher eventPublisher):base(sysTenantDAL)
        {
            this._eventPublisher = eventPublisher;
            _classOt = DateTime.Now.Date;
        }

        public override async Task ProcessTenant(SysTenant tenant)
        {
            _eventPublisher.Publish(new NoticeStudentsOfClassTodayGenerateEvent(tenant.Id)
            {
                ClassOt = _classOt
            });
        }
    }
}
