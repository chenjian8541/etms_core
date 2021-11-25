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
    public class NoticeStudentsOfClassTodayJob : BaseTenantHandle
    {
        private readonly IEventPublisher _eventPublisher;

        private DateTime _classOt;

        private DateTime _nowTime;

        public NoticeStudentsOfClassTodayJob(ISysTenantDAL sysTenantDAL, IEventPublisher eventPublisher)
            :base(sysTenantDAL)
        {
            this._eventPublisher = eventPublisher;
            var now = DateTime.Now;
            this._classOt = now.Date;
            this._nowTime = new DateTime(now.Year, now.Month, now.Day, now.Hour, now.Minute, 1);
        }

        public override async Task ProcessTenant(SysTenant tenant)
        {
            _eventPublisher.Publish(new NoticeStudentsOfClassTodayTenantEvent(tenant.Id)
            {
                ClassOt = _classOt,
                NowTime = _nowTime
            });
        }
    }
}
