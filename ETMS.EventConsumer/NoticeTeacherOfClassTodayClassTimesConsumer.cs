using ETMS.Event.DataContract;
using ETMS.IBusiness.SendNotice;
using ETMS.IOC;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.EventConsumer
{
    [QueueConsumerAttribution("NoticeTeacherOfClassTodayClassTimesQueue")]
    public class NoticeTeacherOfClassTodayClassTimesConsumer : ConsumerBase<NoticeTeacherOfClassTodayClassTimesEvent>
    {
        protected override async Task Receive(NoticeTeacherOfClassTodayClassTimesEvent eEvent)
        {
            var userSendNoticeBLL = CustomServiceLocator.GetInstance<IUserSendNoticeBLL>();
            userSendNoticeBLL.InitTenantId(eEvent.TenantId);
            await userSendNoticeBLL.NoticeTeacherOfClassTodayClassTimesConsumerEvent(eEvent);
        }
    }
}