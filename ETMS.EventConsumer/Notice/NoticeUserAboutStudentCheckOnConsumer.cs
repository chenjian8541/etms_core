using ETMS.Event.DataContract;
using ETMS.IBusiness.SendNotice;
using ETMS.IOC;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.EventConsumer
{
    [QueueConsumerAttribution("NoticeUserAboutStudentCheckOnQueue")]
    public class NoticeUserAboutStudentCheckOnConsumer : ConsumerBase<NoticeUserAboutStudentCheckOnEvent>
    {
        protected override async Task Receive(NoticeUserAboutStudentCheckOnEvent eEvent)
        {
            var userSendNotice2BLL = CustomServiceLocator.GetInstance<IUserSendNotice2BLL>();
            userSendNotice2BLL.InitTenantId(eEvent.TenantId);
            await userSendNotice2BLL.NoticeUserAboutStudentCheckOnConsumerEvent(eEvent);
        }
    }
}
