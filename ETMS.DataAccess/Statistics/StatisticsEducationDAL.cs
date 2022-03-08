using ETMS.DataAccess.Core;
using ETMS.Entity.Database.Source;
using ETMS.Entity.Enum;
using ETMS.Entity.View.Persistence;
using ETMS.IDataAccess;
using ETMS.Utility;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using ETMS.IDataAccess.Statistics;
using ETMS.Entity.Temp.View;
using ETMS.Entity.Common;

namespace ETMS.DataAccess.Statistics
{
    public class StatisticsEducationDAL : DataAccessBase, IStatisticsEducationDAL
    {
        private readonly IClassDAL _classDAL;

        public StatisticsEducationDAL(IDbWrapper dbWrapper, IClassDAL classDAL) : base(dbWrapper)
        {
            this._classDAL = classDAL;
        }

        public override void InitTenantId(int tenantId)
        {
            base.InitTenantId(tenantId);
            this._classDAL.InitTenantId(tenantId);
        }

        public override void ResetTenantId(int tenantId)
        {
            base.ResetTenantId(tenantId);
            this._classDAL.ResetTenantId(tenantId);
        }

        public async Task StatisticsEducationUpdate(DateTime time)
        {
            var firstDate = new DateTime(time.Year, time.Month, 1);
            var startTimeDesc = firstDate.EtmsToDateString();
            var endTimeDesc = firstDate.AddMonths(1).EtmsToDateString();
            await StatisticsEducationClassAndTeacherAndCourse(firstDate, startTimeDesc, endTimeDesc);
            await StatisticsEducationStudent(firstDate, startTimeDesc, endTimeDesc);
        }

        private async Task DelOldLogsStatisticsEducationClassAndTeacherAndCourse(DateTime firstDate, string startTimeDesc, string endTimeDesc)
        {
            try
            {
                await _dbWrapper.Execute($"DELETE EtStatisticsEducationMonth WHERE TenantId = {_tenantId} AND IsDeleted = {EmIsDeleted.Normal} AND Ot = '{startTimeDesc}'");
                await _dbWrapper.Execute($"DELETE EtStatisticsEducationClassMonth WHERE TenantId = {_tenantId} AND IsDeleted = {EmIsDeleted.Normal} AND Ot = '{startTimeDesc}'");
                await _dbWrapper.Execute($"DELETE EtStatisticsEducationTeacherMonth WHERE TenantId = {_tenantId} AND IsDeleted = {EmIsDeleted.Normal} AND Ot = '{startTimeDesc}'");
                await _dbWrapper.Execute($"DELETE EtStatisticsEducationCourseMonth WHERE TenantId = {_tenantId} AND IsDeleted = {EmIsDeleted.Normal} AND Ot = '{startTimeDesc}'");
            }
            catch (Exception ex)
            {
                LOG.Log.Fatal("[StatisticsEducationDAL]DelOldLogsStatisticsEducationClassAndTeacherAndCourse错误", ex, this.GetType());
                await _dbWrapper.Execute($"UPDATE EtStatisticsEducationMonth SET IsDeleted = {EmIsDeleted.Deleted} WHERE TenantId = {_tenantId} AND IsDeleted = {EmIsDeleted.Normal} AND Ot = '{startTimeDesc}'");
                await _dbWrapper.Execute($"UPDATE EtStatisticsEducationClassMonth SET IsDeleted = {EmIsDeleted.Deleted}  WHERE TenantId = {_tenantId} AND IsDeleted = {EmIsDeleted.Normal} AND Ot = '{startTimeDesc}'");
                await _dbWrapper.Execute($"UPDATE EtStatisticsEducationTeacherMonth SET IsDeleted = {EmIsDeleted.Deleted}  WHERE TenantId = {_tenantId} AND IsDeleted = {EmIsDeleted.Normal} AND Ot = '{startTimeDesc}'");
                await _dbWrapper.Execute($"UPDATE EtStatisticsEducationCourseMonth SET IsDeleted = {EmIsDeleted.Deleted}  WHERE TenantId = {_tenantId} AND IsDeleted = {EmIsDeleted.Normal} AND Ot = '{startTimeDesc}'");
            }
        }

        private async Task StatisticsEducationClassAndTeacherAndCourse(DateTime firstDate, string startTimeDesc, string endTimeDesc)
        {
            await DelOldLogsStatisticsEducationClassAndTeacherAndCourse(firstDate, startTimeDesc, endTimeDesc);
            var sql = $"SELECT TOP 4000 ClassId,Teachers,CourseList,SUM(ClassTimes) AS TotalClassTimes,SUM(DeSum) as TotalDeSum,Count(Id) as TotalCount,SUM(NeedAttendNumber) AS TotalNeedAttendNumber,SUM(AttendNumber) AS TotalAttendNumber FROM EtClassRecord WHERE TenantId = {_tenantId} AND IsDeleted = {EmIsDeleted.Normal} AND ClassOt >= '{startTimeDesc}' AND ClassOt < '{endTimeDesc}' AND [Status] = {EmClassRecordStatus.Normal} GROUP BY ClassId,Teachers,CourseList";
            var obj = await _dbWrapper.ExecuteObject<StatisticsEducationClassAndTeacher>(sql);
            if (obj.Any())
            {
                var tempClassList = new List<EtClass>();
                var statisticsEducationMonth = new EtStatisticsEducationMonth()
                {
                    IsDeleted = EmIsDeleted.Normal,
                    Month = firstDate.Month,
                    Ot = firstDate,
                    Year = firstDate.Year,
                    TenantId = _tenantId,
                    Attendance = 0
                };
                var statisticsEducationClassMonths = new List<EtStatisticsEducationClassMonth>();
                var statisticsEducationTeacherMonths = new List<EtStatisticsEducationTeacherMonth>();
                var statisticsEducationCourseMonth = new List<EtStatisticsEducationCourseMonth>();
                foreach (var p in obj)
                {
                    long? classCategoryId = null;
                    var myClassInfoLog = tempClassList.FirstOrDefault(j => j.Id == p.ClassId);
                    if (myClassInfoLog == null)
                    {
                        var myClassBucket = await _classDAL.GetClassBucket(p.ClassId);
                        if (myClassBucket != null && myClassBucket.EtClass != null)
                        {
                            tempClassList.Add(myClassBucket.EtClass);
                            classCategoryId = myClassBucket.EtClass.ClassCategoryId;
                        }
                    }
                    else
                    {
                        classCategoryId = myClassInfoLog.ClassCategoryId;
                    }

                    statisticsEducationMonth.TeacherTotalClassTimes += p.TotalClassTimes;
                    statisticsEducationMonth.TeacherTotalClassCount += p.TotalCount;
                    statisticsEducationMonth.TotalDeSum += p.TotalDeSum;
                    statisticsEducationMonth.AttendNumber += p.TotalAttendNumber;
                    statisticsEducationMonth.NeedAttendNumber += p.TotalNeedAttendNumber;

                    //Class
                    var myClassLog = statisticsEducationClassMonths.FirstOrDefault(j => j.ClassId == p.ClassId);
                    if (myClassLog != null)
                    {
                        myClassLog.TeacherTotalClassTimes += p.TotalClassTimes;
                        myClassLog.TeacherTotalClassCount += p.TotalCount;
                        myClassLog.TotalDeSum += p.TotalDeSum;
                        myClassLog.AttendNumber += p.TotalAttendNumber;
                        myClassLog.NeedAttendNumber += p.TotalNeedAttendNumber;
                    }
                    else
                    {
                        statisticsEducationClassMonths.Add(new EtStatisticsEducationClassMonth()
                        {
                            IsDeleted = EmIsDeleted.Normal,
                            Month = firstDate.Month,
                            Ot = firstDate,
                            Year = firstDate.Year,
                            TenantId = _tenantId,
                            Attendance = 0,
                            ClassId = p.ClassId,
                            AttendNumber = p.TotalAttendNumber,
                            NeedAttendNumber = p.TotalNeedAttendNumber,
                            TeacherTotalClassCount = p.TotalCount,
                            TeacherTotalClassTimes = p.TotalClassTimes,
                            TotalDeSum = p.TotalDeSum,
                            ClassCategoryId = classCategoryId
                        });
                    }

                    //Teacher
                    var teachers = EtmsHelper.AnalyzeMuIds(p.Teachers);
                    if (teachers.Count > 0)
                    {
                        foreach (var myid in teachers)
                        {
                            var myTeacherLog = statisticsEducationTeacherMonths.FirstOrDefault(j => j.TeacherId == myid && j.ClassId == p.ClassId);
                            if (myTeacherLog != null)
                            {
                                myTeacherLog.TeacherTotalClassTimes += p.TotalClassTimes;
                                myTeacherLog.TeacherTotalClassCount += p.TotalCount;
                                myTeacherLog.TotalDeSum += p.TotalDeSum;
                                myTeacherLog.AttendNumber += p.TotalAttendNumber;
                                myTeacherLog.NeedAttendNumber += p.TotalNeedAttendNumber;
                            }
                            else
                            {
                                statisticsEducationTeacherMonths.Add(new EtStatisticsEducationTeacherMonth()
                                {
                                    IsDeleted = EmIsDeleted.Normal,
                                    Month = firstDate.Month,
                                    Ot = firstDate,
                                    Year = firstDate.Year,
                                    TenantId = _tenantId,
                                    Attendance = 0,
                                    ClassId = p.ClassId,
                                    TeacherId = myid,
                                    AttendNumber = p.TotalAttendNumber,
                                    NeedAttendNumber = p.TotalNeedAttendNumber,
                                    TeacherTotalClassCount = p.TotalCount,
                                    TeacherTotalClassTimes = p.TotalClassTimes,
                                    TotalDeSum = p.TotalDeSum,
                                    ClassCategoryId = classCategoryId
                                });
                            }
                        }
                    }

                    //Course
                    var courses = EtmsHelper.AnalyzeMuIds(p.CourseList);
                    if (courses.Count > 0)
                    {
                        foreach (var myCourseId in courses)
                        {
                            var myCourseLog = statisticsEducationCourseMonth.FirstOrDefault(j => j.CourseId == myCourseId);
                            if (myCourseLog != null)
                            {
                                myCourseLog.TeacherTotalClassTimes += p.TotalClassTimes;
                                myCourseLog.TeacherTotalClassCount += p.TotalCount;
                                myCourseLog.TotalDeSum += p.TotalDeSum;
                                myCourseLog.AttendNumber += p.TotalAttendNumber;
                                myCourseLog.NeedAttendNumber += p.TotalNeedAttendNumber;
                            }
                            else
                            {
                                statisticsEducationCourseMonth.Add(new EtStatisticsEducationCourseMonth()
                                {
                                    IsDeleted = EmIsDeleted.Normal,
                                    Month = firstDate.Month,
                                    Ot = firstDate,
                                    Year = firstDate.Year,
                                    TenantId = _tenantId,
                                    Attendance = 0,
                                    AttendNumber = p.TotalAttendNumber,
                                    NeedAttendNumber = p.TotalNeedAttendNumber,
                                    TeacherTotalClassCount = p.TotalCount,
                                    TeacherTotalClassTimes = p.TotalClassTimes,
                                    TotalDeSum = p.TotalDeSum,
                                    CourseId = myCourseId
                                });
                            }
                        }
                    }
                }

                await _dbWrapper.Insert(statisticsEducationMonth);
                if (statisticsEducationClassMonths.Count > 0)
                {
                    _dbWrapper.InsertRange(statisticsEducationClassMonths);
                }
                if (statisticsEducationTeacherMonths.Count > 0)
                {
                    _dbWrapper.InsertRange(statisticsEducationTeacherMonths);
                }
                if (statisticsEducationCourseMonth.Count > 0)
                {
                    _dbWrapper.InsertRange(statisticsEducationCourseMonth);
                }
            }
        }

        private async Task DelOldLogsStatisticsEducationStudent(DateTime firstDate, string startTimeDesc, string endTimeDesc)
        {
            try
            {
                await _dbWrapper.Execute($"DELETE EtStatisticsEducationStudentMonth WHERE TenantId = {_tenantId} AND IsDeleted = {EmIsDeleted.Normal} AND Ot = '{startTimeDesc}'");
            }
            catch (Exception ex)
            {
                LOG.Log.Fatal("[StatisticsEducationDAL]DelOldLogsStatisticsEducationStudent错误", ex, this.GetType());
                await _dbWrapper.Execute($"UPDATE EtStatisticsEducationStudentMonth SET IsDeleted = {EmIsDeleted.Deleted} WHERE TenantId = {_tenantId} AND IsDeleted = {EmIsDeleted.Normal} AND Ot = '{startTimeDesc}'");
            }
        }

        private async Task StatisticsEducationStudent(DateTime firstDate, string startTimeDesc, string endTimeDesc)
        {
            await DelOldLogsStatisticsEducationStudent(firstDate, startTimeDesc, endTimeDesc);
            var sql1 = $"SELECT TOP 3000 StudentId,COUNT(Id) AS TotalCount,SUM(DeClassTimes) AS TotalDeClassTimes,SUM(DeSum) AS TotalDeSum FROM EtClassRecordStudent WHERE TenantId = {_tenantId} AND IsDeleted = {EmIsDeleted.Normal} AND ClassOt >= '{startTimeDesc}' AND ClassOt < '{endTimeDesc}' AND [Status] = {EmClassRecordStatus.Normal} GROUP BY StudentId";
            var sql2 = $"SELECT TOP 3000 StudentId,StudentCheckStatus,Count(StudentCheckStatus) as TotalCount FROM EtClassRecordStudent WHERE TenantId = {_tenantId} AND IsDeleted = {EmIsDeleted.Normal} AND ClassOt >= '{startTimeDesc}' AND ClassOt <= '{endTimeDesc}' AND [Status] = {EmClassRecordStatus.Normal} AND StudentCheckStatus <> {EmClassStudentCheckStatus.Arrived} GROUP BY StudentId,StudentCheckStatus";
            var statisticsEducationStudent = await _dbWrapper.ExecuteObject<StatisticsEducationStudentView>(sql1);
            var statisticsEducationStudentStudentCheckStatus = await _dbWrapper.ExecuteObject<StatisticsEducationStudentCheckStatusView>(sql2);
            if (statisticsEducationStudent.Any())
            {
                var statisticsEducationStudentMonths = new List<EtStatisticsEducationStudentMonth>();
                foreach (var p in statisticsEducationStudent)
                {
                    var beLateCount = 0;
                    var leaveCount = 0;
                    var notArrivedCount = 0;
                    if (statisticsEducationStudentStudentCheckStatus.Any())
                    {
                        var myCheckStatus = statisticsEducationStudentStudentCheckStatus.Where(j => j.StudentId == p.StudentId);
                        if (myCheckStatus.Any())
                        {
                            var myCheckStatusBeLate = myCheckStatus.FirstOrDefault(j => j.StudentCheckStatus == EmClassStudentCheckStatus.BeLate);
                            if (myCheckStatusBeLate != null)
                            {
                                beLateCount = myCheckStatusBeLate.TotalCount;
                            }
                            var myCheckStatusLeave = myCheckStatus.FirstOrDefault(j => j.StudentCheckStatus == EmClassStudentCheckStatus.Leave);
                            if (myCheckStatusLeave != null)
                            {
                                leaveCount = myCheckStatusLeave.TotalCount;
                            }
                            var myCheckStatusNotArrived = myCheckStatus.FirstOrDefault(j => j.StudentCheckStatus == EmClassStudentCheckStatus.NotArrived);
                            if (myCheckStatusNotArrived != null)
                            {
                                notArrivedCount = myCheckStatusNotArrived.TotalCount;
                            }
                        }
                    }
                    statisticsEducationStudentMonths.Add(new EtStatisticsEducationStudentMonth()
                    {
                        IsDeleted = EmIsDeleted.Normal,
                        Month = firstDate.Month,
                        Ot = firstDate,
                        Year = firstDate.Year,
                        TenantId = _tenantId,
                        TotalDeSum = p.TotalDeSum,
                        TeacherTotalClassCount = p.TotalCount,
                        TeacherTotalClassTimes = p.TotalDeClassTimes,
                        StudentId = p.StudentId,
                        BeLateCount = beLateCount,
                        LeaveCount = leaveCount,
                        NotArrivedCount = notArrivedCount
                    });
                }

                if (statisticsEducationStudentMonths.Count > 0)
                {
                    this._dbWrapper.InsertRange(statisticsEducationStudentMonths);
                }
            }
        }

        public async Task<EtStatisticsEducationMonth> GetStatisticsEducationMonth(DateTime firstDate)
        {
            return await _dbWrapper.Find<EtStatisticsEducationMonth>(p => p.TenantId == _tenantId && p.IsDeleted == EmIsDeleted.Normal && p.Ot == firstDate);
        }

        public async Task<Tuple<IEnumerable<EtStatisticsEducationTeacherMonth>, int>> GetEtStatisticsEducationTeacherMonthPaging(RequestPagingBase request)
        {
            return await _dbWrapper.ExecutePage<EtStatisticsEducationTeacherMonth>("EtStatisticsEducationTeacherMonth", "*", request.PageSize, request.PageCurrent, "[TeacherTotalClassTimes] DESC", request.ToString());
        }

        public async Task<Tuple<IEnumerable<EtStatisticsEducationClassMonth>, int>> GetEtStatisticsEducationClassMonthPaging(RequestPagingBase request)
        {
            return await _dbWrapper.ExecutePage<EtStatisticsEducationClassMonth>("EtStatisticsEducationClassMonth", "*", request.PageSize, request.PageCurrent, "[TeacherTotalClassTimes] DESC", request.ToString());
        }

        public async Task<Tuple<IEnumerable<EtStatisticsEducationCourseMonth>, int>> GetEtStatisticsEducationCourseMonthPaging(RequestPagingBase request)
        {
            return await _dbWrapper.ExecutePage<EtStatisticsEducationCourseMonth>("EtStatisticsEducationCourseMonth", "*", request.PageSize, request.PageCurrent, "[TeacherTotalClassTimes] DESC", request.ToString());
        }

        public async Task<Tuple<IEnumerable<EtStatisticsEducationStudentMonth>, int>> GetEtStatisticsEducationStudentMonthPaging(RequestPagingBase request)
        {
            return await _dbWrapper.ExecutePage<EtStatisticsEducationStudentMonth>("EtStatisticsEducationStudentMonth", "*", request.PageSize, request.PageCurrent, "[TeacherTotalClassTimes] DESC", request.ToString());
        }

        public async Task SyncClassCategoryId(long classId, long? classCategoryId)
        {
            var sql = new StringBuilder();
            if (classCategoryId == null)
            {
                sql.Append($"UPDATE EtStatisticsEducationClassMonth SET ClassCategoryId = NULL WHERE TenantId = {_tenantId} AND ClassId = {classId} AND IsDeleted = {EmIsDeleted.Normal};");
                sql.Append($"UPDATE EtStatisticsEducationTeacherMonth SET ClassCategoryId = NULL WHERE TenantId = {_tenantId} AND ClassId = {classId} AND IsDeleted = {EmIsDeleted.Normal};");
            }
            else
            {
                sql.Append($"UPDATE EtStatisticsEducationClassMonth SET ClassCategoryId = {classCategoryId.Value} WHERE TenantId = {_tenantId} AND ClassId = {classId} AND IsDeleted = {EmIsDeleted.Normal};");
                sql.Append($"UPDATE EtStatisticsEducationTeacherMonth SET ClassCategoryId = {classCategoryId.Value} WHERE TenantId = {_tenantId} AND ClassId = {classId} AND IsDeleted = {EmIsDeleted.Normal};");
            }
            await _dbWrapper.Execute(sql.ToString());
        }
    }
}
