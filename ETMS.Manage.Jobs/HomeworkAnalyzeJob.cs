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
    public class HomeworkAnalyzeJob : BaseTenantHandle
    {
        private readonly IEventPublisher _eventPublisher;

        private DateTime _myNow;

        public HomeworkAnalyzeJob(ISysTenantDAL sysTenantDAL, IEventPublisher eventPublisher)
            : base(sysTenantDAL)
        {
            this._eventPublisher = eventPublisher;
            this._myNow = DateTime.Now;
        }

        public override async Task ProcessTenant(SysTenant tenant)
        {
            _eventPublisher.Publish(new NoticeStudentsOfHomeworkNotAnswerEvent(tenant.Id)
            {
                MyNow = this._myNow
            });
        }
    }
}
