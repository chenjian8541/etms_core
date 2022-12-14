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
    public class NoticeUserOfClassTodayJob : BaseTenantHandle
    {
        private readonly IEventPublisher _eventPublisher;

        private DateTime _classOt;

        private DateTime _nowTime;

        public NoticeUserOfClassTodayJob(ISysTenantDAL sysTenantDAL, IEventPublisher eventPublisher)
            :base(sysTenantDAL)
        {
            this._eventPublisher = eventPublisher;
            var now = DateTime.Now;
            _classOt = now.Date;
            _nowTime = new DateTime(now.Year, now.Month, now.Day, now.Hour, now.Minute, 1);
        }

        public override async Task ProcessTenant(SysTenant tenant)
        {
            _eventPublisher.Publish(new NoticeTeacherOfClassTodayTenantEvent(tenant.Id)
            {
                ClassOt = _classOt,
                NowTime = _nowTime
            });
        }
    }
}