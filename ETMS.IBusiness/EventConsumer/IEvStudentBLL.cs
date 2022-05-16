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

        Task StudentAutoMarkGraduationConsumerEvent(StudentAutoMarkGraduationEvent request);

        Task SyncStudentClassInfoConsumerEvent(SyncStudentClassInfoEvent request);

        Task UpdateStudentInfoConsumerEvent(UpdateStudentInfoEvent request);

        Task ImportExtendFieldExcelConsumerEvent(ImportExtendFieldExcelEvent request);

        Task SyncStudentCourseStatusConsumerEvent(SyncStudentCourseStatusEvent request);

        Task StudentCourseRestoreTimeBatchConsumerEvent(StudentCourseRestoreTimeBatchEvent request);

        Task SyncStudentStudentClassIdsConsumerEvent(SyncStudentStudentClassIdsEvent request);

        Task SyncStudentStudentCourseIdsConsumerEvent(SyncStudentStudentCourseIdsEvent request);

        Task SyncStudentReadTypeConsumerEvent(SyncStudentReadTypeEvent request);
    }
}
