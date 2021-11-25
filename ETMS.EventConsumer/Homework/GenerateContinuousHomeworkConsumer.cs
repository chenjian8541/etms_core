using ETMS.Event.DataContract;
using ETMS.IBusiness.EventConsumer;
using ETMS.IOC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.EventConsumer
{
    [QueueConsumerAttribution("GenerateContinuousHomeworkQueue")]
    public class GenerateContinuousHomeworkConsumer : ConsumerBase<GenerateContinuousHomeworkEvent>
    {
        protected override async Task Receive(GenerateContinuousHomeworkEvent request)
        {
            var homeworkBLL = CustomServiceLocator.GetInstance<IEvHomeworkBLL>();
            homeworkBLL.InitTenantId(request.TenantId);
            await homeworkBLL.GenerateContinuousHomeworkConsumerEvent(request);
        }
    }
}
