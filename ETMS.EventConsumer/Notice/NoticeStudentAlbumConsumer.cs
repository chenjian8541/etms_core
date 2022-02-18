using ETMS.Event.DataContract;
using ETMS.IBusiness.SendNotice;
using ETMS.IOC;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.EventConsumer.Notice
{
    [QueueConsumerAttribution("NoticeStudentAlbumQueue")]
    public class NoticeStudentAlbumConsumer : ConsumerBase<NoticeStudentAlbumEvent>
    {
        protected override async Task Receive(NoticeStudentAlbumEvent eEvent)
        {
            var studentSendNoticeBLL = CustomServiceLocator.GetInstance<IStudentSendNotice3BLL>();
            studentSendNoticeBLL.InitTenantId(eEvent.TenantId);
            await studentSendNoticeBLL.NoticeStudentAlbumConsumerEvent(eEvent);
        }
    }
}
