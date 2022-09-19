using ETMS.Event.DataContract;
using ETMS.IBusiness.SendNotice;
using ETMS.IOC;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.EventConsumer
{
    [QueueConsumerAttribution("NoticeSendArrearageBatchQueue")]
    public class NoticeSendArrearageBatchConsumer: ConsumerBase<NoticeSendArrearageBatchEvent>
    {
        protected override async Task Receive(NoticeSendArrearageBatchEvent eEvent)
        {
            var studentSendNotice2BLL = CustomServiceLocator.GetInstance<IStudentSendNotice2BLL>();
            studentSendNotice2BLL.InitTenantId(eEvent.TenantId);
            await studentSendNotice2BLL.NoticeSendArrearageBatchConsumerEvent(eEvent);
        }
    }
}
