using ETMS.Business.Common;
using ETMS.Entity.Common;
using ETMS.Entity.Database.Source;
using ETMS.Entity.Dto.HisData.Output;
using ETMS.Entity.Dto.HisData.Request;
using ETMS.Entity.Enum;
using ETMS.Event.DataContract;
using ETMS.IBusiness;
using ETMS.IDataAccess;
using ETMS.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.Business
{
    public class StatisticsClassBLL : IStatisticsClassBLL
    {
        private readonly IStatisticsClassDAL _statisticsClassDAL;

        private readonly ICourseDAL _courseDAL;

        private readonly IUserDAL _userDAL;

        private readonly IStatisticsClassAttendanceTagDAL _statisticsClassAttendanceTagDAL;
        public StatisticsClassBLL(IStatisticsClassDAL statisticsClassDAL, ICourseDAL courseDAL, IUserDAL userDAL, IStatisticsClassAttendanceTagDAL statisticsClassAttendanceTagDAL)
        {
            this._statisticsClassDAL = statisticsClassDAL;
            this._courseDAL = courseDAL;
            this._userDAL = userDAL;
            this._statisticsClassAttendanceTagDAL = statisticsClassAttendanceTagDAL;
        }

        public void InitTenantId(int tenantId)
        {
            this.InitDataAccess(tenantId, _statisticsClassDAL, _courseDAL, _userDAL, _statisticsClassAttendanceTagDAL);
        }

        public async Task StatisticsClassConsumeEvent(StatisticsClassEvent request)
        {
            request.ClassRecord.Id = request.RecordId;
            var ot = request.ClassRecord.ClassOt.Date;
            await StatisticsClassAttendanceHandle(ot, request.ClassRecord);
            await StatisticsClassTimesHandle(ot, request.ClassRecord.ClassTimes, request.ClassRecord.DeSum);
            await StatisticsClassCourseHandle(ot, request.ClassRecord.ClassTimes, request.ClassRecord.CourseList);
            await StatisticsClassTeacherHandle(ot, request.ClassRecord.ClassTimes, request.ClassRecord.Teachers);
            await StatisticsClassAttendanceTag(ot);
        }

        public async Task StatisticsClassRevokeConsumeEvent(StatisticsClassRevokeEvent request)
        {
            await _statisticsClassDAL.StatisticsClassAttendanceDel(request.ClassRecord.Id);
            await _statisticsClassDAL.StatisticsClassTimesDeduction(request.ClassRecord.ClassOt, request.ClassRecord.ClassTimes, request.ClassRecord.DeSum);
            var myCourses = request.ClassRecord.CourseList.Split(',');
            foreach (var p in myCourses)
            {
                if (string.IsNullOrEmpty(p))
                {
                    continue;
                }
                await _statisticsClassDAL.StatisticsClassCourseDeduction(request.ClassRecord.ClassOt, p.ToLong(), request.ClassRecord.ClassTimes);
            }
            var myTeachers = request.ClassRecord.Teachers.Split(',');
            foreach (var p in myTeachers)
            {
                if (string.IsNullOrEmpty(p))
                {
                    continue;
                }
                await _statisticsClassDAL.StatisticsClassTeacherDeduction(request.ClassRecord.ClassOt, p.ToLong(), request.ClassRecord.ClassTimes);
            }
            await this._statisticsClassAttendanceTagDAL.UpdateStatisticsClassAttendanceTag(request.ClassRecord.ClassOt);
        }

        private async Task StatisticsClassAttendanceHandle(DateTime ot, EtClassRecord classRecord)
        {
            decimal attendance = 0;
            if (classRecord.NeedAttendNumber > 0 && classRecord.AttendNumber > 0)
            {
                attendance = classRecord.AttendNumber / (decimal)classRecord.NeedAttendNumber;
            }
            await _statisticsClassDAL.StatisticsClassAttendanceAdd(new EtStatisticsClassAttendance()
            {
                ClassRecordId = classRecord.Id,
                AttendNumber = classRecord.AttendNumber,
                IsDeleted = EmIsDeleted.Normal,
                NeedAttendNumber = classRecord.NeedAttendNumber,
                Ot = ot,
                TenantId = classRecord.TenantId,
                Day = ot.Day,
                StartTime = classRecord.StartTime,
                Attendance = attendance
            });
        }

        private async Task StatisticsClassTimesHandle(DateTime ot, int addClassTimes, decimal addDeSum)
        {
            await _statisticsClassDAL.StatisticsClassTimesSave(ot, addClassTimes, addDeSum);
        }

        private async Task StatisticsClassCourseHandle(DateTime ot, int classTimes, string classRecordCourses)
        {
            var myCourses = classRecordCourses.Split(',');
            foreach (var p in myCourses)
            {
                if (string.IsNullOrEmpty(p))
                {
                    continue;
                }
                await _statisticsClassDAL.StatisticsClassCourseSave(ot, p.ToLong(), classTimes);
            }
        }

        private async Task StatisticsClassTeacherHandle(DateTime ot, int classTimes, string classRecordTeachers)
        {
            var myTeachers = classRecordTeachers.Split(',');
            foreach (var p in myTeachers)
            {
                if (string.IsNullOrEmpty(p))
                {
                    continue;
                }
                await _statisticsClassDAL.StatisticsClassTeacherSave(ot, p.ToLong(), classTimes);
            }
        }

        private async Task StatisticsClassAttendanceTag(DateTime ot)
        {
            await this._statisticsClassAttendanceTagDAL.UpdateStatisticsClassAttendanceTag(ot);
        }

        public async Task<ResponseBase> StatisticsClassAttendanceGet(StatisticsClassAttendanceRequest request)
        {
            var otAnalyze = request.Ot.Split("-");
            var startOt = new DateTime(otAnalyze[0].ToInt(), otAnalyze[1].ToInt(), 1);
            var endOt = startOt.AddMonths(1).AddDays(-1);
            var myData = await _statisticsClassDAL.StatisticsClassAttendanceGet(startOt, endOt);
            var outputScatter = new EchartsScatter<decimal>();
            foreach (var p in myData)
            {
                var tempData = new List<decimal>();
                tempData.Add(Convert.ToDecimal($"{p.Day}.{p.StartTime}"));
                tempData.Add(p.Attendance);
                outputScatter.MyData.Add(tempData);
            }
            outputScatter.MyData.Add(new List<decimal>() { 31 });
            return ResponseBase.Success(outputScatter);
        }

        public async Task<ResponseBase> StatisticsClassTimesGet(StatisticsClassTimesGetRequest request)
        {
            var currentDate = request.StartOt.Value;
            var endDate = request.EndOt.Value;
            var statisticsClassTimes = await _statisticsClassDAL.StatisticsClassTimesGet(currentDate, endDate);
            var echartsBar = new EchartsBar<int>();
            while (currentDate <= endDate)
            {
                var myStatisticsClassTime = statisticsClassTimes.FirstOrDefault(p => p.Ot == currentDate);
                echartsBar.XData.Add(currentDate.ToString("MM-dd"));
                echartsBar.MyData.Add(myStatisticsClassTime == null ? 0 : myStatisticsClassTime.ClassTimes);
                currentDate = currentDate.AddDays(1);
            }
            return ResponseBase.Success(echartsBar);
        }

        public async Task<ResponseBase> StatisticsClassCourseGet(StatisticsClassCourseGetRequest request)
        {
            var currentDate = request.StartOt.Value;
            var endDate = request.EndOt.Value;
            var statisticsData = await _statisticsClassDAL.StatisticsClassCourseGet(currentDate, endDate, 20);
            statisticsData = statisticsData.OrderBy(p => p.TotalClassTimes);
            var echartsBarVerticalOutput = new EchartsBarVertical<int>();
            if (statisticsData != null && statisticsData.Any())
            {
                var tempBox = new DataTempBox<EtCourse>();
                foreach (var item in statisticsData)
                {
                    var courseName = await ComBusiness.GetCourseName(tempBox, _courseDAL, item.CourseId);
                    if (string.IsNullOrEmpty(courseName))
                    {
                        continue;
                    }
                    echartsBarVerticalOutput.YData.Add(courseName);
                    echartsBarVerticalOutput.XData.Add(item.TotalClassTimes);
                }
            }
            return ResponseBase.Success(echartsBarVerticalOutput);
        }

        public async Task<ResponseBase> StatisticsClassTeacherGet(StatisticsClassTeacherGetRequest request)
        {
            var currentDate = request.StartOt.Value;
            var endDate = request.EndOt.Value;
            var statisticsData = await _statisticsClassDAL.StatisticsClassTeacherGet(currentDate, endDate, 20);
            statisticsData = statisticsData.OrderBy(p => p.TotalClassTimes);
            var echartsBarVerticalOutput = new EchartsBarVertical<int>();
            if (statisticsData != null && statisticsData.Any())
            {
                var tempBox = new DataTempBox<EtUser>();
                foreach (var item in statisticsData)
                {
                    var teacherName = await ComBusiness.GetUserName(tempBox, _userDAL, item.TeacherId);
                    if (string.IsNullOrEmpty(teacherName))
                    {
                        continue;
                    }
                    echartsBarVerticalOutput.YData.Add(teacherName);
                    echartsBarVerticalOutput.XData.Add(item.TotalClassTimes);
                }
            }
            return ResponseBase.Success(echartsBarVerticalOutput);
        }

        public async Task<ResponseBase> StatisticsClassAttendanceTagGet(StatisticsClassAttendanceTagGetRequest request)
        {
            var statisticsClassAttendanceTag = await _statisticsClassAttendanceTagDAL.GetStatisticsClassAttendanceTag(request.StartOt.Value, request.EndOt.Value);
            var echartsPieClassAttendanceTag = new EchartsPie<int>();
            foreach (var p in statisticsClassAttendanceTag)
            {
                var tempName = EmClassStudentCheckStatus.GetClassStudentCheckStatus(p.StudentCheckStatus);
                if (string.IsNullOrEmpty(tempName))
                {
                    continue;
                }
                echartsPieClassAttendanceTag.LegendData.Add(tempName);
                echartsPieClassAttendanceTag.MyData.Add(new EchartsPieData<int>()
                {
                    Name = tempName,
                    Value = p.TotalCount
                });
            }
            return ResponseBase.Success(echartsPieClassAttendanceTag);
        }
    }
}
