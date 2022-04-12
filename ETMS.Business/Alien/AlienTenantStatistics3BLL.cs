using ETMS.Business.Common;
using ETMS.Entity.Alien.Dto.TenantStatistics.Output;
using ETMS.Entity.Alien.Dto.TenantStatistics.Request;
using ETMS.Entity.Common;
using ETMS.Entity.Database.Source;
using ETMS.Entity.Dto.HisData.Output;
using ETMS.Entity.Enum;
using ETMS.Entity.Enum.EtmsManage;
using ETMS.Entity.Temp;
using ETMS.Entity.View.Persistence;
using ETMS.IBusiness;
using ETMS.IBusiness.Alien;
using ETMS.IDataAccess;
using ETMS.IDataAccess.Statistics;
using ETMS.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.Business.Alien
{
    public class AlienTenantStatistics3BLL : IAlienTenantStatistics3BLL
    {
        private readonly IStatisticsClassDAL _statisticsClassDAL;

        private readonly ICourseDAL _courseDAL;

        private readonly IUserDAL _userDAL;

        private readonly IStatisticsClassAttendanceTagDAL _statisticsClassAttendanceTagDAL;

        private readonly IStatisticsEducationDAL _statisticsEducationDAL;

        private readonly IClassDAL _classDAL;

        private readonly IStudentDAL _studentDAL;

        private readonly IClassRecordDAL _classRecordDAL;

        private readonly IClassCategoryDAL _classCategoryDAL;

        private readonly IClassRoomDAL _classRoomDAL;
        public AlienTenantStatistics3BLL(IStatisticsClassDAL statisticsClassDAL, ICourseDAL courseDAL, IUserDAL userDAL,
            IStatisticsClassAttendanceTagDAL statisticsClassAttendanceTagDAL, IStatisticsEducationDAL statisticsEducationDAL,
            IClassDAL classDAL, IStudentDAL studentDAL, IClassRecordDAL classRecordDAL, IClassCategoryDAL classCategoryDAL,
            IClassRoomDAL classRoomDAL)
        {
            this._statisticsClassDAL = statisticsClassDAL;
            this._courseDAL = courseDAL;
            this._userDAL = userDAL;
            this._statisticsClassAttendanceTagDAL = statisticsClassAttendanceTagDAL;
            this._statisticsEducationDAL = statisticsEducationDAL;
            this._classDAL = classDAL;
            this._studentDAL = studentDAL;
            this._classRecordDAL = classRecordDAL;
            this._classCategoryDAL = classCategoryDAL;
            this._classRoomDAL = classRoomDAL;
        }

        public void InitHeadId(int headId)
        {
        }

        public void InitTenant(int tenantId)
        {
            this.InitTenantDataAccess(tenantId, _statisticsClassDAL, _courseDAL, _userDAL, _statisticsClassAttendanceTagDAL,
                _statisticsEducationDAL, _classDAL, _studentDAL, _classRecordDAL, _classCategoryDAL, _classRoomDAL);
        }

        public async Task<ResponseBase> AlienTenantStatisticsClassTimesGet(AlienTenantStatisticsClassTimesGetRequest request)
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

        public async Task<ResponseBase> AlienTenantStatisticsClassAttendanceTagGet(AlienTenantStatisticsClassAttendanceTagGetRequest request)
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

        public async Task<ResponseBase> AlienTenantStatisticsClassAttendanceGet(AlienTenantStatisticsClassAttendanceGetRequest request)
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

        public async Task<ResponseBase> AlienTenantStatisticsEducationMonthGet(AlienTenantStatisticsEducationMonthGetRequest request)
        {
            var firstDate = new DateTime(request.Year, request.Month, 1);
            var log = await _statisticsEducationDAL.GetStatisticsEducationMonth(firstDate);
            if (log == null)
            {
                return ResponseBase.Success(new AlienTenantStatisticsEducationMonthGetOutput());
            }
            decimal attendance = 0;
            if (log.NeedAttendNumber > 0 && log.AttendNumber > 0)
            {
                attendance = log.AttendNumber / (decimal)log.NeedAttendNumber;
            }
            return ResponseBase.Success(new AlienTenantStatisticsEducationMonthGetOutput()
            {
                TotalDeSum = log.TotalDeSum,
                TeacherTotalClassTimes = log.TeacherTotalClassTimes,
                TeacherTotalClassCount = log.TeacherTotalClassCount,
                NeedAttendNumber = log.NeedAttendNumber,
                AttendNumber = log.AttendNumber,
                Attendance = attendance.EtmsPercentage()
            }); ;
        }

        public async Task<ResponseBase> AlienTenantStatisticsEducationStudentMonthGetPaging(AlienTenantStatisticsEducationStudentMonthGetPagingRequest request)
        {
            var pagingData = await _statisticsEducationDAL.GetEtStatisticsEducationStudentMonthPaging(request);
            var output = new List<AlienTenantStatisticsEducationStudentMonthGetPagingOutput>();
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
                    output.Add(new AlienTenantStatisticsEducationStudentMonthGetPagingOutput()
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
            return ResponseBase.Success(new ResponsePagingDataBase<AlienTenantStatisticsEducationStudentMonthGetPagingOutput>(pagingData.Item2, output));
        }

        public async Task<ResponseBase> AlienTenantStatisticsEducationTeacherMonthGetPaging(AlienTenantStatisticsEducationTeacherMonthGetPagingRequest request)
        {
            var pagingData = await _statisticsEducationDAL.GetEtStatisticsEducationTeacherMonthPaging(request);
            var output = new List<AlienTenantStatisticsEducationTeacherMonthGetPagingOutput>();
            var tempBoxUser = new DataTempBox<EtUser>();
            var tempBoxClass = new DataTempBox<EtClass>();
            if (pagingData.Item1.Any())
            {
                var allClassCategory = await _classCategoryDAL.GetAllClassCategory();
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
                    var classCategoryName = string.Empty;
                    if (p.ClassCategoryId != null)
                    {
                        var myClassCategory = allClassCategory.FirstOrDefault(j => j.Id == p.ClassCategoryId);
                        if (myClassCategory != null)
                        {
                            classCategoryName = myClassCategory.Name;
                        }
                    }
                    output.Add(new AlienTenantStatisticsEducationTeacherMonthGetPagingOutput()
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
                        TotalDeSum = p.TotalDeSum,
                        ClassCategoryName = classCategoryName
                    });
                }
            }
            return ResponseBase.Success(new ResponsePagingDataBase<AlienTenantStatisticsEducationTeacherMonthGetPagingOutput>(pagingData.Item2, output));
        }

        public async Task<ResponseBase> AlienTenantStatisticsEducationCourseMonthGetPaging(AlienTenantStatisticsEducationCourseMonthGetPagingRequest request)
        {
            var pagingData = await _statisticsEducationDAL.GetEtStatisticsEducationCourseMonthPaging(request);
            var output = new List<AlienTenantStatisticsEducationCourseMonthGetPagingOutput>();
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
                    output.Add(new AlienTenantStatisticsEducationCourseMonthGetPagingOutput()
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
            return ResponseBase.Success(new ResponsePagingDataBase<AlienTenantStatisticsEducationCourseMonthGetPagingOutput>(pagingData.Item2, output));
        }

        public async Task<ResponseBase> AlienTenantStatisticsEducationClassMonthGetPaging(AlienTenantStatisticsEducationClassMonthGetPagingRequest request)
        {
            var pagingData = await _statisticsEducationDAL.GetEtStatisticsEducationClassMonthPaging(request);
            var output = new List<AlienTenantStatisticsEducationClassMonthGetPagingOutput>();
            var tempBoxClass = new DataTempBox<EtClass>();
            if (pagingData.Item1.Any())
            {
                var allClassCategory = await _classCategoryDAL.GetAllClassCategory();
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
                    var classCategoryName = string.Empty;
                    if (p.ClassCategoryId != null)
                    {
                        var myClassCategory = allClassCategory.FirstOrDefault(j => j.Id == p.ClassCategoryId);
                        if (myClassCategory != null)
                        {
                            classCategoryName = myClassCategory.Name;
                        }
                    }
                    output.Add(new AlienTenantStatisticsEducationClassMonthGetPagingOutput()
                    {
                        TotalDeSum = p.TotalDeSum,
                        Attendance = attendance.EtmsPercentage(),
                        AttendNumber = p.AttendNumber,
                        ClassId = p.ClassId,
                        ClassName = className,
                        NeedAttendNumber = p.NeedAttendNumber,
                        TeacherTotalClassCount = p.TeacherTotalClassCount,
                        TeacherTotalClassTimes = p.TeacherTotalClassTimes,
                        ClassCategoryName = classCategoryName
                    });
                }
            }
            return ResponseBase.Success(new ResponsePagingDataBase<AlienTenantStatisticsEducationClassMonthGetPagingOutput>(pagingData.Item2, output));
        }

        public async Task<ResponseBase> AlienTenantClassRecordGetPaging(AlienTenantClassRecordGetPagingRequest request)
        {
            var pagingData = await _classRecordDAL.GetPaging(request);
            var output = new List<AlienTenantClassRecordGetPagingOutput>();
            var allClassRoom = await _classRoomDAL.GetAllClassRoom();
            var tempBoxCourse = new DataTempBox<EtCourse>();
            var tempBoxUser = new DataTempBox<EtUser>();
            var tempBoxClass = new DataTempBox<EtClass>();
            if (pagingData.Item1.Any())
            {
                var allClassCategory = await _classCategoryDAL.GetAllClassCategory();
                foreach (var classRecord in pagingData.Item1)
                {
                    var classRoomIdsDesc = string.Empty;
                    var courseListDesc = string.Empty;
                    var courseStyleColor = string.Empty;
                    var className = string.Empty;
                    var teachersDesc = string.Empty;
                    var classCategoryName = string.Empty;
                    var etClass = await ComBusiness.GetClass(tempBoxClass, _classDAL, classRecord.ClassId);
                    var courseInfo = await ComBusiness.GetCourseNameAndColor(tempBoxCourse, _courseDAL, classRecord.CourseList);
                    classRoomIdsDesc = ComBusiness.GetDesc(allClassRoom, classRecord.ClassRoomIds);
                    className = etClass.Name;
                    courseListDesc = courseInfo.Item1;
                    courseStyleColor = courseInfo.Item2;
                    teachersDesc = await ComBusiness.GetUserNames(tempBoxUser, _userDAL, classRecord.Teachers);
                    if (classRecord.ClassCategoryId != null)
                    {
                        var myClassCategory = allClassCategory.FirstOrDefault(j => j.Id == classRecord.ClassCategoryId);
                        if (myClassCategory != null)
                        {
                            classCategoryName = myClassCategory.Name;
                        }
                    }
                    output.Add(new AlienTenantClassRecordGetPagingOutput()
                    {
                        CId = classRecord.Id,
                        ClassContent = classRecord.ClassContent,
                        ClassId = classRecord.ClassId,
                        ClassName = className,
                        ClassOtDesc = classRecord.ClassOt.EtmsToDateString(),
                        ClassTimeDesc = $"{EtmsHelper.GetTimeDesc(classRecord.StartTime)}~{EtmsHelper.GetTimeDesc(classRecord.EndTime)}",
                        ClassRoomIdsDesc = classRoomIdsDesc,
                        CourseListDesc = courseListDesc,
                        Status = classRecord.Status,
                        WeekDesc = $"周{EtmsHelper.GetWeekDesc(classRecord.Week)}",
                        TeachersDesc = teachersDesc,
                        AttendNumber = classRecord.AttendNumber,
                        CheckOt = classRecord.CheckOt,
                        CheckUserId = classRecord.CheckUserId,
                        NeedAttendNumber = classRecord.NeedAttendNumber,
                        ClassTimes = classRecord.ClassTimes.EtmsToString(),
                        DeSum = EmRoleSecrecyType.GetSecrecyValue(EmRoleSecrecyType.NotLimited, classRecord.DeSum.EtmsToString2()),
                        StatusDesc = EmClassRecordStatus.GetClassRecordStatusDesc(classRecord.Status),
                        CheckUserDesc = await ComBusiness.GetUserName(tempBoxUser, _userDAL, classRecord.CheckUserId),
                        ClassCategoryName = classCategoryName
                    });
                }
            }
            return ResponseBase.Success(new ResponsePagingDataBase<AlienTenantClassRecordGetPagingOutput>(pagingData.Item2, output));
        }

        public async Task<ResponseBase> AlienTenantClassRecordGet(AlienTenantClassRecordGetRequest request)
        {
            var classRecord = await _classRecordDAL.GetClassRecord(request.ClassRecordId);
            var etClass = await _classDAL.GetClassBucket(classRecord.ClassId);
            if (etClass == null || etClass.EtClass == null)
            {
                return ResponseBase.CommonError("班级不存在");
            }
            var tempBoxCourse = new DataTempBox<EtCourse>();
            var courseInfo = await ComBusiness.GetCourseNameAndColor(tempBoxCourse, _courseDAL, classRecord.CourseList);
            var classRoomIdsDesc = string.Empty;
            if (!string.IsNullOrEmpty(classRecord.ClassRoomIds))
            {
                var allClassRoom = await _classRoomDAL.GetAllClassRoom();
                classRoomIdsDesc = ComBusiness.GetDesc(allClassRoom, classRecord.ClassRoomIds);
            }
            var myClass = etClass.EtClass;
            var courseListDesc = courseInfo.Item1;
            var tempBoxUser = new DataTempBox<EtUser>();
            var teachersDesc = await ComBusiness.GetUserNames(tempBoxUser, _userDAL, classRecord.Teachers);
            return ResponseBase.Success(new AlienTenantClassRecordGetOutput()
            {
                CId = classRecord.Id,
                ClassContent = classRecord.ClassContent,
                ClassId = classRecord.ClassId,
                ClassName = myClass.Name,
                ClassOtDesc = classRecord.ClassOt.EtmsToDateString(),
                ClassTimeDesc = $"{EtmsHelper.GetTimeDesc(classRecord.StartTime)}~{EtmsHelper.GetTimeDesc(classRecord.EndTime)}",
                ClassRoomIdsDesc = classRoomIdsDesc,
                CourseListDesc = courseListDesc,
                Status = classRecord.Status,
                WeekDesc = $"周{EtmsHelper.GetWeekDesc(classRecord.Week)}",
                TeachersDesc = teachersDesc,
                AttendNumber = classRecord.AttendNumber,
                CheckOt = classRecord.CheckOt,
                CheckUserId = classRecord.CheckUserId,
                NeedAttendNumber = classRecord.NeedAttendNumber,
                ClassTimes = classRecord.ClassTimes.EtmsToString(),
                DeSum = EmRoleSecrecyType.GetSecrecyValue(EmRoleSecrecyType.NotLimited, classRecord.DeSum.EtmsToString2()),
                StatusDesc = EmClassRecordStatus.GetClassRecordStatusDesc(classRecord.Status),
                CheckUserDesc = await ComBusiness.GetUserName(tempBoxUser, _userDAL, classRecord.CheckUserId),
                IsLeaveCharge = myClass.IsLeaveCharge,
                IsNotComeCharge = myClass.IsNotComeCharge
            });
        }

        public async Task<ResponseBase> AlienTenantClassRecordStudentGet(AlienTenantClassRecordStudentGetRequest request)
        {
            var classRecordStudents = await _classRecordDAL.GetClassRecordStudents(request.ClassRecordId);
            var outPut = new List<AlienTenantClassRecordStudentGetOutput>();

            var courseTempBox = new DataTempBox<EtCourse>();
            foreach (var p in classRecordStudents)
            {
                var studentBucket = await _studentDAL.GetStudent(p.StudentId);
                if (studentBucket == null || studentBucket.Student == null)
                {
                    continue;
                }
                var deClassTimes = p.DeClassTimes.EtmsToString();
                var myCourse = await ComBusiness.GetCourse(courseTempBox, _courseDAL, p.CourseId);
                var courseDesc = string.Empty;
                var checkPointsDefault = 0;
                if (myCourse != null)
                {
                    courseDesc = myCourse.Name;
                    checkPointsDefault = myCourse.CheckPoints;
                }
                outPut.Add(new AlienTenantClassRecordStudentGetOutput()
                {
                    CId = p.Id,
                    CourseDesc = courseDesc,
                    CheckPointsDefault = checkPointsDefault,
                    CourseId = p.CourseId,
                    DeClassTimes = deClassTimes,
                    DeSum = EmRoleSecrecyType.GetSecrecyValue(EmRoleSecrecyType.NotLimited, p.DeSum.EtmsToString2()),
                    DeType = p.DeType,
                    DeTypeDesc = EmDeClassTimesType.GetDeClassTimesTypeDesc(p.DeType),
                    ExceedClassTimes = p.ExceedClassTimes.EtmsToString(),
                    IsRewardPoints = p.IsRewardPoints,
                    Remark = p.Remark,
                    RewardPoints = p.RewardPoints,
                    StudentCheckStatus = p.StudentCheckStatus,
                    StudentCheckStatusDesc = EmClassStudentCheckStatus.GetClassStudentCheckStatus(p.StudentCheckStatus),
                    StudentName = studentBucket.Student.Name,
                    StudentPhone = ComBusiness3.PhoneSecrecy(studentBucket.Student.Phone, EmRoleSecrecyType.NotLimited),
                    StudentType = p.StudentType,
                    StudentTypeDesc = EmClassStudentType.GetClassStudentTypeDesc(p.StudentType),
                    DeClassTimesDesc = ComBusiness2.GetDeClassTimesDesc(p.DeType, p.DeClassTimes, p.ExceedClassTimes),
                    Status = p.Status,
                    StatusDesc = EmClassRecordStatus.GetClassRecordStatusDesc(p.Status),
                    ChangeRowState = 0,
                    NewDeClassTimes = deClassTimes,
                    NewRemark = p.Remark,
                    NewStudentCheckStatus = p.StudentCheckStatus,
                    NewRewardPoints = p.RewardPoints,
                    SurplusCourseDesc = p.SurplusCourseDesc,
                });
            }
            return ResponseBase.Success(outPut);
        }
    }
}
