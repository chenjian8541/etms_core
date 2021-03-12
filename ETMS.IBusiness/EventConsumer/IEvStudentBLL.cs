using ETMS.Event.DataContract;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.IBusiness.EventConsumer
{
    public interface IEvStudentBLL : IBaseBLL
    {
        Task StudentRecommendRewardConsumerEvent(StudentRecommendRewardEvent request);
    }
}
