using ETMS.Entity.CacheBucket.RedisLock;
using ETMS.Event.DataContract;
using ETMS.EventConsumer.Lib;
using ETMS.IBusiness.EventConsumer;
using ETMS.IBusiness.SendNotice;
using ETMS.IOC;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.EventConsumer.Notice
{
    [QueueConsumerAttribution("NoticeStudentToClassQueue")]
    public class NoticeStudentToClassConsumer: ConsumerBase<NoticeStudentToClassEvent>
    {
        protected override async Task Receive(NoticeStudentToClassEvent eEvent)
        {
            var _lockKey = new NoticeStudentToClassToken(eEvent.TenantId);
            var studentSendNotice3BLL = CustomServiceLocator.GetInstance<IStudentSendNotice3BLL>();
            studentSendNotice3BLL.InitTenantId(eEvent.TenantId);
            var lockTakeHandler = new LockTakeHandler<NoticeStudentToClassToken, NoticeStudentToClassEvent>(_lockKey, eEvent, this.ClassName,
                async () =>
                await studentSendNotice3BLL.NoticeStudentToClassConsumerEvent(eEvent)
                );
            await lockTakeHandler.Process();
        }
    }
}
