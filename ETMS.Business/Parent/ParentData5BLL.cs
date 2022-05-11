using ETMS.Business.Common;
using ETMS.Entity.Common;
using ETMS.Entity.Database.Source;
using ETMS.Entity.Dto.Parent5.Output;
using ETMS.Entity.Dto.Parent5.Request;
using ETMS.Entity.Enum;
using ETMS.Entity.Temp;
using ETMS.Entity.View;
using ETMS.Event.DataContract;
using ETMS.IBusiness;
using ETMS.IBusiness.Parent;
using ETMS.IDataAccess;
using ETMS.IEventProvider;
using ETMS.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.Business.Parent
{
    public class ParentData5BLL : IParentData5BLL
    {
        private readonly IClassDAL _classDAL;

        private readonly IUserDAL _userDAL;

        private readonly IStudentDAL _studentDAL;

        private readonly ITeacherSchooltimeConfigDAL _teacherSchooltimeConfigDAL;

        private readonly ICourseDAL _courseDAL;

        private readonly IHolidaySettingDAL _holidaySettingDAL;

        private readonly IClassTimesDAL _classTimesDAL;

        private readonly IAppConfig2BLL _appConfig2BLL;

        private readonly IEventPublisher _eventPublisher;

        private readonly IStudentOperationLogDAL _studentOperationLogDAL;

        public ParentData5BLL(IClassDAL classDAL, IUserDAL userDAL, IStudentDAL studentDAL,
            ITeacherSchooltimeConfigDAL teacherSchooltimeConfigDAL, ICourseDAL courseDAL,
            IHolidaySettingDAL holidaySettingDAL, IClassTimesDAL classTimesDAL, IAppConfig2BLL appConfig2BLL,
            IEventPublisher eventPublisher, IStudentOperationLogDAL studentOperationLogDAL)
        {
            this._classDAL = classDAL;
            this._userDAL = userDAL;
            this._studentDAL = studentDAL;
            this._teacherSchooltimeConfigDAL = teacherSchooltimeConfigDAL;
            this._courseDAL = courseDAL;
            this._holidaySettingDAL = holidaySettingDAL;
            this._classTimesDAL = classTimesDAL;
            this._appConfig2BLL = appConfig2BLL;
            this._eventPublisher = eventPublisher;
            this._studentOperationLogDAL = studentOperationLogDAL;
        }

        public void InitTenantId(int tenantId)
        {
            this._appConfig2BLL.InitTenantId(tenantId);
            this.InitDataAccess(tenantId, _classDAL, _userDAL, _studentDAL, _teacherSchooltimeConfigDAL,
                _courseDAL, _holidaySettingDAL, _classTimesDAL, _studentOperationLogDAL);
        }

        public async Task<ResponseBase> StudentReservation1v1Check(StudentReservation1v1CheckRequest request)
        {
            var isHaveReservation1v1Class = await _classDAL.CheckStudentHaveOneToOneClassNormalIsReservation(request.StudentId);
            return ResponseBase.Success(new StudentReservation1v1CheckOutput()
            {
                IsHaveReservation1v1Class = isHaveReservation1v1Class
            });
        }

        public async Task<ResponseBase> StudentReservation1v1Init(StudentReservation1v1InitRequest request)
        {
            var output = new StudentReservation1v1InitOutput()
            {
                AllCourses = new List<StudentReservation1v1Course>()
            };
            var my1v1ClassLog = await _classDAL.GetStudentOneToOneClassNormalIsReservation(request.StudentId);
            if (my1v1ClassLog.Any())
            {
                var tempBoxUser = new DataTempBox<EtUser>();
                var processCourseIds = new List<long>();
                foreach (var itemClass in my1v1ClassLog)
                {
                    if (string.IsNullOrEmpty(itemClass.Teachers))
                    {
                        continue;
                    }
                    var myClassBucket = await _classDAL.GetClassBucket(itemClass.Id);
                    if (myClassBucket == null || myClassBucket.EtClass == null ||
                        myClassBucket.EtClass.CompleteStatus == EmClassCompleteStatus.Completed)
                    {
                        continue;
                    }
                    if (myClassBucket.EtClassStudents == null || myClassBucket.EtClassStudents.Count == 0)
                    {
                        continue;
                    }
                    var itemLog = myClassBucket.EtClassStudents.FirstOrDefault(j => j.StudentId == request.StudentId);
                    if (itemLog == null)
                    {
                        continue;
                    }
                    if (processCourseIds.Exists(j => j == itemLog.CourseId))
                    {
                        continue;
                    }
                    var myCourse = await _courseDAL.GetCourse(itemLog.CourseId);
                    if (myCourse == null || myCourse.Item1 == null)
                    {
                        continue;
                    }
                    if (myClassBucket.EtClass.ReservationType == EmBool.False)
                    {
                        continue;
                    }
                    if (myClassBucket.EtClass.DurationHour == 0 && myClassBucket.EtClass.DurationMinute == 0)
                    {
                        continue;
                    }
                    var allTeacherIds = EtmsHelper.AnalyzeMuIds(myClassBucket.EtClass.Teachers);
                    if (allTeacherIds.Count == 0)
                    {
                        continue;
                    }
                    if (allTeacherIds.Count == 1)
                    {
                        var myTeacherId1 = allTeacherIds[0];
                        var teacher = await ComBusiness.GetUser(tempBoxUser, _userDAL, myTeacherId1);
                        if (teacher == null)
                        {
                            continue;
                        }
                        var myTeacherSchooltimeConfig = await _teacherSchooltimeConfigDAL.TeacherSchooltimeConfigGet(myTeacherId1);
                        if (myTeacherSchooltimeConfig == null)
                        {
                            continue;
                        }
                        output.AllCourses.Add(new StudentReservation1v1Course()
                        {
                            CId = itemLog.CourseId,
                            Name = myCourse.Item1.Name,
                            Teachers = new List<StudentReservation1v1Teacher>() {
                              new StudentReservation1v1Teacher(){
                                  CId =myTeacherId1,
                                  ClassId = itemLog.ClassId,
                                  Name = ComBusiness2.GetParentTeacherName(teacher)
                              }
                             }
                        });
                    }
                    else
                    {
                        var studentReservation1v1TeacherList = new List<StudentReservation1v1Teacher>();
                        foreach (var itemTeacherId in allTeacherIds)
                        {
                            var teacher = await ComBusiness.GetUser(tempBoxUser, _userDAL, itemTeacherId);
                            if (teacher == null)
                            {
                                continue;
                            }
                            var myTeacherSchooltimeConfig = await _teacherSchooltimeConfigDAL.TeacherSchooltimeConfigGet(itemTeacherId);
                            if (myTeacherSchooltimeConfig == null)
                            {
                                continue;
                            }
                            studentReservation1v1TeacherList.Add(new StudentReservation1v1Teacher()
                            {
                                CId = itemTeacherId,
                                ClassId = itemLog.ClassId,
                                Name = ComBusiness2.GetParentTeacherName(teacher)
                            });
                        }
                        if (!studentReservation1v1TeacherList.Any())
                        {
                            continue;
                        }
                        output.AllCourses.Add(new StudentReservation1v1Course()
                        {
                            CId = itemLog.CourseId,
                            Name = myCourse.Item1.Name,
                            Teachers = studentReservation1v1TeacherList
                        });
                    }
                    processCourseIds.Add(itemLog.CourseId);
                }
            }
            return ResponseBase.Success(output);
        }

        public async Task<ResponseBase> StudentReservation1v1DateCheck(StudentReservation1v1DateCheckRequest request)
        {
            var output = new StudentReservation1v1DateCheckOutput()
            {
                ValidDates = new List<string>()
            };
            var teacherSchooltimeConfigBucket = await _teacherSchooltimeConfigDAL.TeacherSchooltimeConfigGet(request.TeacherId);
            if (teacherSchooltimeConfigBucket == null || teacherSchooltimeConfigBucket.TeacherSchooltimeConfigs == null ||
                teacherSchooltimeConfigBucket.TeacherSchooltimeConfigs.Count == 0)
            {
                return ResponseBase.Success(output);
            }
            var details = teacherSchooltimeConfigBucket.EtTeacherSchooltimeConfigDetails;
            var exConfigDate = teacherSchooltimeConfigBucket.TeacherSchooltimeConfigExclude?.AllExcludeDate;
            var allHoliday = await _holidaySettingDAL.GetAllHolidaySetting();
            var currentDate = request.StartOt.Value;
            while (currentDate <= request.EndOt.Value)
            {
                var myDate = currentDate;
                currentDate = currentDate.AddDays(1);
                var exWeek = details.FirstOrDefault(p => p.Week == (int)myDate.DayOfWeek);
                if (exWeek == null)
                {
                    continue;
                }
                if (exWeek.IsJumpHoliday && allHoliday != null && allHoliday.Count > 0)
                {
                    var myHlDay = allHoliday.FirstOrDefault(p => myDate >= p.StartTime && myDate <= p.EndTime);
                    if (myHlDay != null)
                    {
                        continue;
                    }
                }
                if (exConfigDate != null && exConfigDate.Any())
                {
                    var myExDay = exConfigDate.Where(p => p == myDate);
                    if (myExDay.Any())
                    {
                        continue;
                    }
                }
                output.ValidDates.Add(myDate.EtmsToDateString()); ;
            }
            return ResponseBase.Success(output);
        }

        /// <summary>
        /// 预约配置
        /// </summary>
        private ClassReservationSettingView _ruleConfig;

        private Check1v1ClassReservationLimitOutput Check1v1ClassReservationLimit(DateTime classDateTime, DateTime now, int isReservationCourseCount)
        {
            var diffTime = classDateTime - now;
            var diffTotalMinutes = diffTime.TotalMinutes;
            if (diffTotalMinutes <= 1)
            {
                return new Check1v1ClassReservationLimitOutput("截止预约时间限制，无法预约");
            };

            if (_ruleConfig.StartClassReservaLimitType != EmStartClassReservaLimitType.NotLimit) //开始预约时间
            {
                switch (_ruleConfig.StartClassReservaLimitType)
                {
                    case EmStartClassReservaLimitType.LimitHour:
                        var tempLimitMinutes = _ruleConfig.StartClassReservaLimitValue * 60;
                        if (diffTotalMinutes > tempLimitMinutes)
                        {
                            return new Check1v1ClassReservationLimitOutput("开始预约时间限制，无法预约");
                        }
                        break;
                    case EmStartClassReservaLimitType.LimitDay:
                        var startReservationDate = classDateTime.AddDays(-_ruleConfig.StartClassReservaLimitValue).Date;
                        if (now < startReservationDate)
                        {
                            return new Check1v1ClassReservationLimitOutput("开始预约时间限制，无法预约");
                        }
                        if (now.Date == startReservationDate && _ruleConfig.StartClassReservaLimitTimeValue > 0) //同一天 判断时间
                        {
                            if (EtmsHelper.GetTimeHourAndMinuteDesc(now) < _ruleConfig.StartClassReservaLimitTimeValue)
                            {
                                return new Check1v1ClassReservationLimitOutput("开始预约时间限制，无法预约");
                            }
                        }
                        break;
                }
            }

            if (_ruleConfig.DeadlineClassReservaLimitType != EmDeadlineClassReservaLimitType.NotLimit)//截止预约时间
            {
                switch (_ruleConfig.DeadlineClassReservaLimitType)
                {
                    case EmDeadlineClassReservaLimitType.LimitMinute:
                        if (diffTotalMinutes < _ruleConfig.DeadlineClassReservaLimitValue)
                        {
                            return new Check1v1ClassReservationLimitOutput("截止预约时间限制，无法预约");
                        }
                        break;
                    case EmDeadlineClassReservaLimitType.LimitHour:
                        var tempLimitHourValue = _ruleConfig.DeadlineClassReservaLimitValue * 60;
                        if (diffTotalMinutes < tempLimitHourValue)
                        {
                            return new Check1v1ClassReservationLimitOutput("截止预约时间限制，无法预约");
                        }
                        break;
                    case EmDeadlineClassReservaLimitType.LimitDay:
                        var limitReservaDateTime = EtmsHelper.GetTime(classDateTime.AddDays(-_ruleConfig.DeadlineClassReservaLimitValue),
                            _ruleConfig.DeadlineClassReservaLimitDayTimeValue);
                        if (now > limitReservaDateTime)
                        {
                            return new Check1v1ClassReservationLimitOutput("截止预约时间限制，无法预约");
                        }
                        break;
                }
            }

            if (_ruleConfig.MaxCountClassReservaLimitType != EmMaxCountClassReservaLimitType.NotLimit) //预约次数限制
            {
                if (isReservationCourseCount >= _ruleConfig.MaxCountClassReservaLimitValue)
                {
                    return new Check1v1ClassReservationLimitOutput("预约次数限制，无法预约");
                }
            }

            return new Check1v1ClassReservationLimitOutput()
            {
                IsCanReservation = true
            };
        }

        public async Task<ResponseBase> StudentReservation1v1LessonsGet(StudentReservation1v1LessonsGetRequest request)
        {
            var classBucket = await _classDAL.GetClassBucket(request.ClassId);
            if (classBucket == null || classBucket.EtClass == null)
            {
                return ResponseBase.Success(new StudentReservation1v1LessonsGetOutput("班级不存在"));
            }
            var myClass = classBucket.EtClass;
            if (myClass.ReservationType == EmBool.False)
            {
                return ResponseBase.Success(new StudentReservation1v1LessonsGetOutput("未开通约课"));
            }
            if (myClass.DurationHour == 0 && myClass.DurationMinute == 0)
            {
                return ResponseBase.Success(new StudentReservation1v1LessonsGetOutput("未开通约课"));
            }
            var teacherSchooltimeConfigBucket = await _teacherSchooltimeConfigDAL.TeacherSchooltimeConfigGet(request.TeacherId);
            if (teacherSchooltimeConfigBucket == null || teacherSchooltimeConfigBucket.TeacherSchooltimeConfigs == null ||
                teacherSchooltimeConfigBucket.TeacherSchooltimeConfigs.Count == 0)
            {
                return ResponseBase.Success(new StudentReservation1v1LessonsGetOutput("未设置老师上课时间"));
            }
            var myClassDate = request.ClassOt.Value;
            var details = teacherSchooltimeConfigBucket.EtTeacherSchooltimeConfigDetails;
            var exConfigDate = teacherSchooltimeConfigBucket.TeacherSchooltimeConfigExclude?.AllExcludeDate;
            var thisDataConfig = details.Where(p => p.Week == (byte)myClassDate.DayOfWeek);
            if (!thisDataConfig.Any())
            {
                return ResponseBase.Success(new StudentReservation1v1LessonsGetOutput("未查询到当天的课"));
            }
            if (exConfigDate != null && exConfigDate.Any())
            {
                var myExDay = exConfigDate.Where(p => p == myClassDate);
                if (myExDay.Any())
                {
                    return ResponseBase.Success(new StudentReservation1v1LessonsGetOutput("该日期无法约课"));
                }
            }
            var allHoliday = await _holidaySettingDAL.GetAllHolidaySetting();
            var isHoliday = false;
            if (allHoliday != null && allHoliday.Any())
            {
                var myHlDay = allHoliday.FirstOrDefault(p => myClassDate >= p.StartTime && myClassDate <= p.EndTime);
                if (myHlDay != null)
                {
                    isHoliday = true;
                }
            }

            var now = DateTime.Now;
            var minDateTime = myClassDate.AddMinutes(-1);  //至少要提前一分钟预约
            var classMinutes = myClass.DurationHour * 60 + myClass.DurationMinute;
            var lessonsItems = new List<StudentReservation1v1LessonsItem>();
            var myAllClassTimes = await _classTimesDAL.GetClassTimes(request.TeacherId, request.StudentId, myClassDate);
            var myOneToOneClassTimes = await _classTimesDAL.GetStudentOneToOneClassTimes(request.ClassId, myClassDate);
            var sameCount = await _classTimesDAL.ClassTimesReservationLogGetCount(request.CourseId, request.StudentId, now);
            _ruleConfig = await _appConfig2BLL.GetClassReservationSetting();
            foreach (var itemConfig in thisDataConfig)
            {
                if (itemConfig.IsJumpHoliday && isHoliday)
                {
                    continue;
                }
                var limitEndTime = EtmsHelper3.GetDateTime(myClassDate, itemConfig.EndTime);
                var myStartTime = EtmsHelper3.GetDateTime(myClassDate, itemConfig.StartTime);
                var myEndTime = myStartTime.AddMinutes(classMinutes);
                while (myEndTime <= limitEndTime)
                {
                    var myItem = new StudentReservation1v1LessonsItem();
                    myItem.StartTime = EtmsHelper.GetTimeHourAndMinuteDesc(myStartTime);
                    myItem.EndTime = EtmsHelper.GetTimeHourAndMinuteDesc(myEndTime);
                    myItem.Desc = EtmsHelper.GetTimeDesc(myItem.StartTime, myItem.EndTime);
                    var status = EmStudentReservation1v1LessonsStatus.Normal;
                    var isProcessStatus = false;
                    if (myOneToOneClassTimes != null && myOneToOneClassTimes.Any())
                    {
                        var thisClass = myOneToOneClassTimes.FirstOrDefault(p => p.StartTime == myItem.StartTime
                        && p.EndTime == myItem.EndTime && p.Teachers.Contains($",{request.TeacherId},"));
                        if (thisClass != null)
                        {
                            if (thisClass.Status == EmClassTimesStatus.BeRollcall)
                            {
                                status = EmStudentReservation1v1LessonsStatus.IsRollcall;
                                isProcessStatus = true;
                            }
                            else
                            {
                                status = EmStudentReservation1v1LessonsStatus.IsReservation;
                                isProcessStatus = true;
                            }
                        }
                    }
                    if (!isProcessStatus)
                    {
                        if (minDateTime >= myStartTime)
                        {
                            status = EmStudentReservation1v1LessonsStatus.Invalid;
                            isProcessStatus = true;
                        }
                        else
                        {
                            //预约配置判断
                            var checkRuleLimit = Check1v1ClassReservationLimit(myStartTime, now, sameCount);
                            if (!checkRuleLimit.IsCanReservation)
                            {
                                status = EmStudentReservation1v1LessonsStatus.Invalid;
                                isProcessStatus = true;
                            }
                        }
                    }
                    if (!isProcessStatus) //判断老师、学员上课时间重叠
                    {
                        if (myAllClassTimes.Any())
                        {
                            var overlappingLog = myAllClassTimes.FirstOrDefault(p => !(p.StartTime >= myItem.EndTime || p.EndTime <= myItem.StartTime));
                            if (overlappingLog != null)
                            {
                                status = EmStudentReservation1v1LessonsStatus.Occupy;
                                isProcessStatus = true;
                            }
                        }
                    }
                    myItem.Status = status;
                    lessonsItems.Add(myItem);

                    if (myClass.DunIntervalMinute > 0)
                    {
                        myStartTime = myEndTime.AddMinutes(myClass.DunIntervalMinute);
                    }
                    else
                    {
                        myStartTime = myEndTime;
                    }
                    myEndTime = myStartTime.AddMinutes(classMinutes);
                }
            }
            if (lessonsItems.Count == 0)
            {
                return ResponseBase.Success(new StudentReservation1v1LessonsGetOutput("未查询到符合的课次"));
            }
            return ResponseBase.Success(new StudentReservation1v1LessonsGetOutput()
            {
                IsSuccess = true,
                Items = lessonsItems.OrderBy(j => j.StartTime)
            });
        }

        public async Task<ResponseBase> StudentReservation1v1LessonsSubmit(StudentReservation1v1LessonsSubmitRequest request)
        {
            var classBucket = await _classDAL.GetClassBucket(request.ClassId);
            if (classBucket == null || classBucket.EtClass == null)
            {
                return ResponseBase.CommonError("班级不存在");
            }
            var myClass = classBucket.EtClass;
            var now = DateTime.Now;
            var myStartTime = EtmsHelper3.GetDateTime(request.ClassOt.Value, request.StartTime);
            var sameCount = await _classTimesDAL.ClassTimesReservationLogGetCount(request.CourseId, request.StudentId, now);
            _ruleConfig = await _appConfig2BLL.GetClassReservationSetting();
            var checkRuleLimit = Check1v1ClassReservationLimit(myStartTime, now, sameCount);
            if (!checkRuleLimit.IsCanReservation)
            {
                return ResponseBase.CommonError(checkRuleLimit.ErrMsg);
            }

            var myAllClassTimes = await _classTimesDAL.GetClassTimes(request.TeacherId, request.StudentId, request.ClassOt.Value);
            var overlappingLog = myAllClassTimes.FirstOrDefault(p => !(p.StartTime >= request.EndTime || p.EndTime <= request.StartTime));
            if (overlappingLog != null)
            {
                return ResponseBase.CommonError("此时间段已存在课次");
            }

            var strStudentIds = $",{request.StudentId},";
            var classTimes = new EtClassTimes()
            {
                ClassContent = string.Empty,
                ClassId = request.ClassId,
                ClassOt = request.ClassOt.Value,
                ClassRecordId = null,
                ClassRoomIds = null,
                ClassRoomIdsIsAlone = false,
                ClassType = myClass.Type,
                CourseList = $",{request.CourseId},",
                CourseListIsAlone = false,
                EndTime = request.EndTime,
                IsDeleted = EmIsDeleted.Normal,
                LimitStudentNums = null,
                LimitStudentNumsIsAlone = false,
                LimitStudentNumsType = EmLimitStudentNumsType.CanOverflow,
                ReservationType = EmBool.True,
                RuleId = 0,
                StartTime = request.StartTime,
                Status = EmClassTimesStatus.UnRollcall,
                StudentCount = 1,
                StudentIdsClass = strStudentIds,
                StudentIdsReservation = strStudentIds,
                StudentIdsTemp = null,
                StudentTempCount = 0,
                TeacherNum = 1,
                Teachers = $",{request.TeacherId},",
                TeachersIsAlone = true,
                TenantId = request.LoginTenantId,
                Week = (byte)request.ClassOt.Value.DayOfWeek
            };
            await _classTimesDAL.AddClassTimes(classTimes);

            await _classTimesDAL.ClassTimesReservationLogAdd(new EtClassTimesReservationLog()
            {
                StudentId = request.StudentId,
                ClassTimesId = classTimes.Id,
                ClassId = classTimes.ClassId,
                CourseId = request.CourseId,
                ClassOt = classTimes.ClassOt,
                IsDeleted = classTimes.IsDeleted,
                StartTime = classTimes.StartTime,
                Week = classTimes.Week,
                RuleId = classTimes.RuleId,
                Status = EmClassTimesReservationLogStatus.Normal,
                EndTime = classTimes.EndTime,
                TenantId = classTimes.TenantId,
                CreateOt = DateTime.Now
            });

            var tempClassTimesStudent = new EtClassTimesStudent()
            {
                ClassOt = classTimes.ClassOt,
                ClassId = classTimes.ClassId,
                ClassTimesId = classTimes.Id,
                CourseId = request.CourseId,
                IsDeleted = EmIsDeleted.Normal,
                IsReservation = EmBool.True,
                RuleId = 0,
                Status = EmClassTimesStatus.UnRollcall,
                StudentId = request.StudentId,
                StudentTryCalssLogId = null,
                StudentType = EmClassStudentType.ClassStudent,
                TenantId = request.LoginTenantId
            };
            _eventPublisher.Publish(new SyncClassTimesStudentEvent(request.LoginTenantId)
            {
                ClassTimesId = classTimes.Id
            });
            _eventPublisher.Publish(new NoticeStudentReservationEvent(request.LoginTenantId)
            {
                ClassTimesStudent = tempClassTimesStudent,
                OpType = NoticeStudentReservationOpType.Success
            });

            await _studentOperationLogDAL.AddStudentLog(request.StudentId, request.LoginTenantId,
                $"预约上课(1v1)-班级:{myClass.Name},课次:{classTimes.ClassOt.EtmsToDateString()}({EtmsHelper.GetTimeDesc(classTimes.StartTime, classTimes.EndTime)})", EmStudentOperationLogType.StudentReservation);
            return ResponseBase.Success();
        }
    }
}
