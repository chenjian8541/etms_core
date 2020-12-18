using ETMS.Event.DataContract;
using ETMS.IBusiness.SendNotice;
using ETMS.IOC;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.EventConsumer
{
    [QueueConsumerAttribution("NoticeTeacherOfClassTodayTenantQueue")]
    public class NoticeTeacherOfClassTodayTenantConsumer : ConsumerBase<NoticeTeacherOfClassTodayTenantEvent>
    {
        protected override async Task Receive(NoticeTeacherOfClassTodayTenantEvent eEvent)
        {
            var userSendNoticeBLL = CustomServiceLocator.GetInstance<IUserSendNoticeBLL>();
            userSendNoticeBLL.InitTenantId(eEvent.TenantId);
            await userSendNoticeBLL.NoticeTeacherOfClassTodayTenantConsumerEvent(eEvent);
        }
    }
}