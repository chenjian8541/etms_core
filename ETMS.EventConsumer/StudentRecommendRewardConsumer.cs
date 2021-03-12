using ETMS.Event.DataContract;
using ETMS.IBusiness.EventConsumer;
using ETMS.IOC;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.EventConsumer
{
    [QueueConsumerAttribution("StudentRecommendRewardQueue")]
    public class StudentRecommendRewardConsumer : ConsumerBase<StudentRecommendRewardEvent>
    {
        protected override async Task Receive(StudentRecommendRewardEvent request)
        {
            var evStudentBLL = CustomServiceLocator.GetInstance<IEvStudentBLL>();
            evStudentBLL.InitTenantId(request.TenantId);
            await evStudentBLL.StudentRecommendRewardConsumerEvent(request);
        }
    }
}
