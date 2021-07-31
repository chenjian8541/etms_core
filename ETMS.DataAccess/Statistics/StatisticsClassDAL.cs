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

        public async Task StatisticsClassAttendanceSave(EtStatisticsClassAttendance entity)
        {
            var log = await _dbWrapper.Find<EtStatisticsClassAttendance>(p => p.TenantId == _tenantId && p.IsDeleted == EmIsDeleted.Normal && p.ClassRecordId == entity.ClassRecordId);
            if (log == null)
            {
                await _dbWrapper.Insert(entity);
            }
            else
            {
                log.Attendance = entity.Attendance;
                log.AttendNumber = entity.AttendNumber;
                log.NeedAttendNumber = entity.NeedAttendNumber;
                await _dbWrapper.Update(log);
            }
        }

        public async Task StatisticsClassAttendanceDel(long classRecordId)
        {
            var sql = $"DELETE EtStatisticsClassAttendance WHERE TenantId = {_tenantId} AND ClassRecordId = {classRecordId}";
            await _dbWrapper.Execute(sql);
        }

        public async Task<List<EtStatisticsClassAttendance>> StatisticsClassAttendanceGet(DateTime startTime, DateTime endTime)
        {
            return await _dbWrapper.FindList<EtStatisticsClassAttendance>(p => p.TenantId == _tenantId && p.IsDeleted == EmIsDeleted.Normal && p.Ot >= startTime && p.Ot <= endTime);
        }

        public async Task StatisticsClassTimesSave(DateTime ot)
        {
            var totalData = await _dbWrapper.ExecuteObject<StatisticsClassTimesView>($"SELECT SUM(ClassTimes) as TotalClassTimes,SUM(DeSum) AS TotalDeSum FROM EtClassRecord WHERE TenantId = {_tenantId} AND IsDeleted = {EmIsDeleted.Normal} AND [Status] = {EmClassRecordStatus.Normal} AND ClassOt = '{ot.EtmsToDateString()}'");
            var myTodayData = totalData.FirstOrDefault();
            var totalClassTimes = 0M;
            var totalDeSum = 0M;
            if (myTodayData != null)
            {
                totalClassTimes = myTodayData.TotalClassTimes;
                totalDeSum = myTodayData.TotalDeSum;
            }
            var hisData = await this._dbWrapper.Find<EtStatisticsClassTimes>(p => p.TenantId == _tenantId && p.IsDeleted == EmIsDeleted.Normal && p.Ot == ot);
            if (hisData != null)
            {
                hisData.ClassTimes = totalClassTimes;
                hisData.DeSum = totalDeSum;
                await _dbWrapper.Update(hisData);
            }
            else
            {
                await _dbWrapper.Insert(new EtStatisticsClassTimes()
                {
                    ClassTimes = totalClassTimes,
                    DeSum = totalDeSum,
                    IsDeleted = EmIsDeleted.Normal,
                    Ot = ot,
                    TenantId = _tenantId
                });
            }
        }

        //public async Task StatisticsClassTimesDeduction(DateTime ot, decimal deClassTimes, decimal deDeSum)
        //{
        //    var hisData = await this._dbWrapper.Find<EtStatisticsClassTimes>(p => p.TenantId == _tenantId && p.IsDeleted == EmIsDeleted.Normal && p.Ot == ot);
        //    if (hisData != null)
        //    {
        //        hisData.ClassTimes -= deClassTimes;
        //        hisData.DeSum -= deDeSum;
        //        await _dbWrapper.Update(hisData);
        //    }
        //}

        public async Task<List<EtStatisticsClassTimes>> StatisticsClassTimesGet(DateTime startTime, DateTime endTime)
        {
            return await _dbWrapper.FindList<EtStatisticsClassTimes>(p => p.TenantId == _tenantId && p.IsDeleted == EmIsDeleted.Normal && p.Ot >= startTime && p.Ot <= endTime);
        }

        public async Task StatisticsClassCourseSave(DateTime ot, long courseId, decimal addClassTimes)
        {
            var hisData = await this._dbWrapper.Find<EtStatisticsClassCourse>(p => p.TenantId == _tenantId && p.IsDeleted == EmIsDeleted.Normal && p.Ot == ot && p.CourseId == courseId);
            if (hisData != null)
            {
                hisData.ClassTimes += addClassTimes;
                await _dbWrapper.Update(hisData);
            }
            else
            {
                await _dbWrapper.Insert(new EtStatisticsClassCourse()
                {
                    ClassTimes = addClassTimes,
                    CourseId = courseId,
                    IsDeleted = EmIsDeleted.Normal,
                    Ot = ot,
                    TenantId = _tenantId
                });
            }
        }

        public async Task StatisticsClassCourseDeduction(DateTime ot, long courseId, decimal deClassTimes)
        {
            var hisData = await this._dbWrapper.Find<EtStatisticsClassCourse>(p => p.TenantId == _tenantId && p.IsDeleted == EmIsDeleted.Normal && p.Ot == ot && p.CourseId == courseId);
            if (hisData != null)
            {
                hisData.ClassTimes -= deClassTimes;
                await _dbWrapper.Update(hisData);
            }
        }

        public async Task<IEnumerable<StatisticsClassCourseView>> StatisticsClassCourseGet(DateTime startTime, DateTime endTime, int topLimit)
        {
            return await _dbWrapper.ExecuteObject<StatisticsClassCourseView>($"SELECT TOP {topLimit} CourseId,SUM(ClassTimes) AS TotalClassTimes FROM EtStatisticsClassCourse WHERE TenantId = {_tenantId} AND IsDeleted = {EmIsDeleted.Normal} AND Ot >= '{startTime.EtmsToDateString()}' AND Ot <= '{endTime.EtmsToDateString()}' GROUP BY CourseId ORDER BY TotalClassTimes DESC");
        }

        public async Task StatisticsClassTeacherSave(DateTime ot, long teacherId, decimal addClassTimes)
        {
            var hisData = await this._dbWrapper.Find<EtStatisticsClassTeacher>(p => p.TenantId == _tenantId && p.IsDeleted == EmIsDeleted.Normal && p.Ot == ot && p.TeacherId == teacherId);
            if (hisData != null)
            {
                hisData.ClassTimes += addClassTimes;
                await _dbWrapper.Update(hisData);
            }
            else
            {
                await _dbWrapper.Insert(new EtStatisticsClassTeacher()
                {
                    ClassTimes = addClassTimes,
                    IsDeleted = EmIsDeleted.Normal,
                    Ot = ot,
                    TeacherId = teacherId,
                    TenantId = _tenantId
                });
            }
        }

        public async Task StatisticsClassTeacherDeduction(DateTime ot, long teacherId, decimal deClassTimes)
        {
            var hisData = await this._dbWrapper.Find<EtStatisticsClassTeacher>(p => p.TenantId == _tenantId && p.IsDeleted == EmIsDeleted.Normal && p.Ot == ot && p.TeacherId == teacherId);
            if (hisData != null)
            {
                hisData.ClassTimes -= deClassTimes;
                await _dbWrapper.Update(hisData);
            }
        }

        public async Task<IEnumerable<StatisticsClassTeacherView>> StatisticsClassTeacherGet(DateTime startTime, DateTime endTime, int topLimit)
        {
            return await _dbWrapper.ExecuteObject<StatisticsClassTeacherView>($"SELECT TOP {topLimit} TeacherId,SUM(ClassTimes) AS TotalClassTimes FROM EtStatisticsClassTeacher WHERE TenantId = {_tenantId} AND IsDeleted = {EmIsDeleted.Normal} AND Ot >= '{startTime.EtmsToDateString()}' AND Ot <= '{endTime.EtmsToDateString()}' GROUP BY TeacherId ORDER BY TotalClassTimes DESC");
        }
    }
}
