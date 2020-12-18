using ETMS.Event.DataContract;
using ETMS.IBusiness;
using ETMS.IOC;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.EventConsumer
{
    [QueueConsumerAttribution("NoticeStudentContractsQueue")]
    public class NoticeStudentContractsConsumer : ConsumerBase<NoticeStudentContractsEvent>
    {
        protected override async Task Receive(NoticeStudentContractsEvent eEvent)
        {
            var studentSendNoticeBLL = CustomServiceLocator.GetInstance<IStudentSendNoticeBLL>();
            studentSendNoticeBLL.InitTenantId(eEvent.TenantId);
            await studentSendNoticeBLL.NoticeStudentContracts(eEvent);
        }
    }
}