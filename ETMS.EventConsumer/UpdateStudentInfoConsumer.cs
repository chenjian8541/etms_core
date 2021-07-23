using ETMS.Event.DataContract;
using ETMS.IBusiness.EventConsumer;
using ETMS.IOC;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.EventConsumer
{
    [QueueConsumerAttribution("UpdateStudentInfoQueue")]
    public class UpdateStudentInfoConsumer : ConsumerBase<UpdateStudentInfoEvent>
    {
        protected override async Task Receive(UpdateStudentInfoEvent request)
        {
            var evStudentBLL = CustomServiceLocator.GetInstance<IEvStudentBLL>();
            evStudentBLL.InitTenantId(request.TenantId);
            await evStudentBLL.UpdateStudentInfoConsumerEvent(request);
        }
    }
}
