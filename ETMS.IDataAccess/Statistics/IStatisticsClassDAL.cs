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
        Task SaveStatisticsClass(DateTime classOt,EtStatisticsClassTimes myStatisticsClassTimes,
            List<EtStatisticsClassAttendance> myStatisticsClassAttendances,
            List<EtStatisticsClassCourse> myStatisticsClassCourse,
            List<EtStatisticsClassTeacher> myStatisticsClassTeacher);

        Task<List<EtStatisticsClassAttendance>> StatisticsClassAttendanceGet(DateTime startTime, DateTime endTime);

        Task<List<EtStatisticsClassTimes>> StatisticsClassTimesGet(DateTime startTime, DateTime endTime);

        Task<IEnumerable<StatisticsClassCourseView>> StatisticsClassCourseGet(DateTime startTime, DateTime endTime, int topLimit);

        Task<IEnumerable<StatisticsClassTeacherView>> StatisticsClassTeacherGet(DateTime startTime, DateTime endTime, int topLimit);
    }
}
