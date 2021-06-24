using ETMS.Business.Common;
using ETMS.Entity.Common;
using ETMS.Entity.Database.Source;
using ETMS.Entity.Dto.HisData.Output;
using ETMS.Entity.Dto.HisData.Request;
using ETMS.Entity.Enum;
using ETMS.Event.DataContract;
using ETMS.IBusiness;
using ETMS.IDataAccess;
using ETMS.IDataAccess.Statistics;
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

        private readonly IStatisticsEducationDAL _statisticsEducationDAL;

        private readonly IClassDAL _classDAL;

        private readonly IStudentDAL _studentDAL;

        public StatisticsClassBLL(IStatisticsClassDAL statisticsClassDAL, ICourseDAL courseDAL, IUserDAL userDAL,
            IStatisticsClassAttendanceTagDAL statisticsClassAttendanceTagDAL, IStatisticsEducationDAL statisticsEducationDAL,
            IClassDAL classDAL, IStudentDAL studentDAL)
        {
            this._statisticsClassDAL = statisticsClassDAL;
            this._courseDAL = courseDAL;
            this._userDAL = userDAL;
            this._statisticsClassAttendanceTagDAL = statisticsClassAttendanceTagDAL;
            this._statisticsEducationDAL = statisticsEducationDAL;
            this._classDAL = classDAL;
            this._studentDAL = studentDAL;
        }

        public void InitTenantId(int tenantId)
        {
            this.InitDataAccess(tenantId, _statisticsClassDAL, _courseDAL, _userDAL,
                _statisticsClassAttendanceTagDAL, _statisticsEducationDAL, _classDAL, _studentDAL);
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
                Attendance = Math.Round(attendance, 2)
            });
        }

        private async Task StatisticsClassTimesHandle(DateTime ot, decimal addClassTimes, decimal addDeSum)
        {
            await _statisticsClassDAL.StatisticsClassTimesSave(ot, addClassTimes, addDeSum);
        }

        private async Task StatisticsClassCourseHandle(DateTime ot, decimal classTimes, string classRecordCourses)
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

        private async Task StatisticsClassTeacherHandle(DateTime ot, decimal classTimes, string classRecordTeachers)
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
                tempData.Add(Math.Round(p.Attendance, 2));
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
            var echartsBar = new EchartsBar<string>();
            while (currentDate <= endDate)
            {
                var myStatisticsClassTime = statisticsClassTimes.FirstOrDefault(p => p.Ot == currentDate);
                echartsBar.XData.Add(currentDate.ToString("MM-dd"));
                echartsBar.MyData.Add(myStatisticsClassTime == null ? "0" : myStatisticsClassTime.ClassTimes.EtmsToString());
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
            var echartsBarVerticalOutput = new EchartsBarVertical<string>();
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
                    echartsBarVerticalOutput.XData.Add(item.TotalClassTimes.EtmsToString());
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
            var echartsBarVerticalOutput = new EchartsBarVertical<string>();
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
                    echartsBarVerticalOutput.XData.Add(item.TotalClassTimes.EtmsToString());
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

        public async Task<ResponseBase> StatisticsEducationMonthGet(StatisticsEducationMonthGetRequest request)
        {
            var firstDate = new DateTime(request.Year, request.Month, 1);
            var log = await _statisticsEducationDAL.GetStatisticsEducationMonth(firstDate);
            if (log == null)
            {
                return ResponseBase.Success(new StatisticsEducationMonthGetOutput());
            }
            decimal attendance = 0;
            if (log.NeedAttendNumber > 0 && log.AttendNumber > 0)
            {
                attendance = log.AttendNumber / (decimal)log.NeedAttendNumber;
            }
            return ResponseBase.Success(new StatisticsEducationMonthGetOutput()
            {
                TotalDeSum = log.TotalDeSum,
                TeacherTotalClassTimes = log.TeacherTotalClassTimes,
                TeacherTotalClassCount = log.TeacherTotalClassCount,
                NeedAttendNumber = log.NeedAttendNumber,
                AttendNumber = log.AttendNumber,
                Attendance = attendance.EtmsPercentage()
            }); ;
        }

        public async Task<ResponseBase> StatisticsEducationTeacherMonthGetPaging(StatisticsEducationTeacherMonthGetPagingRequest request)
        {
            var pagingData = await _statisticsEducationDAL.GetEtStatisticsEducationTeacherMonthPaging(request);
            var output = new List<StatisticsEducationTeacherMonthGetPagingOutput>();
            var tempBoxUser = new DataTempBox<EtUser>();
            var tempBoxClass = new DataTempBox<EtClass>();
            if (pagingData.Item1.Any())
            {
                foreach (var p in pagingData.Item1)
                {
                    var className = string.Empty;
                    var teacherName = string.Empty;
                    decimal attendance = 0;
                    if (p.NeedAttendNumber > 0 && p.AttendNumber > 0)
                    {
                        attendance = p.AttendNumber / (decimal)p.NeedAttendNumber;
                    }
                    var myClass = await ComBusiness.GetClass(tempBoxClass, _classDAL, p.ClassId);
                    if (myClass != null)
                    {
                        className = myClass.Name;
                    }
                    var myTeacher = await ComBusiness.GetUser(tempBoxUser, _userDAL, p.TeacherId);
                    if (myTeacher != null)
                    {
                        teacherName = myTeacher.Name;
                    }
                    output.Add(new StatisticsEducationTeacherMonthGetPagingOutput()
                    {
                        TeacherId = p.TeacherId,
                        Attendance = attendance.EtmsPercentage(),
                        AttendNumber = p.AttendNumber,
                        ClassId = p.ClassId,
                        ClassName = className,
                        NeedAttendNumber = p.NeedAttendNumber,
                        TeacherName = teacherName,
                        TeacherTotalClassCount = p.TeacherTotalClassCount,
                        TeacherTotalClassTimes = p.TeacherTotalClassTimes,
                        TotalDeSum = p.TotalDeSum
                    });
                }
            }
            return ResponseBase.Success(new ResponsePagingDataBase<StatisticsEducationTeacherMonthGetPagingOutput>(pagingData.Item2, output));
        }

        public async Task<ResponseBase> StatisticsEducationClassMonthGetPaging(StatisticsEducationClassMonthGetPagingRequest request)
        {
            var pagingData = await _statisticsEducationDAL.GetEtStatisticsEducationClassMonthPaging(request);
            var output = new List<StatisticsEducationClassMonthGetPagingOutput>();
            var tempBoxClass = new DataTempBox<EtClass>();
            if (pagingData.Item1.Any())
            {
                foreach (var p in pagingData.Item1)
                {
                    decimal attendance = 0;
                    if (p.NeedAttendNumber > 0 && p.AttendNumber > 0)
                    {
                        attendance = p.AttendNumber / (decimal)p.NeedAttendNumber;
                    }
                    var className = string.Empty;
                    var myClass = await ComBusiness.GetClass(tempBoxClass, _classDAL, p.ClassId);
                    if (myClass != null)
                    {
                        className = myClass.Name;
                    }
                    output.Add(new StatisticsEducationClassMonthGetPagingOutput()
                    {
                        TotalDeSum = p.TotalDeSum,
                        Attendance = attendance.EtmsPercentage(),
                        AttendNumber = p.AttendNumber,
                        ClassId = p.ClassId,
                        ClassName = className,
                        NeedAttendNumber = p.NeedAttendNumber,
                        TeacherTotalClassCount = p.TeacherTotalClassCount,
                        TeacherTotalClassTimes = p.TeacherTotalClassTimes
                    });
                }
            }
            return ResponseBase.Success(new ResponsePagingDataBase<StatisticsEducationClassMonthGetPagingOutput>(pagingData.Item2, output));
        }

        public async Task<ResponseBase> StatisticsEducationCourseMonthGetPaging(StatisticsEducationCourseMonthGetPagingRequest request)
        {
            var pagingData = await _statisticsEducationDAL.GetEtStatisticsEducationCourseMonthPaging(request);
            var output = new List<StatisticsEducationCourseMonthGetPagingOutput>();
            var tempBoxCourse = new DataTempBox<EtCourse>();
            if (pagingData.Item1.Any())
            {
                foreach (var p in pagingData.Item1)
                {
                    decimal attendance = 0;
                    if (p.NeedAttendNumber > 0 && p.AttendNumber > 0)
                    {
                        attendance = p.AttendNumber / (decimal)p.NeedAttendNumber;
                    }
                    var courseName = string.Empty;
                    var myCourse = await ComBusiness.GetCourse(tempBoxCourse, _courseDAL, p.CourseId);
                    if (myCourse != null)
                    {
                        courseName = myCourse.Name;
                    }
                    output.Add(new StatisticsEducationCourseMonthGetPagingOutput()
                    {
                        CourseId = p.CourseId,
                        Attendance = attendance.EtmsPercentage(),
                        AttendNumber = p.AttendNumber,
                        CourseName = courseName,
                        NeedAttendNumber = p.NeedAttendNumber,
                        TeacherTotalClassCount = p.TeacherTotalClassCount,
                        TeacherTotalClassTimes = p.TeacherTotalClassTimes,
                        TotalDeSum = p.TotalDeSum
                    });
                }
            }
            return ResponseBase.Success(new ResponsePagingDataBase<StatisticsEducationCourseMonthGetPagingOutput>(pagingData.Item2, output));
        }

        public async Task<ResponseBase> StatisticsEducationStudentMonthGetPaging(StatisticsEducationStudentMonthGetPagingRequest request)
        {
            var pagingData = await _statisticsEducationDAL.GetEtStatisticsEducationStudentMonthPaging(request);
            var output = new List<StatisticsEducationStudentMonthGetPagingOutput>();
            var tempBoxStudent = new DataTempBox<EtStudent>();
            if (pagingData.Item1.Any())
            {
                foreach (var p in pagingData.Item1)
                {
                    var studentName = string.Empty;
                    var student = await ComBusiness.GetStudent(tempBoxStudent, _studentDAL, p.StudentId);
                    if (student != null)
                    {
                        studentName = student.Name;
                    }
                    output.Add(new StatisticsEducationStudentMonthGetPagingOutput()
                    {
                        StudentId = p.StudentId,
                        BeLateCount = p.BeLateCount,
                        LeaveCount = p.LeaveCount,
                        NotArrivedCount = p.NotArrivedCount,
                        StudentName = studentName,
                        TeacherTotalClassCount = p.TeacherTotalClassCount,
                        TeacherTotalClassTimes = p.TeacherTotalClassTimes,
                        TotalDeSum = p.TotalDeSum
                    });
                }
            }
            return ResponseBase.Success(new ResponsePagingDataBase<StatisticsEducationStudentMonthGetPagingOutput>(pagingData.Item2, output));
        }
    }
}
