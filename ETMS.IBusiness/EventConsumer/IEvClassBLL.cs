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

        Task SyncClassInfoAboutDelStudentProcessEvent(SyncClassInfoAboutDelStudentEvent request);

        Task StudentCourseMarkExceedConsumerEvent(StudentCourseMarkExceedEvent request);

        Task SyncClassCategoryIdConsumerEvent(SyncClassCategoryIdEvent request);

        Task AutoSyncTenantClassConsumerEvent(AutoSyncTenantClassEvent request);

        Task AutoSyncTenantClassDetailConsumerEvent(AutoSyncTenantClassDetailEvent request);

        Task SyncClassTimesRuleStudentInfoConsumerEvent(SyncClassTimesRuleStudentInfoEvent request);
    }
}
