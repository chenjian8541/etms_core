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
    /// 上课前一天通知
    /// </summary>
    public class NoticeStudentsOfClassBeforeDayJob : BaseTenantHandle
    {
        private readonly IEventPublisher _eventPublisher;

        private DateTime _classOt;

        private int _nowTime;

        public NoticeStudentsOfClassBeforeDayJob(ISysTenantDAL sysTenantDAL, IEventPublisher eventPublisher)
            :base(sysTenantDAL)
        {
            this._eventPublisher = eventPublisher;
            _classOt = DateTime.Now.AddDays(1).Date;
            _nowTime = Convert.ToInt32(DateTime.Now.ToString("HHmm"));
        }

        public override async Task ProcessTenant(SysTenant tenant)
        {
            _eventPublisher.Publish(new NoticeStudentsOfClassBeforeDayTenantEvent(tenant.Id)
            {
                ClassOt = _classOt,
                NowTime = _nowTime
            });
        }
    }
}
