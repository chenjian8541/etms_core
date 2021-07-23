using ETMS.Event.DataContract;
using ETMS.Event.DataContract.Statistics;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.IBusiness.EventConsumer
{
    public interface IEvClassBLL: IBaseBLL
    {
        Task ClassOfOneAutoOverConsumerEvent(ClassOfOneAutoOverEvent request);

        Task ClassRemoveStudentConsumerEvent(ClassRemoveStudentEvent request);

        Task SyncClassTimesStudentConsumerEvent(SyncClassTimesStudentEvent request);

        Task StatisticsEducationConsumerEvent(StatisticsEducationEvent request);

        Task StatisticsClassFinishCountConsumerEvent(StatisticsClassFinishCountEvent request);

        Task StudentCheckOnAutoGenerateClassRecordConsumerEvent(StudentCheckOnAutoGenerateClassRecordEvent request);
    }
}
