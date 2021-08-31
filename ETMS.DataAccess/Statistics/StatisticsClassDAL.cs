using ETMS.DataAccess.Core;
using ETMS.Entity.Database.Source;
using ETMS.Entity.Enum;
using ETMS.Entity.View;
using ETMS.Entity.View.Persistence;
using ETMS.IDataAccess;
using ETMS.Utility;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Linq;

namespace ETMS.DataAccess
{
    public class StatisticsClassDAL : DataAccessBase, IStatisticsClassDAL
    {
        public StatisticsClassDAL(IDbWrapper dbWrapper) : base(dbWrapper)
        {
        }

        public async Task SaveStatisticsClass(DateTime classOt, EtStatisticsClassTimes myStatisticsClassTimes,
            List<EtStatisticsClassAttendance> myStatisticsClassAttendances,
            List<EtStatisticsClassCourse> myStatisticsClassCourse,
            List<EtStatisticsClassTeacher> myStatisticsClassTeacher)
        {
            var otDesc = classOt.EtmsToDateString();
            var strSql = new StringBuilder();
            strSql.Append($"DELETE EtStatisticsClassTimes WHERE TenantId = {_tenantId} AND Ot = '{otDesc}' ;");
            strSql.Append($"DELETE EtStatisticsClassAttendance WHERE TenantId = {_tenantId} AND Ot = '{otDesc}' ;");
            strSql.Append($"DELETE EtStatisticsClassCourse WHERE TenantId = {_tenantId} AND Ot = '{otDesc}' ;");
            strSql.Append($"DELETE EtStatisticsClassTeacher WHERE TenantId = {_tenantId} AND Ot = '{otDesc}' ;");
            await _dbWrapper.Insert(myStatisticsClassTimes);
            if (myStatisticsClassAttendances.Count > 0)
            {
                _dbWrapper.InsertRange(myStatisticsClassAttendances);
            }
            if (myStatisticsClassCourse.Count > 0)
            {
                _dbWrapper.InsertRange(myStatisticsClassCourse);
            }
            if (myStatisticsClassTeacher.Count > 0)
            {
                _dbWrapper.InsertRange(myStatisticsClassTeacher);
            }
        }

        public async Task<List<EtStatisticsClassAttendance>> StatisticsClassAttendanceGet(DateTime startTime, DateTime endTime)
        {
            return await _dbWrapper.FindList<EtStatisticsClassAttendance>(p => p.TenantId == _tenantId && p.IsDeleted == EmIsDeleted.Normal && p.Ot >= startTime && p.Ot <= endTime);
        }

        public async Task<List<EtStatisticsClassTimes>> StatisticsClassTimesGet(DateTime startTime, DateTime endTime)
        {
            return await _dbWrapper.FindList<EtStatisticsClassTimes>(p => p.TenantId == _tenantId && p.IsDeleted == EmIsDeleted.Normal && p.Ot >= startTime && p.Ot <= endTime);
        }

        public async Task<IEnumerable<StatisticsClassCourseView>> StatisticsClassCourseGet(DateTime startTime, DateTime endTime, int topLimit)
        {
            return await _dbWrapper.ExecuteObject<StatisticsClassCourseView>($"SELECT TOP {topLimit} CourseId,SUM(ClassTimes) AS TotalClassTimes FROM EtStatisticsClassCourse WHERE TenantId = {_tenantId} AND IsDeleted = {EmIsDeleted.Normal} AND Ot >= '{startTime.EtmsToDateString()}' AND Ot <= '{endTime.EtmsToDateString()}' GROUP BY CourseId ORDER BY TotalClassTimes DESC");
        }

        public async Task<IEnumerable<StatisticsClassTeacherView>> StatisticsClassTeacherGet(DateTime startTime, DateTime endTime, int topLimit)
        {
            return await _dbWrapper.ExecuteObject<StatisticsClassTeacherView>($"SELECT TOP {topLimit} TeacherId,SUM(ClassTimes) AS TotalClassTimes FROM EtStatisticsClassTeacher WHERE TenantId = {_tenantId} AND IsDeleted = {EmIsDeleted.Normal} AND Ot >= '{startTime.EtmsToDateString()}' AND Ot <= '{endTime.EtmsToDateString()}' GROUP BY TeacherId ORDER BY TotalClassTimes DESC");
        }
    }
}
