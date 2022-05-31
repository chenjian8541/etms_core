using ETMS.Event.DataContract;
using ETMS.IBusiness;
using ETMS.IDataAccess;
using ETMS.LOG;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using ETMS.IEventProvider;
using ETMS.Business.Common;
using ETMS.Entity.Database.Source;
using ETMS.ExternalService.Contract;
using ETMS.Entity.ExternalService.Dto.Request;
using ETMS.Utility;
using ETMS.Entity.Enum;
using ETMS.Entity.Config;
using ETMS.IDataAccess.EtmsManage;
using ETMS.IBusiness.Wechart;
using ETMS.Business.WxCore;
using ETMS.Entity.Enum.EtmsManage;
using ETMS.IBusiness.EventConsumer;

namespace ETMS.Business
{
    public class StudentSendNoticeBLL : SendStudentNoticeBase, IStudentSendNoticeBLL
    {
        private readonly ITenantConfigDAL _tenantConfigDAL;

        private readonly ITempDataCacheDAL _tempDataCacheDAL;

        private readonly IJobAnalyzeDAL _jobAnalyzeDAL;

        private readonly IEventPublisher _eventPublisher;

        private readonly IStudentDAL _studentDAL;

        private readonly ICourseDAL _courseDAL;

        private readonly IClassRoomDAL _classRoomDAL;

        private readonly ISmsService _smsService;

        private readonly IClassDAL _classDAL;

        private readonly ITempStudentClassNoticeDAL _tempStudentClassNoticeDAL;

        private readonly IStudentCourseDAL _studentCourseDAL;

        private readonly IWxService _wxService;

        private readonly IAppConfigurtaionServices _appConfigurtaionServices;

        private readonly IUserDAL _userDAL;

        private readonly IActiveWxMessageDAL _activeWxMessageDAL;

        private readonly IStudentCheckOnLogDAL _studentCheckOnLogDAL;

        private readonly ISysSmsTemplate2BLL _sysSmsTemplate2BLL;

        private readonly IStudentLeaveApplyLogDAL _studentLeaveApplyLogDAL;

        public StudentSendNoticeBLL(ITenantConfigDAL tenantConfigDAL, ITempDataCacheDAL tempDataCacheDAL, IJobAnalyzeDAL jobAnalyzeDAL,
            IEventPublisher eventPublisher, IStudentDAL studentDAL, ICourseDAL courseDAL, IClassRoomDAL classRoomDAL, ISmsService smsService,
            IClassDAL classDAL, ITempStudentClassNoticeDAL tempStudentClassNoticeDAL, IStudentCourseDAL studentCourseDAL,
            IWxService wxService, IAppConfigurtaionServices appConfigurtaionServices,
            IStudentWechatDAL studentWechatDAL, IUserDAL userDAL, ISysTenantDAL sysTenantDAL,
            IComponentAccessBLL componentAccessBLL, IActiveWxMessageDAL activeWxMessageDAL, IStudentCheckOnLogDAL studentCheckOnLogDAL,
            ISysSmsTemplate2BLL sysSmsTemplate2BLL, ITenantLibBLL tenantLibBLL,
            IStudentLeaveApplyLogDAL studentLeaveApplyLogDAL)
            : base(studentWechatDAL, componentAccessBLL, sysTenantDAL, tenantLibBLL)
        {
            this._tenantConfigDAL = tenantConfigDAL;
            this._tempDataCacheDAL = tempDataCacheDAL;
            this._jobAnalyzeDAL = jobAnalyzeDAL;
            this._eventPublisher = eventPublisher;
            this._studentDAL = studentDAL;
            this._courseDAL = courseDAL;
            this._classRoomDAL = classRoomDAL;
            this._smsService = smsService;
            this._classDAL = classDAL;
            this._tempStudentClassNoticeDAL = tempStudentClassNoticeDAL;
            this._studentCourseDAL = studentCourseDAL;
            this._wxService = wxService;
            this._appConfigurtaionServices = appConfigurtaionServices;
            this._userDAL = userDAL;
            this._activeWxMessageDAL = activeWxMessageDAL;
            this._studentCheckOnLogDAL = studentCheckOnLogDAL;
            this._sysSmsTemplate2BLL = sysSmsTemplate2BLL;
            this._studentLeaveApplyLogDAL = studentLeaveApplyLogDAL;
        }

        public void InitTenantId(int tenantId)
        {
            this._tenantLibBLL.InitTenantId(tenantId);
            this._sysSmsTemplate2BLL.InitTenantId(tenantId);
            this.InitDataAccess(tenantId, _tenantConfigDAL, _jobAnalyzeDAL, _studentDAL, _courseDAL, _classRoomDAL, _classDAL,
                _tempStudentClassNoticeDAL, _studentCourseDAL,
                _studentWechatDAL, _userDAL, _activeWxMessageDAL, _studentCheckOnLogDAL, _studentLeaveApplyLogDAL);
        }

        public async Task NoticeStudentsOfClassBeforeDayTenant(NoticeStudentsOfClassBeforeDayTenantEvent request)
        {
            var tenantConfig = await _tenantConfigDAL.GetTenantConfig();
            if (!tenantConfig.StudentNoticeConfig.StartClassDayBeforeIsOpen) //是否开启了提前一天提醒
            {
                return;
            }
            var noticeTime = tenantConfig.StudentNoticeConfig.StartClassDayBeforeTimeValue;
            if (request.NowTime < noticeTime)  //提醒时间限制
            {
                return;
            }
            var hisBucket = _tempDataCacheDAL.GetNoticeStudentsOfClassDayBucket(request.TenantId, request.ClassOt);
            if (hisBucket != null)
            {
                Log.Info($"[NoticeStudentsOfClassBeforeDayTenant]重复收到处理请求:TenantId:{request.TenantId},ClassOt:{request.ClassOt}", this.GetType());
                return;
            }
            Log.Info($"[NoticeStudentsOfClassBeforeDayTenant]准备处理上课前一天上课提醒：TenantId:{request.TenantId}", this.GetType());
            var classTimes = await _jobAnalyzeDAL.GetClassTimesUnRollcall(request.ClassOt);
            if (classTimes != null && classTimes.Any())
            {
                var _studentLeaveApplyLogs = await _studentLeaveApplyLogDAL.GetStudentLeaveApplyPassLog(request.ClassOt);
                foreach (var p in classTimes)
                {
                    _eventPublisher.Publish(new NoticeStudentsOfClassBeforeDayTimesEvent(request.TenantId)
                    {
                        ClassTimes = p,
                        IsSendSms = tenantConfig.StudentNoticeConfig.StartClassSms,
                        IsSendWeChat = tenantConfig.StudentNoticeConfig.StartClassWeChat,
                        WeChatNoticeRemark = tenantConfig.StudentNoticeConfig.WeChatNoticeRemark,
                        LeaveApplyLogs = _studentLeaveApplyLogs
                    });
                }
            }
            _tempDataCacheDAL.SetNoticeStudentsOfClassDayBucket(request.TenantId, request.ClassOt);
        }

        public async Task NoticeStudentsOfClassBeforeDayClassTimes(NoticeStudentsOfClassBeforeDayTimesEvent request)
        {
            if (!request.IsSendSms && !request.IsSendWeChat)
            {
                Log.Info($"[NoticeStudentsOfClassBeforeDayClassTimes]未开启发送上课提醒服务:TenantId:{request.TenantId},ClassTimesId:{request.ClassTimes.Id}", this.GetType());
                return;
            }
            var classTimesStudents = await _jobAnalyzeDAL.GetClassTimesStudent(request.ClassTimes.Id);
            var classBucket = await _classDAL.GetClassBucket(request.ClassTimes.ClassId);
            if (classBucket == null)
            {
                Log.Info($"[NoticeStudentsOfClassBeforeDayClassTimes]未找到对应的班级信息:TenantId:{request.TenantId},ClassTimesId:{request.ClassTimes.Id}", this.GetType());
                return;
            }
            var classStudent = classBucket.EtClassStudents;
            if (!classTimesStudents.Any() && !classStudent.Any())
            {
                Log.Info($"[NoticeStudentsOfClassBeforeDayClassTimes]未查询到要提醒的学生:TenantId:{request.TenantId},ClassTimesId:{request.ClassTimes.Id}", this.GetType());
                return;
            }
            await this.InitNoticeConfig(EmNoticeConfigScenesType.StartClass);
            if (this.CheckLimitNoticeClass(request.ClassTimes.ClassId))
            {
                return;
            }

            var studentLeaveCheck = new StudentIsLeaveCheck(request.LeaveApplyLogs);
            var tempBoxCourse = new DataTempBox<EtCourse>();
            var stringClassRoom = string.Empty;
            if (!string.IsNullOrEmpty(request.ClassTimes.ClassRoomIds))
            {
                var allClassRoom = await _classRoomDAL.GetAllClassRoom();
                stringClassRoom = ComBusiness.GetClassRoomDesc(allClassRoom, request.ClassTimes.ClassRoomIds);
            }
            var smsReq = new NoticeStudentsOfClassBeforeDayRequest(await GetNoticeRequestBase(request.TenantId, request.IsSendWeChat))
            {
                ClassRoom = stringClassRoom,
                ClassTimeDesc = EtmsHelper.GetTimeDesc(request.ClassTimes.StartTime, request.ClassTimes.EndTime, "-"),
                ClassTimeFullDesc = ComBusiness4.GetClassTimesOtDesc(request.ClassTimes),
                Students = new List<NoticeStudentsOfClassBeforeDayStudent>(),
                StartTimeDesc = EtmsHelper.GetTimeDesc(request.ClassTimes.StartTime)
            };
            var wxConfig = _appConfigurtaionServices.AppSettings.WxConfig;
            if (request.IsSendWeChat)
            {
                smsReq.TemplateIdShort = wxConfig.TemplateNoticeConfig.NoticeStudentsOfClass;
                smsReq.Remark = request.WeChatNoticeRemark;
            }
            if (classTimesStudents != null && classTimesStudents.Any())
            {
                foreach (var p in classTimesStudents)
                {
                    if (this.CheckLimitNoticeCourse(p.CourseId))
                    {
                        continue;
                    }
                    if (this.CheckLimitNoticeStudent(p.StudentId))
                    {
                        continue;
                    }
                    var studentBucket = await _studentDAL.GetStudent(p.StudentId);
                    if (studentBucket == null || studentBucket.Student == null)
                    {
                        continue;
                    }
                    var student = studentBucket.Student;
                    if (string.IsNullOrEmpty(student.Phone))
                    {
                        continue;
                    }
                    if (studentLeaveCheck.IsCheckStudentIsLeave(request.ClassTimes.StartTime,
                        request.ClassTimes.EndTime, p.StudentId, request.ClassTimes.ClassOt))
                    {
                        continue;
                    }
                    var myStudentCourse = await _studentCourseDAL.GetStudentCourse(student.Id, p.CourseId);
                    if (ComBusiness3.IsStopOfClass(myStudentCourse))
                    {
                        continue;
                    }
                    var courseName = await ComBusiness.GetCourseName(tempBoxCourse, _courseDAL, p.CourseId);
                    smsReq.Students.Add(new NoticeStudentsOfClassBeforeDayStudent()
                    {
                        StudentId = student.Id,
                        CourseName = courseName,
                        Phone = student.Phone,
                        StudentName = student.Name,
                        OpendId = await GetOpenId(request.IsSendWeChat, student.Phone)
                    });
                    if (!string.IsNullOrEmpty(student.PhoneBak) && EtmsHelper.IsMobilePhone(student.PhoneBak))
                    {
                        smsReq.Students.Add(new NoticeStudentsOfClassBeforeDayStudent()
                        {
                            StudentId = student.Id,
                            CourseName = courseName,
                            Phone = student.PhoneBak,
                            StudentName = student.Name,
                            OpendId = await GetOpenId(request.IsSendWeChat, student.PhoneBak)
                        });
                    }
                }
            }
            if (classStudent != null && classStudent.Any())
            {
                foreach (var p in classStudent)
                {
                    if (this.CheckLimitNoticeCourse(p.CourseId))
                    {
                        continue;
                    }
                    if (this.CheckLimitNoticeStudent(p.StudentId))
                    {
                        continue;
                    }
                    var studentBucket = await _studentDAL.GetStudent(p.StudentId);
                    if (studentBucket == null || studentBucket.Student == null)
                    {
                        continue;
                    }
                    var student = studentBucket.Student;
                    if (string.IsNullOrEmpty(student.Phone))
                    {
                        continue;
                    }
                    if (studentLeaveCheck.IsCheckStudentIsLeave(request.ClassTimes.StartTime,
                        request.ClassTimes.EndTime, p.StudentId, request.ClassTimes.ClassOt))
                    {
                        continue;
                    }
                    var myStudentCourse = await _studentCourseDAL.GetStudentCourse(student.Id, p.CourseId);
                    if (ComBusiness3.IsStopOfClass(myStudentCourse))
                    {
                        continue;
                    }
                    var courseName = await ComBusiness.GetCourseName(tempBoxCourse, _courseDAL, p.CourseId);
                    smsReq.Students.Add(new NoticeStudentsOfClassBeforeDayStudent()
                    {
                        StudentId = student.Id,
                        CourseName = courseName,
                        Phone = student.Phone,
                        StudentName = student.Name,
                        OpendId = await GetOpenId(request.IsSendWeChat, student.Phone)
                    });
                    if (!string.IsNullOrEmpty(student.PhoneBak) && EtmsHelper.IsMobilePhone(student.PhoneBak))
                    {
                        smsReq.Students.Add(new NoticeStudentsOfClassBeforeDayStudent()
                        {
                            StudentId = student.Id,
                            CourseName = courseName,
                            Phone = student.PhoneBak,
                            StudentName = student.Name,
                            OpendId = await GetOpenId(request.IsSendWeChat, student.PhoneBak)
                        });
                    }
                }
            }
            if (smsReq.Students.Any())
            {
                if (request.IsSendSms)
                {
                    smsReq.SmsTemplate = await _sysSmsTemplate2BLL.GetSmsTemplate(request.TenantId, EmSysSmsTemplateType.NoticeStudentsOfClassBeforeDay);
                    await _smsService.NoticeStudentsOfClassBeforeDay(smsReq);
                }
                if (request.IsSendWeChat)
                {
                    _wxService.NoticeStudentsOfClassBeforeDay(smsReq);
                }
            }
        }

        public async Task NoticeStudentsOfClassTodayGenerate(NoticeStudentsOfClassTodayGenerateEvent request)
        {
            Log.Info($"[NoticeStudentsOfClassTodayGenerate]准备提前生成学员上课通知数据：TenantId:{request.TenantId}", this.GetType());
            await _tempStudentClassNoticeDAL.GenerateTempStudentClassNotice(request.ClassOt);
        }

        public async Task NoticeStudentsOfClassTodayTenant(NoticeStudentsOfClassTodayTenantEvent request)
        {
            var tenantConfig = await _tenantConfigDAL.GetTenantConfig();
            if (!tenantConfig.StudentNoticeConfig.StartClassBeforeMinuteIsOpen) //是否开启当天提醒
            {
                return;
            }
            var beforeMinute = tenantConfig.StudentNoticeConfig.StartClassBeforeMinuteValue;
            var classNoticeData = await _tempStudentClassNoticeDAL.GetTempStudentClassNotice(request.ClassOt);
            if (classNoticeData != null && classNoticeData.Count > 0)
            {
                var studentLeaveApplyLogs = await _studentLeaveApplyLogDAL.GetStudentLeaveApplyPassLog(request.ClassOt);
                var ids = new List<long>();
                foreach (var p in classNoticeData)
                {
                    var diffMinute = p.ClassTime.Subtract(request.NowTime).TotalMinutes;  //上课时间和当前时间相差的分钟数
                    if (diffMinute <= 0)
                    {
                        Log.Warn($"[NoticeStudentsOfClassTodayTenant]超时提醒:TenantId:{request.TenantId},ClassTimesId:{p.ClassTimesId},ClassOt:{request.ClassOt}", this.GetType());
                        ids.Add(p.Id);
                        _eventPublisher.Publish(new NoticeStudentsOfClassTodayClassTimesEvent(request.TenantId)
                        {
                            IsSendSms = tenantConfig.StudentNoticeConfig.StartClassSms,
                            IsSendWeChat = tenantConfig.StudentNoticeConfig.StartClassWeChat,
                            ClassTimesId = p.ClassTimesId,
                            WeChatNoticeRemark = tenantConfig.StudentNoticeConfig.WeChatNoticeRemark,
                            LeaveApplyLogs = studentLeaveApplyLogs
                        });
                    }
                    else if (diffMinute - beforeMinute <= 5)  //5分钟的容错时间
                    {
                        ids.Add(p.Id);
                        _eventPublisher.Publish(new NoticeStudentsOfClassTodayClassTimesEvent(request.TenantId)
                        {
                            IsSendSms = tenantConfig.StudentNoticeConfig.StartClassSms,
                            IsSendWeChat = tenantConfig.StudentNoticeConfig.StartClassWeChat,
                            ClassTimesId = p.ClassTimesId,
                            WeChatNoticeRemark = tenantConfig.StudentNoticeConfig.WeChatNoticeRemark,
                            LeaveApplyLogs = studentLeaveApplyLogs
                        });
                    }
                }
                if (ids.Count > 0)
                {
                    await _tempStudentClassNoticeDAL.SetProcessed(ids, request.ClassOt);
                }
            }
            else
            {
                Log.Info($"[NoticeStudentsOfClassTodayTenant]未查询到需要提醒的课次信息:TenantId:{request.TenantId},ClassOt:{request.ClassOt}", this.GetType());
            }
        }

        public async Task NoticeStudentsOfClassTodayClassTimes(NoticeStudentsOfClassTodayClassTimesEvent request)
        {
            if (!request.IsSendSms && !request.IsSendWeChat)
            {
                Log.Info($"[NoticeStudentsOfClassTodayClassTimes]未开启发送上课提醒服务:TenantId:{request.TenantId},ClassTimesId:{request.ClassTimesId}", this.GetType());
                return;
            }
            var classTimes = await _jobAnalyzeDAL.GetClassTimes(request.ClassTimesId);
            if (classTimes == null)
            {
                Log.Fatal($"[NoticeStudentsOfClassTodayClassTimes]课次不存在", request, this.GetType());
                return;
            }
            if (classTimes.Status == EmClassTimesStatus.BeRollcall)
            {
                Log.Warn($"[NoticeStudentsOfClassTodayClassTimes]已点名,无需发送上课通知:TenantId:{request.TenantId},ClassTimesId:{request.ClassTimesId}", this.GetType());
                return;
            }
            if (classTimes.DataType != EmClassTimesDataType.Normal)
            {
                return;
            }
            var classTimesStudents = await _jobAnalyzeDAL.GetClassTimesStudent(classTimes.Id);
            var classBucket = await _classDAL.GetClassBucket(classTimes.ClassId);
            if (classBucket == null)
            {
                Log.Info($"[NoticeStudentsOfClassTodayClassTimes]未找到对应的班级信息:TenantId:{request.TenantId},ClassTimesId:{classTimes.Id}", this.GetType());
                return;
            }
            var classStudent = classBucket.EtClassStudents;
            if (!classTimesStudents.Any() && !classStudent.Any())
            {
                Log.Info($"[NoticeStudentsOfClassTodayClassTimes]未查询到要提醒的学生:TenantId:{request.TenantId},ClassTimesId:{classTimes.Id}", this.GetType());
                return;
            }
            await this.InitNoticeConfig(EmNoticeConfigScenesType.StartClass);
            if (this.CheckLimitNoticeClass(classTimes.ClassId))
            {
                return;
            }

            var studentLeaveCheck = new StudentIsLeaveCheck(request.LeaveApplyLogs);
            var tempBoxCourse = new DataTempBox<EtCourse>();
            var stringClassRoom = string.Empty;
            if (!string.IsNullOrEmpty(classTimes.ClassRoomIds))
            {
                var allClassRoom = await _classRoomDAL.GetAllClassRoom();
                stringClassRoom = ComBusiness.GetClassRoomDesc(allClassRoom, classTimes.ClassRoomIds);
            }

            var smsReq = new NoticeStudentsOfClassTodayRequest(await GetNoticeRequestBase(request.TenantId, request.IsSendWeChat))
            {
                ClassRoom = stringClassRoom,
                ClassTimeDesc = EtmsHelper.GetTimeDesc(classTimes.StartTime, classTimes.EndTime, "-"),
                ClassTimeFullDesc = ComBusiness4.GetClassTimesOtDesc(classTimes),
                StartTimeDesc = EtmsHelper.GetTimeDesc(classTimes.StartTime),
                Students = new List<NoticeStudentsOfClassTodayStudent>()
            };
            var wxConfig = _appConfigurtaionServices.AppSettings.WxConfig;
            if (request.IsSendWeChat)
            {
                smsReq.TemplateIdShort = wxConfig.TemplateNoticeConfig.NoticeStudentsOfClass;
                smsReq.Remark = request.WeChatNoticeRemark;
            }
            var checkInLog = await _studentCheckOnLogDAL.GetStudentCheckOnLogByClassTimesId(request.ClassTimesId);
            if (classTimesStudents != null && classTimesStudents.Any())
            {
                foreach (var p in classTimesStudents)
                {
                    if (this.CheckLimitNoticeCourse(p.CourseId))
                    {
                        continue;
                    }
                    if (this.CheckLimitNoticeStudent(p.StudentId))
                    {
                        continue;
                    }
                    if (checkInLog != null && checkInLog.Count > 0) //判断已记考勤
                    {
                        var myCheckLog = checkInLog.FirstOrDefault(j => j.StudentId == p.StudentId);
                        if (myCheckLog != null)
                        {
                            continue;
                        }
                    }
                    var studentBucket = await _studentDAL.GetStudent(p.StudentId);
                    if (studentBucket == null || studentBucket.Student == null)
                    {
                        continue;
                    }
                    var student = studentBucket.Student;
                    if (string.IsNullOrEmpty(student.Phone))
                    {
                        continue;
                    }
                    if (studentLeaveCheck.IsCheckStudentIsLeave(classTimes.StartTime, classTimes.EndTime, p.StudentId, classTimes.ClassOt))
                    {
                        continue;
                    }
                    var myStudentCourse = await _studentCourseDAL.GetStudentCourse(student.Id, p.CourseId);
                    if (ComBusiness3.IsStopOfClass(myStudentCourse))
                    {
                        continue;
                    }
                    var courseName = await ComBusiness.GetCourseName(tempBoxCourse, _courseDAL, p.CourseId);
                    smsReq.Students.Add(new NoticeStudentsOfClassTodayStudent()
                    {
                        StudentId = student.Id,
                        CourseName = courseName,
                        Phone = student.Phone,
                        StudentName = student.Name,
                        OpendId = await GetOpenId(request.IsSendWeChat, student.Phone)
                    });
                    if (!string.IsNullOrEmpty(student.PhoneBak) && EtmsHelper.IsMobilePhone(student.PhoneBak))
                    {
                        smsReq.Students.Add(new NoticeStudentsOfClassTodayStudent()
                        {
                            StudentId = student.Id,
                            CourseName = courseName,
                            Phone = student.PhoneBak,
                            StudentName = student.Name,
                            OpendId = await GetOpenId(request.IsSendWeChat, student.PhoneBak)
                        });
                    }
                }
            }
            if (classStudent != null && classStudent.Any())
            {
                foreach (var p in classStudent)
                {
                    if (this.CheckLimitNoticeCourse(p.CourseId))
                    {
                        continue;
                    }
                    if (this.CheckLimitNoticeStudent(p.StudentId))
                    {
                        continue;
                    }
                    if (checkInLog != null && checkInLog.Count > 0) //判断已记考勤
                    {
                        var myCheckLog = checkInLog.FirstOrDefault(j => j.StudentId == p.StudentId);
                        if (myCheckLog != null)
                        {
                            continue;
                        }
                    }
                    var studentBucket = await _studentDAL.GetStudent(p.StudentId);
                    if (studentBucket == null || studentBucket.Student == null)
                    {
                        continue;
                    }
                    var student = studentBucket.Student;
                    if (string.IsNullOrEmpty(student.Phone))
                    {
                        continue;
                    }
                    if (studentLeaveCheck.IsCheckStudentIsLeave(classTimes.StartTime, classTimes.EndTime, p.StudentId, classTimes.ClassOt))
                    {
                        continue;
                    }
                    var myStudentCourse = await _studentCourseDAL.GetStudentCourse(student.Id, p.CourseId);
                    if (ComBusiness3.IsStopOfClass(myStudentCourse))
                    {
                        continue;
                    }
                    var courseName = await ComBusiness.GetCourseName(tempBoxCourse, _courseDAL, p.CourseId);
                    smsReq.Students.Add(new NoticeStudentsOfClassTodayStudent()
                    {
                        StudentId = student.Id,
                        CourseName = courseName,
                        Phone = student.Phone,
                        StudentName = student.Name,
                        OpendId = await GetOpenId(request.IsSendWeChat, student.Phone)
                    });
                    if (!string.IsNullOrEmpty(student.PhoneBak) && EtmsHelper.IsMobilePhone(student.PhoneBak))
                    {
                        smsReq.Students.Add(new NoticeStudentsOfClassTodayStudent()
                        {
                            StudentId = student.Id,
                            CourseName = courseName,
                            Phone = student.PhoneBak,
                            StudentName = student.Name,
                            OpendId = await GetOpenId(request.IsSendWeChat, student.PhoneBak)
                        });
                    }
                }
            }
            if (smsReq.Students.Any())
            {
                if (request.IsSendSms)
                {
                    smsReq.SmsTemplate = await _sysSmsTemplate2BLL.GetSmsTemplate(request.TenantId, EmSysSmsTemplateType.NoticeStudentsOfClassToday);
                    await _smsService.NoticeStudentsOfClassToday(smsReq);
                }
                if (request.IsSendWeChat)
                {
                    _wxService.NoticeStudentsOfClassToday(smsReq);
                }
            }
        }

        public async Task NoticeStudentsCheckSign(NoticeStudentsCheckSignEvent request)
        {
            var tenantConfig = await _tenantConfigDAL.GetTenantConfig();
            if (!tenantConfig.StudentNoticeConfig.ClassCheckSignSms && !tenantConfig.StudentNoticeConfig.ClassCheckSignWeChat)
            {
                return;
            }
            var classRecord = await _jobAnalyzeDAL.GetClassRecord(request.ClassRecordId);
            if (classRecord == null)
            {
                Log.Warn($"[NoticeStudentsCheckSign]未找到点名记录,ClassRecordId:{request.ClassRecordId}", this.GetType());
                return;
            }
            var classRecordStudent = await _jobAnalyzeDAL.GetClassRecordStudent(request.ClassRecordId);
            if (classRecordStudent == null || classRecordStudent.Count == 0)
            {
                Log.Warn($"[NoticeStudentsCheckSign]未找到上课学员,ClassRecordId:{request.ClassRecordId}", this.GetType());
                return;
            }
            var classInfo = await _classDAL.GetClassBucket(classRecord.ClassId);
            if (classInfo == null || classInfo.EtClass == null)
            {
                Log.Warn($"[NoticeStudentsCheckSign]未找到上课班级,ClassRecordId:{request.ClassRecordId}", this.GetType());
                return;
            }

            List<EtStudentCheckOnLog> checkInLog = null;
            if (classRecord.ClassTimesId != null)
            {
                checkInLog = await _studentCheckOnLogDAL.GetStudentCheckOnLogByClassTimesId(classRecord.ClassTimesId.Value);
            }

            await this.InitNoticeConfig(EmNoticeConfigScenesType.ClassCheckSign);
            if (this.CheckLimitNoticeClass(classRecord.ClassId))
            {
                return;
            }

            var tenantNo = TenantLib.GetTenantEncrypt(request.TenantId);
            var tempBox = new DataTempBox<EtUser>();
            var req = new NoticeClassCheckSignRequest(await GetNoticeRequestBase(request.TenantId, tenantConfig.StudentNoticeConfig.ClassCheckSignWeChat))
            {
                ClassTimeDesc = $"{classRecord.ClassOt.EtmsToDateString()} {EtmsHelper.GetTimeDesc(classRecord.StartTime, classRecord.EndTime, "-")}",
                Students = new List<NoticeClassCheckSignStudent>(),
                ClassName = classInfo.EtClass.Name,
                TeacherDesc = await ComBusiness.GetParentTeachers(tempBox, _userDAL, classRecord.Teachers)
            };
            var wxConfig = _appConfigurtaionServices.AppSettings.WxConfig;
            if (tenantConfig.StudentNoticeConfig.ClassCheckSignWeChat)
            {
                req.TemplateIdShort = wxConfig.TemplateNoticeConfig.ClassCheckSign;
                req.Remark = tenantConfig.StudentNoticeConfig.WeChatNoticeRemark;
            }
            var isStudentShowClassTimesUnit = tenantConfig.TenantOtherConfig.IsStudentShowClassTimesUnit;
            var studentShowClassTimesUnitValue = tenantConfig.TenantOtherConfig.StudentShowClassTimesUnitValue;
            var tempBoxCourse = new DataTempBox<EtCourse>();
            foreach (var p in classRecordStudent)
            {
                if (this.CheckLimitNoticeCourse(p.CourseId))
                {
                    continue;
                }
                if (this.CheckLimitNoticeStudent(p.StudentId))
                {
                    continue;
                }
                if (checkInLog != null && checkInLog.Count > 0) //判断已记考勤
                {
                    var myCheckLog = checkInLog.FirstOrDefault(j => j.StudentId == p.StudentId);
                    if (myCheckLog != null)
                    {
                        continue;
                    }
                }
                var studentBucket = await _studentDAL.GetStudent(p.StudentId);
                if (studentBucket == null || studentBucket.Student == null)
                {
                    continue;
                }
                var student = studentBucket.Student;
                if (string.IsNullOrEmpty(student.Phone))
                {
                    continue;
                }
                var courseName = await ComBusiness.GetCourseName(tempBoxCourse, _courseDAL, p.CourseId);
                var myStudentCourse = await _studentCourseDAL.GetStudentCourse(p.StudentId, p.CourseId);
                var surplusClassTimesDesc = ComBusiness.GetStudentCourseDesc(myStudentCourse, true,
                    isStudentShowClassTimesUnit, studentShowClassTimesUnitValue);

                var myStudentCourseDetail = await _studentCourseDAL.GetStudentCourseDetail(p.StudentId, p.CourseId);
                var expireDateDesc = ComBusiness.GetStudentCourseExpireDateDesc(myStudentCourseDetail);
                //if (mySurplusClassTimes != null && mySurplusClassTimes.Any())
                //{
                //    var temp = new StringBuilder();
                //    var deClassTimes = mySurplusClassTimes.FirstOrDefault(j => j.DeType == EmDeClassTimesType.ClassTimes);
                //    if (deClassTimes != null && deClassTimes.BuyQuantity > 0)
                //    {
                //        temp.Append(ComBusiness.GetSurplusQuantityDesc(deClassTimes.SurplusQuantity, deClassTimes.SurplusSmallQuantity, deClassTimes.DeType));
                //    }
                //    var deDay = mySurplusClassTimes.FirstOrDefault(j => j.DeType == EmDeClassTimesType.Day);
                //    if (deDay != null && deDay.BuyQuantity > 0)
                //    {
                //        temp.Append(ComBusiness.GetSurplusQuantityDesc(deDay.SurplusQuantity, deDay.SurplusSmallQuantity, deDay.DeType));
                //    }
                //    if (temp.Length == 0)
                //    {
                //        surplusClassTimesDesc = "0课时";
                //    }
                //    else
                //    {
                //        surplusClassTimesDesc = temp.ToString();
                //    }
                //}

                req.Students.Add(new NoticeClassCheckSignStudent()
                {
                    StudentId = student.Id,
                    CourseName = courseName,
                    Name = student.Name,
                    Phone = student.Phone,
                    DeClassTimesDesc = p.DeClassTimes.EtmsToString(),
                    StudentCheckStatusDesc = EmClassStudentCheckStatus.GetClassStudentCheckStatus(p.StudentCheckStatus),
                    StudentCheckStatus = p.StudentCheckStatus,
                    SurplusClassTimesDesc = surplusClassTimesDesc,
                    OpendId = await GetOpenId(tenantConfig.StudentNoticeConfig.ClassCheckSignWeChat, student.Phone),
                    LinkUrl = string.Format(wxConfig.TemplateNoticeConfig.ClassRecordDetailFrontUrl, tenantNo, p.Id),
                    RewardPoints = p.RewardPoints,
                    Points = student.Points,
                    ExTimeDesc = expireDateDesc
                });
                if (!string.IsNullOrEmpty(student.PhoneBak) && EtmsHelper.IsMobilePhone(student.PhoneBak))
                {
                    req.Students.Add(new NoticeClassCheckSignStudent()
                    {
                        StudentId = student.Id,
                        CourseName = courseName,
                        Name = student.Name,
                        Phone = student.PhoneBak,
                        DeClassTimesDesc = p.DeClassTimes.EtmsToString(),
                        StudentCheckStatusDesc = EmClassStudentCheckStatus.GetClassStudentCheckStatus(p.StudentCheckStatus),
                        StudentCheckStatus = p.StudentCheckStatus,
                        SurplusClassTimesDesc = surplusClassTimesDesc,
                        OpendId = await GetOpenId(tenantConfig.StudentNoticeConfig.ClassCheckSignWeChat, student.PhoneBak),
                        LinkUrl = string.Format(wxConfig.TemplateNoticeConfig.ClassRecordDetailFrontUrl, tenantNo, p.Id),
                        RewardPoints = p.RewardPoints,
                        Points = student.Points,
                        ExTimeDesc = expireDateDesc
                    });
                }
            }
            if (req.Students.Count > 0)
            {
                if (tenantConfig.StudentNoticeConfig.ClassCheckSignSms)
                {
                    req.SmsTemplate = await _sysSmsTemplate2BLL.GetSmsTemplate(request.TenantId, EmSysSmsTemplateType.ClassCheckSign);
                    await _smsService.NoticeClassCheckSign(req);
                }
                if (tenantConfig.StudentNoticeConfig.ClassCheckSignWeChat)
                {
                    _wxService.NoticeClassCheckSign(req);
                }
            }
        }

        public async Task NoticeStudentLeaveApply(NoticeStudentLeaveApplyEvent request)
        {
            var tenantConfig = await _tenantConfigDAL.GetTenantConfig();
            if (!tenantConfig.StudentNoticeConfig.StudentAskForLeaveCheckSms && !tenantConfig.StudentNoticeConfig.StudentAskForLeaveCheckWeChat)
            {
                return;
            }
            var studentBucket = await _studentDAL.GetStudent(request.StudentLeaveApplyLog.StudentId);
            if (studentBucket == null || studentBucket.Student == null)
            {
                Log.Warn($"[NoticeStudentLeaveApply]未找到学员信息,StudentLeaveApplyLogId:{request.StudentLeaveApplyLog.Id}", this.GetType());
                return;
            }
            await this.InitNoticeConfig(EmNoticeConfigScenesType.StudentAskForLeaveCheck);
            if (this.CheckLimitNoticeStudent(request.StudentLeaveApplyLog.StudentId))
            {
                return;
            }
            if (await this.CheckLimitNoticeClassOfStudent(request.StudentLeaveApplyLog.StudentId))
            {
                return;
            }

            var student = studentBucket.Student;
            if (string.IsNullOrEmpty(student.Phone))
            {
                return;
            }
            var wxConfig = _appConfigurtaionServices.AppSettings.WxConfig;
            var req = new NoticeStudentLeaveApplyRequest(await GetNoticeRequestBase(request.TenantId, tenantConfig.StudentNoticeConfig.StudentAskForLeaveCheckWeChat));
            req.Students = new List<NoticeStudentLeaveApplyStudent>();
            req.StartTimeDesc = $"{request.StudentLeaveApplyLog.StartDate.EtmsToDateString()} {EtmsHelper.GetTimeDesc(request.StudentLeaveApplyLog.StartTime)}";
            req.EndTimeDesc = $"{request.StudentLeaveApplyLog.EndDate.EtmsToDateString()} {EtmsHelper.GetTimeDesc(request.StudentLeaveApplyLog.EndTime)}";
            req.TimeDesc = $"{req.StartTimeDesc}~{req.EndTimeDesc}";
            if (tenantConfig.StudentNoticeConfig.StudentAskForLeaveCheckWeChat)
            {
                req.TemplateIdShort = wxConfig.TemplateNoticeConfig.StudentLeaveApply;
                req.Url = string.Format(wxConfig.TemplateNoticeConfig.StudentLeaveApplyDetailFrontUrl, request.StudentLeaveApplyLog.Id);
                req.Remark = tenantConfig.StudentNoticeConfig.WeChatNoticeRemark;
            }
            var handleStatusDesc = request.StudentLeaveApplyLog.HandleStatus == EmStudentLeaveApplyHandleStatus.Pass ? "审批通过" : "审批未通过";
            var handleUser = await ComBusiness2.GetParentTeacherName(_userDAL, request.StudentLeaveApplyLog.HandleUser);
            req.Students.Add(new NoticeStudentLeaveApplyStudent()
            {
                StudentId = student.Id,
                HandleStatusDesc = handleStatusDesc,
                Name = student.Name,
                Phone = student.Phone,
                HandleStatus = request.StudentLeaveApplyLog.HandleStatus,
                HandleUser = handleUser,
                OpendId = await GetOpenId(tenantConfig.StudentNoticeConfig.StudentAskForLeaveCheckWeChat, student.Phone)
            });
            if (!string.IsNullOrEmpty(student.PhoneBak) && EtmsHelper.IsMobilePhone(student.PhoneBak))
            {
                req.Students.Add(new NoticeStudentLeaveApplyStudent()
                {
                    StudentId = student.Id,
                    HandleStatusDesc = handleStatusDesc,
                    Name = student.Name,
                    Phone = student.PhoneBak,
                    HandleStatus = request.StudentLeaveApplyLog.HandleStatus,
                    HandleUser = handleUser,
                    OpendId = await GetOpenId(tenantConfig.StudentNoticeConfig.StudentAskForLeaveCheckWeChat, student.PhoneBak)
                });
            }
            if (req.Students.Count > 0)
            {
                if (tenantConfig.StudentNoticeConfig.StudentAskForLeaveCheckSms)
                {
                    req.SmsTemplate = await _sysSmsTemplate2BLL.GetSmsTemplate(request.TenantId, EmSysSmsTemplateType.StudentLeaveApply);
                    await _smsService.NoticeStudentLeaveApply(req);
                }
                if (tenantConfig.StudentNoticeConfig.StudentAskForLeaveCheckWeChat)
                {
                    _wxService.NoticeStudentLeaveApply(req);
                }
            }
        }

        public async Task NoticeStudentContracts(NoticeStudentContractsEvent request)
        {
            var tenantConfig = await _tenantConfigDAL.GetTenantConfig();
            if (!tenantConfig.StudentNoticeConfig.OrderBySms && !tenantConfig.StudentNoticeConfig.OrderByWeChat)
            {
                return;
            }
            var studentBucket = await _studentDAL.GetStudent(request.Order.StudentId);
            if (studentBucket == null || studentBucket.Student == null)
            {
                Log.Warn($"[NoticeStudentContracts]未找到学员信息,OrderId:{request.Order.Id}", this.GetType());
                return;
            }
            await this.InitNoticeConfig(EmNoticeConfigScenesType.OrderBuy);
            if (this.CheckLimitNoticeStudent(request.Order.StudentId))
            {
                return;
            }

            var student = studentBucket.Student;
            if (string.IsNullOrEmpty(student.Phone))
            {
                return;
            }
            var req = new NoticeStudentContractsRequest(await GetNoticeRequestBase(request.TenantId, tenantConfig.StudentNoticeConfig.OrderByWeChat))
            {
                AptSumDesc = request.Order.AptSum.ToString("F2"),
                PaySumDesc = request.Order.PaySum.ToString("F2"),
                TimeDedc = request.Order.Ot.EtmsToDateString(),
                OrderNo = request.Order.No,
                Students = new List<NoticeStudentContractsStudent>()
            };
            var wxConfig = _appConfigurtaionServices.AppSettings.WxConfig;
            if (tenantConfig.StudentNoticeConfig.OrderByWeChat)
            {
                req.TemplateIdShort = wxConfig.TemplateNoticeConfig.StudentContracts;
                req.Url = string.Format(wxConfig.TemplateNoticeConfig.StudentOrderDetailFrontUrl, request.Order.Id);
                req.Remark = tenantConfig.StudentNoticeConfig.WeChatNoticeRemark;
            }

            var buyDesc = new StringBuilder();
            var buyCourse = request.OrderDetails.Where(p => p.ProductType == EmProductType.Course);
            if (buyCourse.Any())
            {
                var tempCount = buyCourse.Count();
                if (tempCount == 1)
                {
                    var course = await _courseDAL.GetCourse(buyCourse.First().ProductId);
                    buyDesc.Append($"报读了课程（{course.Item1.Name}）");
                }
                else
                {
                    buyDesc.Append($"报读了{tempCount}门课程");
                }
            }
            var buyGoods = request.OrderDetails.Where(p => p.ProductType == EmProductType.Goods);
            if (buyGoods.Any())
            {
                var tempCount = buyGoods.Count();
                if (buyDesc.Length > 0)
                {
                    buyDesc.Append($"、{tempCount}件物品");
                }
                else
                {
                    buyDesc.Append($"购买了{tempCount}件物品");
                }
            }
            var buyCost = request.OrderDetails.Where(p => p.ProductType == EmProductType.Cost);
            if (buyCost.Any())
            {
                var tempCount = buyCost.Count();
                if (buyDesc.Length > 0)
                {
                    buyDesc.Append($"、{tempCount}笔费用");
                }
                else
                {
                    buyDesc.Append($"购买了{tempCount}笔费用");
                }
            }
            req.BuyDesc = buyDesc.ToString();
            req.Students.Add(new NoticeStudentContractsStudent()
            {
                StudentId = student.Id,
                Name = student.Name,
                Phone = student.Phone,
                OpendId = await GetOpenId(tenantConfig.StudentNoticeConfig.OrderByWeChat, student.Phone)
            });
            if (!string.IsNullOrEmpty(student.PhoneBak) && EtmsHelper.IsMobilePhone(student.PhoneBak))
            {
                req.Students.Add(new NoticeStudentContractsStudent()
                {
                    StudentId = student.Id,
                    Name = student.Name,
                    Phone = student.PhoneBak,
                    OpendId = await GetOpenId(tenantConfig.StudentNoticeConfig.OrderByWeChat, student.PhoneBak)
                });
            }
            if (req.Students.Count > 0)
            {
                if (tenantConfig.StudentNoticeConfig.OrderBySms)
                {
                    req.SmsTemplate = await _sysSmsTemplate2BLL.GetSmsTemplate(request.TenantId, EmSysSmsTemplateType.StudentContracts);
                    await _smsService.NoticeStudentContracts(req);
                }
                if (tenantConfig.StudentNoticeConfig.OrderByWeChat)
                {
                    _wxService.NoticeStudentContracts(req);
                }
            }
        }

        public async Task NoticeStudentsOfWxMessageConsumerEvent(NoticeStudentsOfWxMessageEvent request)
        {
            if (request.StudentIds == null || request.StudentIds.Count == 0)
            {
                return;
            }
            var wxConfig = _appConfigurtaionServices.AppSettings.WxConfig;
            var tenantConfig = await _tenantConfigDAL.GetTenantConfig();
            var wxMessageDetailIdViews = await _activeWxMessageDAL.GetWxMessageDetailIdView(request.WxMessageAddId, request.StudentIds);
            var req = new WxMessageRequest(await GetNoticeRequestBase(request.TenantId))
            {
                OtDesc = request.Ot.EtmsToMinuteString(),
                Students = new List<WxMessageStudent>()
            };
            req.TemplateIdShort = wxConfig.TemplateNoticeConfig.WxMessage;
            req.Url = string.Empty;
            req.Remark = tenantConfig.StudentNoticeConfig.WeChatNoticeRemark;
            foreach (var p in request.StudentIds)
            {
                var studentBucket = await _studentDAL.GetStudent(p);
                if (studentBucket == null || studentBucket.Student == null)
                {
                    Log.Warn($"[NoticeStudentsOfWxMessageConsumerEvent]未找到学员信息,studentId:{p}", this.GetType());
                    continue;
                }
                var student = studentBucket.Student;
                if (string.IsNullOrEmpty(student.Phone))
                {
                    continue;
                }
                var myWxMessageDetailIdViews = wxMessageDetailIdViews.FirstOrDefault(j => j.StudentId == p);
                if (myWxMessageDetailIdViews == null)
                {
                    Log.Warn($"[NoticeStudentsOfWxMessageConsumerEvent]未找到对应的通知记录,studentId:{p}", this.GetType());
                    continue;
                }
                var url = string.Format(wxConfig.TemplateNoticeConfig.StudentWxMessageDetailUrl, myWxMessageDetailIdViews.Id);
                req.Students.Add(new WxMessageStudent()
                {
                    Name = student.Name,
                    Phone = student.Phone,
                    StudentId = student.Id,
                    OpendId = await GetOpenId(true, student.Phone),
                    Url = url,
                    TitleAdd = request.Title
                });
                if (!string.IsNullOrEmpty(student.PhoneBak) && EtmsHelper.IsMobilePhone(student.PhoneBak))
                {
                    req.Students.Add(new WxMessageStudent()
                    {
                        Name = student.Name,
                        Phone = student.PhoneBak,
                        StudentId = student.Id,
                        OpendId = await GetOpenId(true, student.PhoneBak),
                        Url = url,
                        TitleAdd = request.Title
                    });
                }
            }
            if (req.Students.Count > 0)
            {
                _wxService.WxMessage(req);
            }
        }
    }
}
