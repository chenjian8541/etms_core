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
    public class SystemDataBackupsJob : BaseJob
    {
        private readonly IEventPublisher _eventPublisher;

        public SystemDataBackupsJob(IEventPublisher eventPublisher)
        {
            this._eventPublisher = eventPublisher;
        }

        public override async Task Process(JobExecutionContext context)
        {
            _eventPublisher.Publish(new SystemDataBackupsEvent(0));
        }
    }
}
