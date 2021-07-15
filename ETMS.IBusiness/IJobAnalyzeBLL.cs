using ETMS.Entity.Temp;
using ETMS.Event.DataContract;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.IBusiness
{
    public interface IJobAnalyzeBLL : IBaseBLL, IMultiTenant
    {
        Task UpdateClassTimesRuleLoopStatus();

        Task<Tuple<IEnumerable<LoopClassTimesRule>, int>> GetNeedLoopClassTimesRule(int pageSize, int pageCurrent);

        Task GenerateClassTimesEvent(GenerateClassTimesEvent request);

        Task<Tuple<IEnumerable<StudentCourseConsume>, int>> GetNeedConsumeStudentCourse(int pageSize, int pageCurrent, DateTime time);

        Task ConsumeStudentCourseProcessEvent(ConsumeStudentCourseEvent request);

        Task<Tuple<IEnumerable<HasCourseStudent>, int>> GetHasCourseStudent(int pageSize, int pageCurrent);

        Task TenantClassTimesTodayConsumerEvent(TenantClassTimesTodayEvent request);

        Task SyncParentStudentsConsumerEvent(SyncParentStudentsEvent request);

        Task SyncStudentAccountRechargeLogPhoneConsumerEvent(SyncStudentAccountRechargeLogPhoneEvent request);
        Task<Tuple<IEnumerable<OnlyId>, int>> GetStudent(int pageSize, int pageCurrent);
    }
}
