using ETMS.Entity.Common;
using ETMS.Entity.Config;
using ETMS.Entity.Dto.Educational.Request;
using ETMS.Entity.Enum;
using ETMS.IBusiness;
using ETMS.IDataAccess;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using ETMS.Entity.Database.Source;
using ETMS.Utility;
using ETMS.IEventProvider;
using ETMS.Event.DataContract;
using ETMS.Entity.Temp;
using ETMS.Entity.Temp.Request;
using Newtonsoft.Json;
using ETMS.Business.Common;
using ETMS.Event.DataContract.Statistics;
using ETMS.Entity.Dto.Educational.Output;

namespace ETMS.Business
{
    public class ClassCheckSignBLL : IClassCheckSignBLL
    {
        private readonly IClassDAL _classDAL;

        private readonly IClassTimesDAL _classTimesDAL;

        private readonly IClassRecordDAL _classRecordDAL;

        private readonly ITenantConfigDAL _tenantConfigDAL;

        private readonly IStudentCourseDAL _studentCourseDAL;

        private readonly IEventPublisher _eventPublisher;

        private readonly IStudentDAL _studentDAL;

        private readonly ITryCalssLogDAL _tryCalssLogDAL;

        private readonly IStudentTrackLogDAL _studentTrackLogDAL;

        private readonly IStudentPointsLogDAL _studentPointsLog;

        private readonly IUserOperationLogDAL _userOperationLogDAL;

        private readonly IUserDAL _userDAL;

        private readonly IStudentCourseConsumeLogDAL _studentCourseConsumeLogDAL;

        private readonly IStudentCourseAnalyzeBLL _studentCourseAnalyzeBLL;

        private readonly IStudentCheckOnLogDAL _studentCheckOnLogDAL;

        private readonly ITempStudentNeedCheckDAL _tempStudentNeedCheckDAL;

        public ClassCheckSignBLL(IClassDAL classDAL, IClassTimesDAL classTimesDAL, IClassRecordDAL classRecordDAL, ITenantConfigDAL tenantConfigDAL,
            IStudentCourseDAL studentCourseDAL, IEventPublisher eventPublisher, IStudentDAL studentDAL, ITryCalssLogDAL tryCalssLogDAL,
            IStudentTrackLogDAL studentTrackLogDAL, IStudentPointsLogDAL studentPointsLog, IUserDAL userDAL, IUserOperationLogDAL userOperationLogDAL,
            IStudentCourseConsumeLogDAL studentCourseConsumeLogDAL, IStudentCourseAnalyzeBLL studentCourseAnalyzeBLL, IStudentCheckOnLogDAL studentCheckOnLogDAL,
            ITempStudentNeedCheckDAL tempStudentNeedCheckDAL)
        {
            this._classDAL = classDAL;
            this._classTimesDAL = classTimesDAL;
            this._classRecordDAL = classRecordDAL;
            this._tenantConfigDAL = tenantConfigDAL;
            this._studentCourseDAL = studentCourseDAL;
            this._eventPublisher = eventPublisher;
            this._studentDAL = studentDAL;
            this._tryCalssLogDAL = tryCalssLogDAL;
            this._studentTrackLogDAL = studentTrackLogDAL;
            this._studentPointsLog = studentPointsLog;
            this._userDAL = userDAL;
            this._userOperationLogDAL = userOperationLogDAL;
            this._studentCourseConsumeLogDAL = studentCourseConsumeLogDAL;
            this._studentCourseAnalyzeBLL = studentCourseAnalyzeBLL;
            this._studentCheckOnLogDAL = studentCheckOnLogDAL;
            this._tempStudentNeedCheckDAL = tempStudentNeedCheckDAL;
        }

        public void InitTenantId(int tenantId)
        {
            this._studentCourseAnalyzeBLL.InitTenantId(tenantId);
            this.InitDataAccess(tenantId, _classDAL, _classTimesDAL, _classRecordDAL, _studentDAL, _tryCalssLogDAL, _studentTrackLogDAL,
                _studentPointsLog, _userDAL, _tenantConfigDAL, _studentCourseDAL, _userOperationLogDAL, _studentCourseConsumeLogDAL,
                _studentCheckOnLogDAL, _tempStudentNeedCheckDAL);
        }

        public async Task<ResponseBase> ClassCheckSign(ClassCheckSignRequest request)
        {
            var etClassBucket = await _classDAL.GetClassBucket(request.ClassId);
            if (etClassBucket == null || etClassBucket.EtClass == null)
            {
                return ResponseBase.CommonError("班级不存在");
            }
            if (request.ClassTimesId != null)
            {
                var classTime = await _classTimesDAL.GetClassTimes(request.ClassTimesId.Value);
                if (classTime == null)
                {
                    return ResponseBase.CommonError("课次不存在");
                }
                if (classTime.Status == EmClassTimesStatus.BeRollcall)
                {
                    return ResponseBase.CommonError("此课次已点名");
                }
            }
            //防止重复点名
            if (!request.IsIgnoreCheck)
            {
                var isRepeatData = await _classRecordDAL.ExistClassRecord(request.ClassId, request.ClassOt, request.StartTime, request.EndTime);
                if (isRepeatData)
                {
                    return ResponseBase.Success(new ClassCheckSignOutput(false));
                }
            }

            List<EtStudentCheckOnLog> checkInLog = null;
            if (request.ClassTimesId != null)
            {
                checkInLog = await _studentCheckOnLogDAL.GetStudentCheckOnLogByClassTimesId(request.ClassTimesId.Value);
            }
            var tenantConfig = await _tenantConfigDAL.GetTenantConfig();
            var classOt = request.ClassOt.Date;
            foreach (var student in request.Students)
            {
                if (checkInLog != null && checkInLog.Count > 0)
                {
                    //考勤记上课
                    var myCheckLog = checkInLog.FirstOrDefault(p => p.StudentId == student.StudentId);
                    if (myCheckLog != null)
                    {
                        continue;
                    }
                }
                var errMsg = string.Empty;
                switch (student.StudentType)
                {
                    case EmClassStudentType.ClassStudent:
                        errMsg = await CheckStudentCourse(student, tenantConfig, classOt);
                        break;
                    case EmClassStudentType.TempStudent:
                        errMsg = await CheckStudentCourse(student, tenantConfig, classOt);
                        break;
                    case EmClassStudentType.MakeUpStudent:
                        if (tenantConfig.ClassCheckSignConfig.MakeupIsDeClassTimes)
                        {
                            errMsg = await CheckStudentCourse(student, tenantConfig, classOt);
                        }
                        break;
                    case EmClassStudentType.TryCalssStudent:
                        break;
                }
                if (!string.IsNullOrEmpty(errMsg))
                {
                    return ResponseBase.CommonError(errMsg);
                }
            }
            var checkOt = DateTime.Now;
            var week = (byte)classOt.DayOfWeek;
            var classRoomIds = EtmsHelper.GetMuIds(request.ClassRoomIds);
            var teachers = EtmsHelper.GetMuIds(request.TeacherIds);
            var teacherNum = request.TeacherIds.Count;
            var classRecordStudents = new List<EtClassRecordStudent>();
            foreach (var student in request.Students)
            {
                classRecordStudents.Add(new EtClassRecordStudent()
                {
                    CheckOt = checkOt,
                    CheckUserId = request.LoginUserId,
                    ClassContent = request.ClassContent,
                    ClassId = request.ClassId,
                    ClassOt = classOt,
                    ClassRoomIds = classRoomIds,
                    CourseId = student.CourseId,
                    DeClassTimes = student.DeClassTimes,
                    EvaluateTeacherNum = 0,
                    DeSum = 0,
                    StartTime = request.StartTime,
                    EndTime = request.EndTime,
                    StudentType = student.StudentType,
                    Remark = student.Remark,
                    ExceedClassTimes = 0,
                    ClassRecordId = 0,
                    DeType = EmDeClassTimesType.NotDe,
                    IsBeEvaluate = false,
                    IsDeleted = EmIsDeleted.Normal,
                    IsRewardPoints = student.RewardPoints > 0,
                    RewardPoints = student.RewardPoints,
                    Status = EmClassRecordStatus.Normal,
                    StudentCheckStatus = student.StudentCheckStatus,
                    StudentId = student.StudentId,
                    StudentTryCalssLogId = student.StudentTryCalssLogId,
                    TeacherNum = teacherNum,
                    Teachers = teachers,
                    TenantId = request.LoginTenantId,
                    Week = week
                });
            }

            var attendStudent = request.Students.Where(p => p.StudentCheckStatus == EmClassStudentCheckStatus.Arrived ||
            p.StudentCheckStatus == EmClassStudentCheckStatus.BeLate);
            var classRecord = new EtClassRecord()
            {
                AttendNumber = attendStudent.Count(),
                CheckOt = checkOt,
                CheckUserId = request.LoginUserId,
                ClassContent = request.ClassContent,
                ClassId = request.ClassId,
                ClassOt = classOt,
                ClassRoomIds = classRoomIds,
                ClassTimes = request.ClassTimes,
                ClassTimesId = request.ClassTimesId,
                CourseList = EtmsHelper.GetMuIds(request.CourseIds),
                DeSum = 0,
                EndTime = request.EndTime,
                IsDeleted = EmIsDeleted.Normal,
                NeedAttendNumber = request.Students.Count,
                Remark = string.Empty,
                StartTime = request.StartTime,
                Status = EmClassRecordStatus.Normal,
                TeacherNum = teacherNum,
                Teachers = teachers,
                TenantId = request.LoginTenantId,
                Week = week,
                StudentIds = EtmsHelper.GetMuIds(request.Students.Select(p => p.StudentId))
            };
            _eventPublisher.Publish(new ClassCheckSignEvent(request.LoginTenantId)
            {
                ClassRecord = classRecord,
                ClassRecordStudents = classRecordStudents,
                ClassName = etClassBucket.EtClass.Name,
                UserId = request.LoginUserId,
                LoginClientType = request.LoginClientType
            });
            return ResponseBase.Success(new ClassCheckSignOutput(true));
        }

        private async Task<string> CheckStudentCourse(ClassCheckSignStudent student, TenantConfig config, DateTime classDate)
        {
            if (config.ClassCheckSignConfig.MustBuyCourse) //是否必须购买了此课程
            {
                var myCourse = await _studentCourseDAL.GetStudentCourse(student.StudentId, student.CourseId);
                if (myCourse == null || !myCourse.Any())
                {
                    return $"学员({student.StudentName})未购买此课程，无法点名";
                }
            }
            List<EtStudentCourseDetail> myCourseDetail = null;
            if (config.ClassCheckSignConfig.MustEnoughSurplusClassTimes) //是否有足够课时
            {
                myCourseDetail = await _studentCourseDAL.GetStudentCourseDetail(student.StudentId, student.CourseId);
                if (myCourseDetail == null || !myCourseDetail.Any())
                {
                    return $"学员({student.StudentName})未购买此课程，无法点名";
                }
                var dayCourseDetail = myCourseDetail.Where(p => p.DeType == EmDeClassTimesType.Day && (p.StartTime == null || classDate >= p.StartTime)
                && (p.EndTime == null || classDate <= p.EndTime) && p.Status == EmStudentCourseStatus.Normal);
                if (!dayCourseDetail.Any())
                {
                    var timesCourseDetail = myCourseDetail.Where(p => p.DeType == EmDeClassTimesType.ClassTimes && p.Status == EmStudentCourseStatus.Normal
                    && (p.EndTime == null || classDate <= p.EndTime) && p.SurplusQuantity >= student.DeClassTimes);
                    if (!timesCourseDetail.Any())
                    {
                        var stopTimeCourseDetail = myCourseDetail.Where(p => p.DeType == EmDeClassTimesType.ClassTimes
                        && p.Status == EmStudentCourseStatus.StopOfClass && (p.EndTime == null || classDate <= p.EndTime) && p.SurplusQuantity >= student.DeClassTimes);
                        if (stopTimeCourseDetail.Any())
                        {
                            return $"学员({student.StudentName})已停课，无法点名";
                        }
                        var stopDayCourseDetail = myCourseDetail.Where(p => p.DeType == EmDeClassTimesType.Day && (p.StartTime == null || classDate >= p.StartTime)
                        && (p.EndTime == null || classDate <= p.EndTime) && p.Status == EmStudentCourseStatus.StopOfClass);
                        if (stopDayCourseDetail.Any())
                        {
                            return $"学员({student.StudentName})已停课，无法点名";
                        }
                        return $"学员({student.StudentName})剩余课时不足，无法点名";
                    }
                }

            }
            if (config.ClassCheckSignConfig.DayCourseMustSetStartEndTime)
            {
                if (myCourseDetail == null)
                {
                    return string.Empty;
                    //myCourseDetail = await _studentCourseDAL.GetStudentCourseDetail(student.StudentId, student.CourseId);
                    //if (myCourseDetail == null || !myCourseDetail.Any())
                    //{
                    //    return $"学员({student.StudentName})未购买此课程，无法点名";
                    //}
                }
                var timesCourseDetail = myCourseDetail.Where(p => p.DeType == EmDeClassTimesType.ClassTimes && p.Status == EmStudentCourseStatus.Normal
                && (p.EndTime == null || classDate <= p.EndTime));
                if (!timesCourseDetail.Any())
                {
                    var dayCourseDetail = myCourseDetail.Where(p => p.DeType == EmDeClassTimesType.Day && (p.StartTime != null && classDate >= p.StartTime)
                    && (p.EndTime != null && classDate <= p.EndTime) && p.Status == EmStudentCourseStatus.Normal);
                    if (!dayCourseDetail.Any())
                    {
                        var notOpenDayCourseDetail = myCourseDetail.Where(p => p.DeType == EmDeClassTimesType.Day && p.StartTime == null
                        && p.EndTime == null && p.Status == EmStudentCourseStatus.Normal);
                        if (notOpenDayCourseDetail.Any())
                        {
                            return $"学员({student.StudentName})未设置课程起止日期，无法点名";
                        }
                    }
                }
            }
            return string.Empty;
        }

        public async Task ClassCheckSignEventProcessEvent(ClassCheckSignEvent request)
        {
            if (request.ClassRecord.ClassTimesId != null)
            {
                var classTime = await _classTimesDAL.GetClassTimes(request.ClassRecord.ClassTimesId.Value);
                if (classTime.Status == EmClassTimesStatus.BeRollcall)
                {
                    LOG.Log.Error($"[ClassCheckSignBLL]无法处理已点名的课次,{JsonConvert.SerializeObject(request)}", this.GetType());
                    return;
                }

            }
            var etClassBucket = await _classDAL.GetClassBucket(request.ClassRecord.ClassId);
            var isLeaveCharge = etClassBucket.EtClass.IsLeaveCharge;
            var isNotComeCharge = etClassBucket.EtClass.IsNotComeCharge;
            var tenantConfig = await _tenantConfigDAL.GetTenantConfig();
            decimal totalDeSum = 0;
            List<EtStudentCheckOnLog> checkInLog = null;
            if (request.ClassRecord.ClassTimesId != null)
            {
                checkInLog = await _studentCheckOnLogDAL.GetStudentCheckOnLogByClassTimesId(request.ClassRecord.ClassTimesId.Value);
            }
            var studentCourseConsumeLogs = new List<EtStudentCourseConsumeLog>();
            var isProcess = false;
            var attendStudentIds = new List<long>();
            foreach (var student in request.ClassRecordStudents)
            {
                if (EmClassStudentCheckStatus.CheckIsAttend(student.StudentCheckStatus))
                {
                    attendStudentIds.Add(student.StudentId);
                }
                isProcess = false;
                var deStudentClassTimesResult = DeStudentClassTimesResult.GetNotDeEntity();
                if (checkInLog != null && checkInLog.Count > 0)
                {
                    //考勤记上课
                    var myCheckLog = checkInLog.FirstOrDefault(p => p.StudentId == student.StudentId);
                    if (myCheckLog != null)
                    {
                        deStudentClassTimesResult = new DeStudentClassTimesResult()
                        {
                            DeClassTimes = myCheckLog.DeClassTimes,
                            DeStudentCourseDetailId = myCheckLog.DeStudentCourseDetailId,
                            DeSum = myCheckLog.DeSum,
                            DeType = myCheckLog.DeType,
                            ExceedClassTimes = myCheckLog.ExceedClassTimes
                        };
                        isProcess = true;
                    }
                }
                if (!isProcess)
                {
                    switch (student.StudentType)
                    {
                        case EmClassStudentType.ClassStudent:
                            deStudentClassTimesResult = await DeStudentClassTimes(student, isLeaveCharge, isNotComeCharge);
                            break;
                        case EmClassStudentType.TempStudent:
                            deStudentClassTimesResult = await DeStudentClassTimes(student, isLeaveCharge, isNotComeCharge);
                            break;
                        case EmClassStudentType.MakeUpStudent:
                            if (tenantConfig.ClassCheckSignConfig.MakeupIsDeClassTimes)
                            {
                                deStudentClassTimesResult = await DeStudentClassTimes(student, isLeaveCharge, isNotComeCharge);
                            }
                            break;
                        case EmClassStudentType.TryCalssStudent:
                            break;
                    }
                }
                student.DeSum = deStudentClassTimesResult.DeSum;
                student.DeType = deStudentClassTimesResult.DeType;
                student.ExceedClassTimes = deStudentClassTimesResult.ExceedClassTimes;
                totalDeSum += deStudentClassTimesResult.DeSum;
                if (deStudentClassTimesResult.DeType != EmDeClassTimesType.ClassTimes)
                {
                    student.DeClassTimes = 0;
                }
                else
                {
                    student.DeClassTimes = deStudentClassTimesResult.DeClassTimes;
                }
                student.DeStudentCourseDetailId = deStudentClassTimesResult.DeStudentCourseDetailId;
                if (deStudentClassTimesResult.DeType != EmDeClassTimesType.NotDe && !isProcess)
                {
                    studentCourseConsumeLogs.Add(new EtStudentCourseConsumeLog()
                    {
                        CourseId = student.CourseId,
                        DeClassTimes = deStudentClassTimesResult.DeClassTimes,
                        DeType = deStudentClassTimesResult.DeType,
                        IsDeleted = EmIsDeleted.Normal,
                        OrderId = deStudentClassTimesResult.OrderId,
                        OrderNo = deStudentClassTimesResult.OrderNo,
                        Ot = request.ClassRecord.CheckOt,
                        SourceType = EmStudentCourseConsumeSourceType.ClassCheckSign,
                        StudentId = student.StudentId,
                        TenantId = student.TenantId,
                        DeClassTimesSmall = 0
                    });
                }
            }
            request.ClassRecord.DeSum = totalDeSum;
            var recordId = await _classRecordDAL.AddEtClassRecord(request.ClassRecord, request.ClassRecordStudents); //上课记录和详情
            if (studentCourseConsumeLogs.Any())
            {
                _studentCourseConsumeLogDAL.AddStudentCourseConsumeLog(studentCourseConsumeLogs); //课消记录
            }
            if (request.ClassRecord.ClassTimesId != null)
            {
                await _classTimesDAL.UpdateClassTimesIsClassCheckSign(request.ClassRecord.ClassTimesId.Value, recordId, EmClassTimesStatus.BeRollcall, request.ClassRecord);
                await _tempStudentNeedCheckDAL.TempStudentNeedCheckClassSetIsAttendClass(request.ClassRecord.ClassTimesId.Value);
                if (checkInLog != null && checkInLog.Count > 0)
                {
                    await _studentCheckOnLogDAL.UpdateStudentCheckOnIsBeRollcall(request.ClassRecord.ClassTimesId.Value);
                }
                await _classTimesDAL.ClassTimesReservationLogEditStatusBuyClassCheck(request.ClassRecord.ClassTimesId.Value, attendStudentIds);
            }
            var classRecordAbsenceLogs = new List<EtClassRecordAbsenceLog>();
            var classRecordPointsApplyLog = new List<EtClassRecordPointsApplyLog>();
            var studentPointsLogs = new List<EtStudentPointsLog>();
            var studentTrackLogs = new List<EtStudentTrackLog>();
            foreach (var student in request.ClassRecordStudents)
            {
                if (student.StudentType == EmClassStudentType.MakeUpStudent)
                {
                    await MakeUpStudentProcess(request.ClassRecord, student);
                }
                if (checkInLog != null && checkInLog.Count > 0)
                {
                    //考勤记上课
                    var myCheckLog = checkInLog.FirstOrDefault(p => p.StudentId == student.StudentId);
                    if (myCheckLog != null)
                    {
                        continue;
                    }
                }
                //缺勤记录
                ComBusiness3.ClassRecordAbsenceProcess(classRecordAbsenceLogs, student, recordId);

                if (student.RewardPoints > 0)
                {
                    //积分（申请，变动，增加学员积分）
                    await RewardPointsProcess(classRecordPointsApplyLog, studentPointsLogs, student, recordId, tenantConfig.ClassCheckSignConfig.RewardPointsMustApply);
                }

                if (student.StudentType == EmClassStudentType.TryCalssStudent && student.StudentTryCalssLogId != null)
                {
                    //试听处理
                    await ComBusiness3.TryCalssStudentProcess(_tryCalssLogDAL, _studentDAL, _eventPublisher, studentTrackLogs,
                        request.ClassRecord, student, recordId, tenantConfig.ClassCheckSignConfig.TryCalssNoticeTrackUser);
                }

                //学员课程详情
                //_eventPublisher.Publish(new StudentCourseDetailAnalyzeEvent(student.TenantId)
                //{
                //    StudentId = student.StudentId,
                //    CourseId = student.CourseId
                //});
                //解决bug#6587 微信通知的课时不及时
                await _studentCourseAnalyzeBLL.CourseDetailAnalyze(new StudentCourseDetailAnalyzeEvent(student.TenantId)
                {
                    StudentId = student.StudentId,
                    CourseId = student.CourseId
                });
            }
            if (classRecordAbsenceLogs.Any())
            {
                _classRecordDAL.AddClassRecordAbsenceLog(classRecordAbsenceLogs);
            }
            if (classRecordPointsApplyLog.Any())
            {
                _classRecordDAL.AddClassRecordPointsApplyLog(classRecordPointsApplyLog);
            }
            if (studentPointsLogs.Any())
            {
                _studentPointsLog.AddStudentPointsLog(studentPointsLogs);
            }
            if (studentTrackLogs.Any())
            {
                await _studentTrackLogDAL.AddStudentTrackLog(studentTrackLogs);
            }

            //老师上课统计
            var teachers = EtmsHelper.AnalyzeMuIds(request.ClassRecord.Teachers);
            foreach (var teacherId in teachers)
            {
                await _userDAL.AddTeacherClassTimesInfo(teacherId, request.ClassRecord.ClassTimes, 1);
                await _userDAL.AddTeacherMonthClassTimes(teacherId, request.ClassRecord.ClassOt, request.ClassRecord.ClassTimes, 1);
            }

            _eventPublisher.Publish(new StatisticsClassEvent(request.TenantId)
            {
                ClassOt = request.ClassRecord.ClassOt
            });
            _eventPublisher.Publish(new NoticeStudentsCheckSignEvent(request.TenantId)
            {
                ClassRecordId = recordId
            });
            _eventPublisher.Publish(new ResetTenantToDoThingEvent(request.TenantId));
            _eventPublisher.Publish(new StatisticsClassFinishCountEvent(request.TenantId)
            {
                ClassId = request.ClassRecord.ClassId
            });

            await _userOperationLogDAL.AddUserLog(new EtUserOperationLog()
            {
                IpAddress = string.Empty,
                IsDeleted = EmIsDeleted.Normal,
                OpContent = $"班级:{request.ClassName},上课时间:{request.ClassRecord.ClassOt.EtmsToDateString()},点名时间:{request.ClassRecord.CheckOt.EtmsToString()},应到:{request.ClassRecord.NeedAttendNumber}人,实到:{request.ClassRecord.AttendNumber}人",
                Ot = request.CreateTime,
                Remark = string.Empty,
                TenantId = request.TenantId,
                UserId = request.UserId,
                Type = (int)EmUserOperationType.ClassCheckSign,
                ClientType = request.LoginClientType
            });

            var notArrivedStudents = request.ClassRecordStudents.Where(p => p.StudentCheckStatus == EmClassStudentCheckStatus.NotArrived).ToList();
            if (notArrivedStudents.Count > 0)
            {
                _eventPublisher.Publish(new NoticeUserContractsNotArrivedEvent(request.TenantId)
                {
                    ClassRecordNotArrivedStudents = notArrivedStudents,
                    ClassName = request.ClassName,
                    ClassRecord = request.ClassRecord
                });
            }

            if (!EtmsHelper2.IsThisMonth(request.ClassRecord.ClassOt))
            {
                _eventPublisher.Publish(new StatisticsEducationEvent(request.TenantId)
                {
                    Time = request.ClassRecord.ClassOt
                });
            }

            _eventPublisher.Publish(new StatisticsTeacherSalaryClassTimesEvent(request.TenantId)
            {
                ClassRecordId = recordId
            });
            _eventPublisher.Publish(new StatisticsTeacherSalaryClassDayEvent(request.TenantId)
            {
                Time = request.ClassRecord.ClassOt
            });
        }

        private async Task<DeStudentClassTimesResult> DeStudentClassTimes(EtClassRecordStudent classRecordStudent, bool isLeaveCharge, bool isNotComeCharge)
        {
            if (!isLeaveCharge && classRecordStudent.StudentCheckStatus == EmClassStudentCheckStatus.Leave)
            {
                return DeStudentClassTimesResult.GetNotDeEntity();
            }
            if (!isNotComeCharge && classRecordStudent.StudentCheckStatus == EmClassStudentCheckStatus.NotArrived)
            {
                return DeStudentClassTimesResult.GetNotDeEntity();
            }
            return await CoreBusiness.DeStudentClassTimes(_studentCourseDAL, new DeStudentClassTimesTempRequest()
            {
                ClassOt = classRecordStudent.ClassOt,
                CourseId = classRecordStudent.CourseId,
                DeClassTimes = classRecordStudent.DeClassTimes,
                StudentId = classRecordStudent.StudentId,
                TenantId = classRecordStudent.TenantId
            });
        }

        private async Task RewardPointsProcess(List<EtClassRecordPointsApplyLog> classRecordPointsApplyLog,
            List<EtStudentPointsLog> studentPointsLogs,
            EtClassRecordStudent student, long recordId, bool rewardPointsMustApply)
        {
            var applyLog = new EtClassRecordPointsApplyLog()
            {
                Status = EmClassRecordStatus.Normal,
                TenantId = student.TenantId,
                ApplyOt = student.CheckOt,
                CheckOt = student.CheckOt,
                CheckUserId = student.CheckUserId,
                ClassId = student.ClassId,
                ClassOt = student.ClassOt,
                ClassRecordId = recordId,
                ClassRoomIds = student.ClassRoomIds,
                CourseId = student.CourseId,
                EndTime = student.EndTime,
                IsDeleted = EmIsDeleted.Normal,
                Points = student.RewardPoints,
                Remark = student.Remark,
                StartTime = student.StartTime,
                StudentId = student.StudentId,
                StudentCheckStatus = student.StudentCheckStatus,
                StudentTryCalssLogId = student.StudentTryCalssLogId,
                StudentType = student.StudentType,
                TeacherId = student.CheckUserId,
                Week = student.Week,
                HandleOt = null,
                HandleStatus = EmClassRecordPointsApplyHandleStatus.NotCheckd,
                HandleUser = null
            };
            if (!rewardPointsMustApply) //无需审核
            {
                applyLog.HandleOt = student.CheckOt;
                applyLog.HandleStatus = EmClassRecordPointsApplyHandleStatus.CheckPass;
                applyLog.HandleUser = student.CheckUserId;
            }
            classRecordPointsApplyLog.Add(applyLog);
            if (!rewardPointsMustApply) //无需审核 直接奖励积分
            {
                studentPointsLogs.Add(new EtStudentPointsLog()
                {
                    IsDeleted = EmIsDeleted.Normal,
                    No = string.Empty,
                    Ot = student.CheckOt,
                    Points = student.RewardPoints,
                    Remark = student.Remark,
                    StudentId = student.StudentId,
                    TenantId = student.TenantId,
                    Type = EmStudentPointsLogType.ClassReward
                });
                await _studentDAL.AddPoint(student.StudentId, student.RewardPoints);
            }
        }

        private async Task MakeUpStudentProcess(EtClassRecord classRecord, EtClassRecordStudent student)
        {
            if (EmClassStudentCheckStatus.CheckIsAttend(student.StudentCheckStatus))
            {
                //如果是补课学员  则自动标记补课记录
                var absenceLog = await _classRecordDAL.GetRelatedAbsenceLog(student.StudentId, student.CourseId);
                if (absenceLog != null)
                {
                    var classTimesDesc = $"{classRecord.ClassOt.EtmsToDateString()} {EtmsHelper.GetTimeDesc(classRecord.StartTime)}~{EtmsHelper.GetTimeDesc(classRecord.EndTime)}";
                    absenceLog.HandleStatus = EmClassRecordAbsenceHandleStatus.MarkFinish;
                    absenceLog.HandleContent = $"[已补课] 上课时间:{classTimesDesc}";
                    absenceLog.HandleOt = DateTime.Now;
                    absenceLog.HandleUser = classRecord.CheckUserId;
                    await _classRecordDAL.UpdateClassRecordAbsenceLog(absenceLog);
                }
            }
        }
    }
}
