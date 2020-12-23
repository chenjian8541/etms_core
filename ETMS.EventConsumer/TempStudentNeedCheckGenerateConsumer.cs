using ETMS.Event.DataContract;
using ETMS.IBusiness;
using ETMS.IOC;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.EventConsumer
{
    [QueueConsumerAttribution("TempStudentNeedCheckGenerateQueue")]
    public class TempStudentNeedCheckGenerateConsumer : ConsumerBase<TempStudentNeedCheckGenerateEvent>
    {
        protected override async Task Receive(TempStudentNeedCheckGenerateEvent eEvent)
        {
            var tempDataGenerateBLL = CustomServiceLocator.GetInstance<ITempDataGenerateBLL>();
            tempDataGenerateBLL.InitTenantId(eEvent.TenantId);
            await tempDataGenerateBLL.TempStudentNeedCheckGenerateConsumerEvent(eEvent);
        }
    }
}

