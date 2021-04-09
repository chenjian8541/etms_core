using ETMS.Entity.Database.Source;
using ETMS.Entity.View.Persistence;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.IDataAccess
{
    public interface IStatisticsClassDAL : IBaseDAL
    {
        Task StatisticsClassAttendanceAdd(EtStatisticsClassAttendance entity);

        Task StatisticsClassAttendanceDel(long classRecordId);

        Task<List<EtStatisticsClassAttendance>> StatisticsClassAttendanceGet(DateTime startTime, DateTime endTime);

        Task StatisticsClassTimesSave(DateTime ot, decimal addClassTimes, decimal addDeSum);

        Task StatisticsClassTimesDeduction(DateTime ot, decimal deClassTimes, decimal deDeSum);

        Task<List<EtStatisticsClassTimes>> StatisticsClassTimesGet(DateTime startTime, DateTime endTime);

        Task StatisticsClassCourseSave(DateTime ot, long courseId, decimal addClassTimes);

        Task StatisticsClassCourseDeduction(DateTime ot, long courseId, decimal deClassTimes);

        Task<IEnumerable<StatisticsClassCourseView>> StatisticsClassCourseGet(DateTime startTime, DateTime endTime, int topLimit);

        Task StatisticsClassTeacherSave(DateTime ot, long teacherId, decimal addClassTimes);

        Task StatisticsClassTeacherDeduction(DateTime ot, long teacherId, decimal deClassTimes);

        Task<IEnumerable<StatisticsClassTeacherView>> StatisticsClassTeacherGet(DateTime startTime, DateTime endTime, int topLimit);
    }
}
