using ETMS.Business.Common;
using ETMS.Entity.Database.Source;
using ETMS.Entity.Enum;
using ETMS.Event.DataContract;
using ETMS.IBusiness.EventConsumer;
using ETMS.IDataAccess;
using ETMS.IEventProvider;
using ETMS.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.Business.EventConsumer
{
    public class EvAutoCheckSignBLL : IEvAutoCheckSignBLL
    {
        private readonly IClassTimesDAL _classTimesDAL;

        private readonly IClassRecordDAL _classRecordDAL;

        private readonly ITenantConfigDAL _tenantConfigDAL;

        private readonly IEventPublisher _eventPublisher;

        private readonly IClassDAL _classDAL;

        private readonly IStudentCheckOnLogDAL _studentCheckOnLogDAL;

        private readonly IUserDAL _userDAL;

        private readonly ICourseDAL _courseDAL;

        private readonly IStudentLeaveApplyLogDAL _studentLeaveApplyLogDAL;

        private readonly IStudentCourseDAL _studentCourseDAL;

        public EvAutoCheckSignBLL(IClassTimesDAL classTimesDAL, IClassRecordDAL classRecordDAL, ITenantConfigDAL tenantConfigDAL,
            IEventPublisher eventPublisher, IClassDAL classDAL, IStudentCheckOnLogDAL studentCheckOnLogDAL, IUserDAL userDAL,
            ICourseDAL courseDAL, IStudentLeaveApplyLogDAL studentLeaveApplyLogDAL, IStudentCourseDAL studentCourseDAL)
        {
            this._classTimesDAL = classTimesDAL;
            this._classRecordDAL = classRecordDAL;
            this._tenantConfigDAL = tenantConfigDAL;
            this._eventPublisher = eventPublisher;
            this._classDAL = classDAL;
            this._studentCheckOnLogDAL = studentCheckOnLogDAL;
            this._userDAL = userDAL;
            this._courseDAL = courseDAL;
            this._studentLeaveApplyLogDAL = studentLeaveApplyLogDAL;
            this._studentCourseDAL = studentCourseDAL;
        }

        public void InitTenantId(int tenantId)
        {
            this.InitDataAccess(tenantId, _classTimesDAL, _classRecordDAL, _tenantConfigDAL, _classDAL,
                _studentCheckOnLogDAL, _userDAL, _courseDAL, _studentLeaveApplyLogDAL, _studentCourseDAL);
        }

        public async Task AutoCheckSignTenantConsumerEvent(AutoCheckSignTenantEvent request)
        {
            var config = await _tenantConfigDAL.GetTenantConfig();
            if (!config.TenantOtherConfig.IsAutoCheckSign)
            {
                return;
            }
            var now = DateTime.Now;
            var minDateTime = now.AddMinutes(-config.TenantOtherConfig.AutoCheckSignLimitMinute);
            if (now.Date != minDateTime.Date)
            {
                return;
            }
            var minEndTimeVaule = EtmsHelper.GetTimeHourAndMinuteDesc(minDateTime);
            IEnumerable<EtClassTimes> unRollcallAndTimeOutData;
            if (config.TenantOtherConfig.AutoCheckSignTimeType == EmAutoCheckSignTimeType.EndTime)
            {
                unRollcallAndTimeOutData = await _classTimesDAL.GetUnRollcallAndTimeOut1(now, minEndTimeVaule);
            }
            else
            {
                unRollcallAndTimeOutData = await _classTimesDAL.GetUnRollcallAndTimeOut2(now, minEndTimeVaule);
            }
            if (unRollcallAndTimeOutData.Any())
            {
                foreach (var p in unRollcallAndTimeOutData)
                {
                    _eventPublisher.Publish(new AutoCheckSignClassTimesEvent(request.TenantId)
                    {
                        ClassTimesId = p.Id,
                        MakeupIsDeClassTimes = config.ClassCheckSignConfig.MakeupIsDeClassTimes,
                        AutoCheckSignCheckStudentType = config.TenantOtherConfig.AutoCheckSignCheckStudentType
                    });
                }
            }
        }

        /// <summary>
        /// 模拟点名
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task AutoCheckSignClassTimesConsumerEvent(AutoCheckSignClassTimesEvent request)
        {
            var myClassTimes = await _classTimesDAL.GetClassTimes(request.ClassTimesId);
            if (myClassTimes.DataType == EmClassTimesDataType.Stop)
            {
                return;
            }
            if (myClassTimes.Status == EmClassTimesStatus.BeRollcall)
            {
                return;
            }
            //防止重复点名
            var isRepeatData = await _classRecordDAL.ExistClassRecord(myClassTimes.ClassId, myClassTimes.ClassOt, myClassTimes.StartTime, myClassTimes.EndTime);
            if (isRepeatData)
            {
                return;
            }
            var etClassBucket = await _classDAL.GetClassBucket(myClassTimes.ClassId);
            if (etClassBucket == null || etClassBucket.EtClass == null)
            {
                return;
            }
            var myClass = etClassBucket.EtClass;
            var myClassStudent = etClassBucket.EtClassStudents;
            var classTimesStudent = await _classTimesDAL.GetClassTimesStudent(request.ClassTimesId);
            if ((myClassStudent == null || myClassStudent.Count == 0) && (classTimesStudent == null || classTimesStudent.Count == 0))
            {
                return;
            }

            var checkUser = await _userDAL.GetAdminUser();
            var classOt = myClassTimes.ClassOt.Date;
            var checkOt = DateTime.Now;
            var week = (byte)classOt.DayOfWeek;
            var courseIds = myClassTimes.CourseList;
            if (string.IsNullOrEmpty(courseIds))
            {
                courseIds = myClass.CourseList;
            }
            var classRoomIds = myClassTimes.ClassRoomIds;
            if (string.IsNullOrEmpty(classRoomIds))
            {
                classRoomIds = myClass.ClassRoomIds;
            }
            var teachers = myClassTimes.Teachers;
            if (string.IsNullOrEmpty(teachers))
            {
                teachers = myClass.Teachers;
            }
            var teacherIds = EtmsHelper.AnalyzeMuIds(teachers);
            var teacherNum = teacherIds.Count;

            //请假
            var studentLeave = await _studentLeaveApplyLogDAL.GetStudentLeaveApplyPassLog(classOt);
            var studentLeaveCheck = new StudentIsLeaveCheck(studentLeave);
            studentLeave = studentLeaveCheck.GetStudentLeaveList(myClassTimes.StartTime, myClassTimes.EndTime, classOt);

            //考勤
            List<EtStudentCheckOnLog> checkInLog = null;
            if (request.AutoCheckSignCheckStudentType == EmAutoCheckSignCheckStudentType.StudentCheckOn)
            {
                checkInLog = await _studentCheckOnLogDAL.GetStudentCheckOnLogByClassTimesId(request.ClassTimesId);
            }

            var classRecordStudents = new List<EtClassRecordStudent>();
            var tempBoxCouese = new DataTempBox<EtCourse>();
            if (myClassStudent != null && myClassStudent.Any())
            {
                foreach (var student in myClassStudent)
                {
                    var myCourse = await ComBusiness.GetCourse(tempBoxCouese, _courseDAL, student.CourseId);
                    if (myCourse == null)
                    {
                        continue;
                    }
                    //判断是否停课
                    var studentCourse = await _studentCourseDAL.GetStudentCourse(student.StudentId, student.CourseId);
                    var stopCourseResult = ComBusiness3.IsStopOfClass2(studentCourse, classOt);
                    if (stopCourseResult.Item1)
                    {
                        continue;
                    }

                    var isRewardPoints = myCourse.CheckPoints > 0;
                    var rewardPoints = myCourse.CheckPoints;
                    var remark = string.Empty;
                    var studentCheckStatus1 = EmClassStudentCheckStatus.Arrived;
                    var deClassTimes = myClass.DefaultClassTimes;
                    if (request.AutoCheckSignCheckStudentType == EmAutoCheckSignCheckStudentType.StudentCheckOn)
                    {
                        studentCheckStatus1 = EmClassStudentCheckStatus.NotArrived;
                        if (checkInLog != null && checkInLog.Count > 0)
                        {
                            var myCheckLog = checkInLog.FirstOrDefault(p => p.StudentId == student.StudentId);
                            if (myCheckLog != null)
                            {
                                studentCheckStatus1 = EmClassStudentCheckStatus.Arrived;
                                isRewardPoints = myCheckLog.Points > 0;
                                rewardPoints = myCheckLog.Points;
                                remark = $"考勤时间:{myCheckLog.CheckOt.EtmsToMinuteString()}";
                            }
                        }
                    }
                    if (studentLeave != null && studentLeave.Count > 0) //是否请假
                    {
                        var myLeaveLog = studentLeaveCheck.GeStudentLeaveLog(myClassTimes.StartTime, myClassTimes.EndTime, student.StudentId, classOt);
                        if (myLeaveLog != null)
                        {
                            studentCheckStatus1 = EmClassStudentCheckStatus.Leave;
                            isRewardPoints = false;
                            rewardPoints = 0;
                            remark = $"请假时间：{myLeaveLog.StartDate.EtmsToDateString()} {EtmsHelper.GetTimeDesc(myLeaveLog.StartTime)}~{myLeaveLog.EndDate.EtmsToDateString()} {EtmsHelper.GetTimeDesc(myLeaveLog.EndTime)}";
                        }
                    }

                    var myClassRecordStudent = new EtClassRecordStudent()
                    {
                        CheckOt = checkOt,
                        CheckUserId = checkUser.Id,
                        ClassContent = myClassTimes.ClassContent,
                        ClassId = myClassTimes.ClassId,
                        ClassOt = classOt,
                        ClassRoomIds = classRoomIds,
                        CourseId = student.CourseId,
                        DeClassTimes = deClassTimes,
                        EvaluateTeacherNum = 0,
                        DeSum = 0,
                        StartTime = myClassTimes.StartTime,
                        EndTime = myClassTimes.EndTime,
                        StudentType = EmClassStudentType.ClassStudent,
                        Remark = remark,
                        ExceedClassTimes = 0,
                        ClassRecordId = 0,
                        DeType = EmDeClassTimesType.NotDe,
                        IsBeEvaluate = false,
                        IsDeleted = EmIsDeleted.Normal,
                        IsRewardPoints = isRewardPoints,
                        RewardPoints = rewardPoints,
                        Status = EmClassRecordStatus.Normal,
                        StudentCheckStatus = studentCheckStatus1,
                        StudentId = student.StudentId,
                        StudentTryCalssLogId = null,
                        TeacherNum = teacherNum,
                        Teachers = teachers,
                        TenantId = request.TenantId,
                        Week = week
                    };
                    classRecordStudents.Add(myClassRecordStudent);
                }
            }
            if (classTimesStudent != null && classTimesStudent.Any())
            {
                foreach (var student in classTimesStudent)
                {
                    var hisLog = classRecordStudents.FirstOrDefault(j => j.StudentId == student.StudentId);
                    if (hisLog != null)
                    {
                        continue;
                    }
                    var myCourse = await ComBusiness.GetCourse(tempBoxCouese, _courseDAL, student.CourseId);
                    if (myCourse == null)
                    {
                        continue;
                    }
                    //判断是否停课
                    if (student.StudentType == EmClassStudentType.TempStudent)
                    {
                        var studentCourse = await _studentCourseDAL.GetStudentCourse(student.StudentId, student.CourseId);
                        var stopCourseResult = ComBusiness3.IsStopOfClass2(studentCourse, classOt);
                        if (stopCourseResult.Item1)
                        {
                            continue;
                        }
                    }

                    var deClassTimes = myClass.DefaultClassTimes;
                    switch (student.StudentType)
                    {
                        case EmClassStudentType.ClassStudent:
                            break;
                        case EmClassStudentType.TempStudent:
                            break;
                        case EmClassStudentType.TryCalssStudent:
                            deClassTimes = 0;
                            break;
                        case EmClassStudentType.MakeUpStudent:
                            if (!request.MakeupIsDeClassTimes)
                            {
                                deClassTimes = 0;
                            }
                            break;
                    }

                    var isRewardPoints = myCourse.CheckPoints > 0;
                    var rewardPoints = myCourse.CheckPoints;
                    var remark = string.Empty;
                    var studentCheckStatus2 = EmClassStudentCheckStatus.Arrived;
                    if (request.AutoCheckSignCheckStudentType == EmAutoCheckSignCheckStudentType.StudentCheckOn)
                    {
                        studentCheckStatus2 = EmClassStudentCheckStatus.NotArrived;
                        if (checkInLog != null && checkInLog.Count > 0)
                        {
                            var myCheckLog = checkInLog.FirstOrDefault(p => p.StudentId == student.StudentId);
                            if (myCheckLog != null)
                            {
                                studentCheckStatus2 = EmClassStudentCheckStatus.Arrived;
                                isRewardPoints = myCheckLog.Points > 0;
                                rewardPoints = myCheckLog.Points;
                                remark = $"考勤时间:{myCheckLog.CheckOt.EtmsToMinuteString()}";
                            }
                        }
                    }
                    if (studentLeave != null && studentLeave.Count > 0) //是否请假
                    {
                        var myLeaveLog = studentLeaveCheck.GeStudentLeaveLog(myClassTimes.StartTime, myClassTimes.EndTime, student.StudentId, classOt);
                        if (myLeaveLog != null)
                        {
                            studentCheckStatus2 = EmClassStudentCheckStatus.Leave;
                            isRewardPoints = false;
                            rewardPoints = 0;
                            remark = $"请假时间：{myLeaveLog.StartDate.EtmsToDateString()} {EtmsHelper.GetTimeDesc(myLeaveLog.StartTime)}~{myLeaveLog.EndDate.EtmsToDateString()} {EtmsHelper.GetTimeDesc(myLeaveLog.EndTime)}";
                        }
                    }

                    var myClassRecordStudent = new EtClassRecordStudent()
                    {
                        CheckOt = checkOt,
                        CheckUserId = checkUser.Id,
                        ClassContent = myClassTimes.ClassContent,
                        ClassId = myClassTimes.ClassId,
                        ClassOt = classOt,
                        ClassRoomIds = classRoomIds,
                        CourseId = student.CourseId,
                        DeClassTimes = deClassTimes,
                        EvaluateTeacherNum = 0,
                        DeSum = 0,
                        StartTime = myClassTimes.StartTime,
                        EndTime = myClassTimes.EndTime,
                        StudentType = student.StudentType,
                        Remark = remark,
                        ExceedClassTimes = 0,
                        ClassRecordId = 0,
                        DeType = EmDeClassTimesType.NotDe,
                        IsBeEvaluate = false,
                        IsDeleted = EmIsDeleted.Normal,
                        IsRewardPoints = isRewardPoints,
                        RewardPoints = rewardPoints,
                        Status = EmClassRecordStatus.Normal,
                        StudentCheckStatus = studentCheckStatus2,
                        StudentId = student.StudentId,
                        StudentTryCalssLogId = student.StudentTryCalssLogId,
                        TeacherNum = teacherNum,
                        Teachers = teachers,
                        TenantId = request.TenantId,
                        Week = week
                    };
                    classRecordStudents.Add(myClassRecordStudent);
                }
            }

            if (classRecordStudents.Count == 0)
            {
                return;
            }

            var attendStudent = classRecordStudents.Where(p => p.StudentCheckStatus == EmClassStudentCheckStatus.Arrived ||
            p.StudentCheckStatus == EmClassStudentCheckStatus.BeLate);
            var classRecord = new EtClassRecord()
            {
                AttendNumber = attendStudent.Count(),
                CheckOt = checkOt,
                CheckUserId = checkUser.Id,
                ClassContent = myClassTimes.ClassContent,
                ClassId = myClassTimes.ClassId,
                ClassOt = classOt,
                ClassRoomIds = classRoomIds,
                ClassTimes = myClass.DefaultClassTimes,
                ClassTimesId = myClassTimes.Id,
                CourseList = courseIds,
                DeSum = 0,
                EndTime = myClassTimes.EndTime,
                IsDeleted = EmIsDeleted.Normal,
                NeedAttendNumber = classRecordStudents.Count,
                Remark = "自动点名",
                StartTime = myClassTimes.StartTime,
                Status = EmClassRecordStatus.Normal,
                TeacherNum = teacherNum,
                Teachers = teachers,
                TenantId = request.TenantId,
                Week = week,
                StudentIds = EtmsHelper.GetMuIds(classRecordStudents.Select(p => p.StudentId)),
                EvaluateStudentCount = 0,
                ClassCategoryId = myClass.ClassCategoryId
            };

            _eventPublisher.Publish(new ClassCheckSignEvent(request.TenantId)
            {
                ClassRecord = classRecord,
                ClassRecordStudents = classRecordStudents,
                EvaluateStudents = new List<EtClassRecordEvaluateStudent>(),
                ClassName = etClassBucket.EtClass.Name,
                UserId = classRecord.CheckUserId,
                LoginClientType = request.LoginClientType
            });
        }
    }
}
