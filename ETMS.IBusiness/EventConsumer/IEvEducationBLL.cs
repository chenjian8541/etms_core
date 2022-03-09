using ETMS.Event.DataContract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.IBusiness.EventConsumer
{
    public interface IEvEducationBLL : IBaseBLL
    {
        Task StatisticsTeacherSalaryClassTimesConsumerEvent(StatisticsTeacherSalaryClassTimesEvent request);

        Task StatisticsTeacherSalaryClassDayConsumerEvent(StatisticsTeacherSalaryClassDayEvent request);

        Task StatisticsTeacherSalaryMonthConsumerEvent(StatisticsTeacherSalaryMonthEvent request);

        Task SyncTeacherMonthClassTimesConsumerEvent(SyncTeacherMonthClassTimesEvent request);

        Task SyncMallGoodsRelatedNameConsumerEvent(SyncMallGoodsRelatedNameEvent request);

        Task SyncStudentLogOfSurplusCourseConsumerEvent(SyncStudentLogOfSurplusCourseEvent request);
    }
}
