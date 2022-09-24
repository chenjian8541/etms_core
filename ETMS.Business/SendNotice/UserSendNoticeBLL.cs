using ETMS.Business.Common;
using ETMS.Entity.Config;
using ETMS.Entity.Database.Source;
using ETMS.Entity.Enum;
using ETMS.Entity.Enum.EtmsManage;
using ETMS.Entity.ExternalService.Dto.Request;
using ETMS.Entity.ExternalService.Dto.Request.User;
using ETMS.Entity.Temp.Compare;
using ETMS.Entity.View;
using ETMS.Event.DataContract;
using ETMS.ExternalService.Contract;
using ETMS.IBusiness;
using ETMS.IBusiness.EventConsumer;
using ETMS.IBusiness.SendNotice;
using ETMS.IBusiness.Wechart;
using ETMS.IDataAccess;
using ETMS.IDataAccess.EtmsManage;
using ETMS.IEventProvider;
using ETMS.LOG;
using ETMS.Utility;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.Business.SendNotice
{
    public class UserSendNoticeBLL : SendUserNoticeBase, IUserSendNoticeBLL
    {
        private readonly IEventPublisher _eventPublisher;

        private readonly ITenantConfigDAL _tenantConfigDAL;

        private readonly ICourseDAL _courseDAL;

        private readonly IClassDAL _classDAL;

        private readonly IJobAnalyzeDAL _jobAnalyzeDAL;

        private readonly ITempUserClassNoticeDAL _tempUserClassNoticeDAL;

        private readonly IClassRoomDAL _classRoomDAL;

        private readonly IAppConfigurtaionServices _appConfigurtaionServices;

        private readonly IUserDAL _userDAL;

        private readonly ISmsService _smsService;

        private readonly IWxService _wxService;

        private readonly IActiveHomeworkDetailDAL _activeHomeworkDetailDAL;

        private readonly IStudentDAL _studentDAL;

        private readonly ISysSmsTemplate2BLL _sysSmsTemplate2BLL;

        private readonly IJobAnalyze2DAL _jobAnalyze2DAL;

        private readonly IStudentCheckOnLogDAL _studentCheckOnLogDAL;

        public UserSendNoticeBLL(IEventPublisher eventPublisher, ITenantConfigDAL tenantConfigDAL, ICourseDAL courseDAL, IClassDAL classDAL,
            IJobAnalyzeDAL jobAnalyzeDAL, ITempUserClassNoticeDAL tempUserClassNoticeDAL, IClassRoomDAL classRoomDAL,
            IUserWechatDAL userWechatDAL, IComponentAccessBLL componentAccessBLL, ISysTenantDAL sysTenantDAL, IAppConfigurtaionServices appConfigurtaionServices,
            IUserDAL userDAL, ISmsService smsService, IWxService wxService, IActiveHomeworkDetailDAL activeHomeworkDetailDAL,
            IStudentDAL studentDAL, ISysSmsTemplate2BLL sysSmsTemplate2BLL, ITenantLibBLL tenantLibBLL, IJobAnalyze2DAL jobAnalyze2DAL,
            IStudentCheckOnLogDAL studentCheckOnLogDAL)
            : base(userWechatDAL, componentAccessBLL, sysTenantDAL, tenantLibBLL)
        {
            this._eventPublisher = eventPublisher;
            this._tenantConfigDAL = tenantConfigDAL;
            this._courseDAL = courseDAL;
            this._classDAL = classDAL;
            this._jobAnalyzeDAL = jobAnalyzeDAL;
            this._tempUserClassNoticeDAL = tempUserClassNoticeDAL;
            this._classRoomDAL = classRoomDAL;
            this._appConfigurtaionServices = appConfigurtaionServices;
            this._userDAL = userDAL;
            this._smsService = smsService;
            this._wxService = wxService;
            this._activeHomeworkDetailDAL = activeHomeworkDetailDAL;
            this._studentDAL = studentDAL;
            this._sysSmsTemplate2BLL = sysSmsTemplate2BLL;
            this._jobAnalyze2DAL = jobAnalyze2DAL;
            this._studentCheckOnLogDAL = studentCheckOnLogDAL;
        }

        public void InitTenantId(int tenantId)
        {
            this._tenantLibBLL.InitTenantId(tenantId);
            this._sysSmsTemplate2BLL.InitTenantId(tenantId);
            this.InitDataAccess(tenantId, _tenantConfigDAL, _courseDAL, _classDAL, _jobAnalyzeDAL, _tempUserClassNoticeDAL, _classRoomDAL,
                _userWechatDAL, _userDAL, _activeHomeworkDetailDAL, _studentDAL, _jobAnalyze2DAL, _studentCheckOnLogDAL);
        }

        public async Task NoticeUserOfClassTodayGenerateConsumerEvent(NoticeUserOfClassTodayGenerateEvent request)
        {
            Log.Info($"[NoticeUserOfClassTodayGenerateConsumerEvent]准备提前生成老师上课通知数据：TenantId:{request.TenantId}", this.GetType());
            await _tempUserClassNoticeDAL.GenerateTempUserClassNotice(request.ClassOt);
        }

        public async Task NoticeTeacherOfClassTodayTenantConsumerEvent(NoticeTeacherOfClassTodayTenantEvent request)
        {
            var tenantConfig = await _tenantConfigDAL.GetTenantConfig();
            if (!tenantConfig.UserNoticeConfig.StartClassSms && !tenantConfig.UserNoticeConfig.StartClassWeChat) //是否开启当天提醒
            {
                return;
            }
            var beforeMinute = tenantConfig.UserNoticeConfig.StartClassBeforeMinuteValue;
            var classNoticeData = await _tempUserClassNoticeDAL.GetTempUserClassNotice(request.ClassOt);
            if (classNoticeData != null && classNoticeData.Count > 0)
            {
                var ids = new List<long>();
                foreach (var p in classNoticeData)
                {
                    var diffMinute = p.ClassTime.Subtract(request.NowTime).TotalMinutes;  //上课时间和当前时间相差的分钟数
                    if (diffMinute <= 0)
                    {
                        Log.Warn($"[NoticeTeacherOfClassTodayTenantConsumerEvent]超时提醒:TenantId:{request.TenantId},ClassTimesId:{p.ClassTimesId},ClassOt:{request.ClassOt}", this.GetType());
                        ids.Add(p.Id);
                        _eventPublisher.Publish(new NoticeTeacherOfClassTodayClassTimesEvent(request.TenantId)
                        {
                            IsSendSms = tenantConfig.UserNoticeConfig.StartClassSms,
                            IsSendWeChat = tenantConfig.UserNoticeConfig.StartClassWeChat,
                            ClassTimesId = p.ClassTimesId,
                            WeChatNoticeRemark = tenantConfig.UserNoticeConfig.WeChatNoticeRemark
                        });
                    }
                    else if (diffMinute - beforeMinute <= 5)  //5分钟的容错时间
                    {
                        ids.Add(p.Id);
                        _eventPublisher.Publish(new NoticeTeacherOfClassTodayClassTimesEvent(request.TenantId)
                        {
                            IsSendSms = tenantConfig.UserNoticeConfig.StartClassSms,
                            IsSendWeChat = tenantConfig.UserNoticeConfig.StartClassWeChat,
                            ClassTimesId = p.ClassTimesId,
                            WeChatNoticeRemark = tenantConfig.UserNoticeConfig.WeChatNoticeRemark
                        });
                    }
                }
                if (ids.Count > 0)
                {
                    await _tempUserClassNoticeDAL.SetProcessed(ids, request.ClassOt);
                }
            }
            else
            {
                Log.Info($"[NoticeTeacherOfClassTodayTenantConsumerEvent]未查询到需要提醒的课次信息:TenantId:{request.TenantId},ClassOt:{request.ClassOt}", this.GetType());
            }
        }

        public async Task NoticeTeacherOfClassTodayClassTimesConsumerEvent(NoticeTeacherOfClassTodayClassTimesEvent request)
        {
            if (!request.IsSendSms && !request.IsSendWeChat)
            {
                Log.Info($"[NoticeTeacherOfClassTodayClassTimesConsumerEvent]未开启发送上课提醒服务:TenantId:{request.TenantId},ClassTimesId:{request.ClassTimesId}", this.GetType());
                return;
            }
            var classTimes = await _jobAnalyzeDAL.GetClassTimes(request.ClassTimesId);
            if (classTimes == null)
            {
                Log.Warn($"[NoticeTeacherOfClassTodayClassTimesConsumerEvent]课次未找到,无需发送上课通知:TenantId:{request.TenantId},ClassTimesId:{request.ClassTimesId}", this.GetType());
                return;
            }
            if (classTimes.Status == EmClassTimesStatus.BeRollcall)
            {
                Log.Warn($"[NoticeTeacherOfClassTodayClassTimesConsumerEvent]已点名,无需发送上课通知:TenantId:{request.TenantId},ClassTimesId:{request.ClassTimesId}", this.GetType());
                return;
            }
            if (classTimes.DataType != EmClassTimesDataType.Normal)
            {
                return;
            }

            var stringClassRoom = string.Empty;
            if (!string.IsNullOrEmpty(classTimes.ClassRoomIds))
            {
                var allClassRoom = await _classRoomDAL.GetAllClassRoom();
                stringClassRoom = ComBusiness.GetClassRoomDesc(allClassRoom, classTimes.ClassRoomIds);
            }

            var smsReq = new NoticeUserOfClassTodayRequest(await GetNoticeRequestBase(request.TenantId, request.IsSendWeChat))
            {
                ClassRoom = stringClassRoom,
                ClassTimeDesc = EtmsHelper.GetTimeDesc(classTimes.StartTime, classTimes.EndTime, "-"),
                StartTimeDesc = EtmsHelper.GetTimeDesc(classTimes.StartTime),
                Users = new List<NoticeUserOfClassTodayTeacher>()
            };
            var wxConfig = _appConfigurtaionServices.AppSettings.WxConfig;
            if (request.IsSendWeChat)
            {
                smsReq.TemplateIdShort = wxConfig.TemplateNoticeConfig.NoticeUserOfClass;
                smsReq.Remark = request.WeChatNoticeRemark;
            }
            var tempBoxCourse = new DataTempBox<EtCourse>();
            var courseName = await ComBusiness.GetCourseNames(tempBoxCourse, _courseDAL, classTimes.CourseList);
            var teachers = classTimes.Teachers.Split(',');
            foreach (var p in teachers)
            {
                if (string.IsNullOrEmpty(p))
                {
                    continue;
                }
                var userId = p.ToLong();
                var user = await _userDAL.GetUser(userId);
                if (user == null)
                {
                    continue;
                }
                smsReq.Users.Add(new NoticeUserOfClassTodayTeacher()
                {
                    CourseName = courseName,
                    Phone = user.Phone,
                    UserId = userId,
                    UserName = ComBusiness2.GetParentTeacherName(user),
                    OpendId = await GetOpenId(request.IsSendWeChat, userId)
                });
            }

            if (smsReq.Users.Any())
            {
                if (request.IsSendSms)
                {
                    smsReq.SmsTemplate = await _sysSmsTemplate2BLL.GetSmsTemplate(request.TenantId, EmSysSmsTemplateType.NoticeUserOfClassToday);
                    await _smsService.NoticeUserOfClassToday(smsReq);
                }
                if (request.IsSendWeChat)
                {
                    _wxService.NoticeUserOfClassToday(smsReq);
                }
            }
        }

        public async Task NoticeTeacherOfHomeworkFinishConsumerEvent(NoticeTeacherOfHomeworkFinishEvent request)
        {
            var tenantConfig = await _tenantConfigDAL.GetTenantConfig();
            if (!tenantConfig.UserNoticeConfig.StudentHomeworkSubmitWeChat)
            {
                return;
            }
            var homeworkDetail = await _activeHomeworkDetailDAL.GetActiveHomeworkDetailBucket(request.HomeworkDetailId);
            if (homeworkDetail == null || homeworkDetail.ActiveHomeworkDetail == null)
            {
                LOG.Log.Error($"[NoticeTeacherOfHomeworkFinishConsumerEvent]作业不存在,{JsonConvert.SerializeObject(request)}", this.GetType());
                return;
            }
            var activeHomeworkDetail = homeworkDetail.ActiveHomeworkDetail;
            var studentBucket = await _studentDAL.GetStudent(activeHomeworkDetail.StudentId);
            if (studentBucket == null || studentBucket.Student == null)
            {
                LOG.Log.Error($"[NoticeTeacherOfHomeworkFinishConsumerEvent]学员不存在,{JsonConvert.SerializeObject(request)}", this.GetType());
                return;
            }

            var homeworkClass = await _classDAL.GetClassBucket(activeHomeworkDetail.ClassId);
            var getUserIds = activeHomeworkDetail.CreateUserId.ToString();
            if (homeworkClass != null && homeworkClass.EtClass != null)
            {
                getUserIds = $"{getUserIds},{homeworkClass.EtClass.Teachers}";
            }
            var smsReq = new NoticeTeacherOfHomeworkFinishRequest(await GetNoticeRequestBase(request.TenantId))
            {
                StudentName = studentBucket.Student.Name,
                HomeworkTitle = activeHomeworkDetail.Title,
                FinishTime = activeHomeworkDetail.AnswerOt.EtmsToMinuteString(),
                Users = new List<NoticeTeacherOfHomeworkFinishItem>()
            };
            var wxConfig = _appConfigurtaionServices.AppSettings.WxConfig;
            smsReq.TemplateIdShort = wxConfig.TemplateNoticeConfig.NoticeUserOfHomeworkFinish;
            smsReq.Remark = tenantConfig.UserNoticeConfig.WeChatNoticeRemark;
            smsReq.Url = string.Format(wxConfig.TemplateNoticeConfig.TeacherHomeworkDetailUrl, activeHomeworkDetail.HomeworkId);

            var users = getUserIds.Split(',').Distinct();
            foreach (var p in users)
            {
                if (string.IsNullOrEmpty(p))
                {
                    continue;
                }
                var userId = p.ToLong();
                var user = await _userDAL.GetUser(userId);
                if (user == null)
                {
                    continue;
                }
                smsReq.Users.Add(new NoticeTeacherOfHomeworkFinishItem()
                {
                    Phone = user.Phone,
                    UserId = userId,
                    UserName = ComBusiness2.GetParentTeacherName(user),
                    OpendId = await GetOpenId(true, userId)
                });
            }
            if (smsReq.Users.Any())
            {
                _wxService.NoticeTeacherOfHomeworkFinish(smsReq);
            }
        }

        public async Task NoticeUserOfStudentTryClassFinishConsumerEvent(NoticeUserOfStudentTryClassFinishEvent request)
        {
            var studentBucket = await _studentDAL.GetStudent(request.StudentId);
            if (studentBucket.Student.TrackUser == null)
            {
                return;
            }
            var trackUser = await _userDAL.GetUser(studentBucket.Student.TrackUser.Value);
            if (trackUser == null)
            {
                LOG.Log.Fatal("[NoticeUserOfStudentTryClassFinishConsumerEvent]跟进人未找到", request, this.GetType());
                return;
            }
            var courseBucket = await _courseDAL.GetCourse(request.CourseId);
            if (courseBucket == null || courseBucket.Item1 == null)
            {
                LOG.Log.Fatal("[NoticeUserOfStudentTryClassFinishConsumerEvent]课程未找到", request, this.GetType());
                return;
            }

            var userOpenId = await GetOpenId(true, trackUser.Id);
            if (string.IsNullOrEmpty(userOpenId))
            {
                return;
            }

            var smsReq = new NoticeUserOfStudentTryClassFinishRequest(await GetNoticeRequestBase(request.TenantId))
            {
                StudentName = studentBucket.Student.Name,
                StudentPhone = studentBucket.Student.Phone,
                CourseName = courseBucket.Item1.Name,
                ClassOtDesc = $"{request.ClassRecord.ClassOt.EtmsToDateString()} {EtmsHelper.GetTimeDesc(request.ClassRecord.StartTime)}~{EtmsHelper.GetTimeDesc(request.ClassRecord.EndTime)}",
                Users = new List<NoticeUserOfStudentTryClassFinishUser>()
            };
            var wxConfig = _appConfigurtaionServices.AppSettings.WxConfig;
            var tenantConfig = await _tenantConfigDAL.GetTenantConfig();
            smsReq.TemplateIdShort = wxConfig.TemplateNoticeConfig.NoticeUserStudentTryClassFinish;
            smsReq.Remark = tenantConfig.UserNoticeConfig.WeChatNoticeRemark;

            smsReq.Users.Add(new NoticeUserOfStudentTryClassFinishUser()
            {
                Phone = trackUser.Phone,
                UserId = trackUser.Id,
                UserName = ComBusiness2.GetParentTeacherName(trackUser),
                OpendId = userOpenId
            });

            _wxService.NoticeUserOfStudentTryClassFinish(smsReq);
        }

        public async Task NoticeTeacherStudentReservation(NoticeStudentReservationEvent request, NoticeStudentOrUserReservationRequest req, EtClassTimes classTimes,
            EtStudent student)
        {
            req.StudentOrUsers = new List<NoticeStudentReservationStudent>();
            if (request.OpType == NoticeStudentReservationOpType.Success)
            {
                req.Title = "老师您好,有学生预约了您的课程!";
            }
            else
            {
                req.Title = "学生预约的您的课程已取消，请及时查看";
            }
            var teachers = classTimes.Teachers.Split(',');
            foreach (var p in teachers)
            {
                if (string.IsNullOrEmpty(p))
                {
                    continue;
                }
                var userId = p.ToLong();
                var user = await _userDAL.GetUser(userId);
                if (user == null)
                {
                    continue;
                }
                req.StudentOrUsers.Add(new NoticeStudentReservationStudent()
                {
                    Name = student.Name,
                    OpendId = await GetOpenId(true, userId),
                    Phone = student.Phone,
                    StudentId = student.Id
                });
            }

            if (req.StudentOrUsers.Count > 0)
            {
                _wxService.NoticeStudentOrUserReservation(req);
            }
        }

        public async Task NoticeUserStudentLeaveApplyConsumerEvent(NoticeUserStudentLeaveApplyEvent request)
        {
            var studentLeaveApplyLog = request.StudentLeaveApplyLog;
            var studentBucket = await _studentDAL.GetStudent(studentLeaveApplyLog.StudentId);
            if (studentBucket == null || studentBucket.Student == null)
            {
                Log.Error("[NoticeUserStudentLeaveApplyConsumerEvent]未找到学员", request, this.GetType());
                return;
            }
            var tenantConfig = await _tenantConfigDAL.GetTenantConfig();
            if (!tenantConfig.UserNoticeConfig.StudentLeaveApplyWeChat)
            {
                return;
            }

            var noticeUser = await ComBusiness5.GetNoticeUser(_classDAL, _userDAL, studentBucket.Student,
                RoleOtherSetting.StudentLeaveApplyMy, RoleOtherSetting.StudentLeaveApply);
            if (!noticeUser.Any())
            {
                return;
            }

            var smsReq = new NoticeUserMessageRequest(await GetNoticeRequestBase(request.TenantId))
            {
                Users = new List<NoticeUserMessageUser>()
            };
            var wxConfig = _appConfigurtaionServices.AppSettings.WxConfig;
            smsReq.TemplateIdShort = wxConfig.TemplateNoticeConfig.UserMessage;
            smsReq.Remark = tenantConfig.UserNoticeConfig.WeChatNoticeRemark;

            if (studentLeaveApplyLog.HandleStatus == EmStudentLeaveApplyHandleStatus.IsRevoke)
            {
                smsReq.Title = $"学员[{studentBucket.Student.Name}]请假撤销通知";
            }
            else
            {
                smsReq.Title = $"学员[{studentBucket.Student.Name}]请假通知";
            }
            if (studentLeaveApplyLog.StartDate == studentLeaveApplyLog.EndDate)
            {
                smsReq.OtDesc = $"{studentLeaveApplyLog.StartDate.EtmsToDateString()} ({EtmsHelper.GetTimeDesc(studentLeaveApplyLog.StartTime)}~{EtmsHelper.GetTimeDesc(studentLeaveApplyLog.EndTime)})";
            }
            else
            {
                smsReq.OtDesc = $"{studentLeaveApplyLog.StartDate.EtmsToDateString()}({EtmsHelper.GetTimeDesc(studentLeaveApplyLog.StartTime)})~{studentLeaveApplyLog.EndDate.EtmsToDateString()}({EtmsHelper.GetTimeDesc(studentLeaveApplyLog.EndTime)})";
            }

            smsReq.Content = $"请假事由_{studentLeaveApplyLog.LeaveContent}";

            var url = string.Format(wxConfig.TemplateNoticeConfig.UserStudentLeaveApplyUrl, studentLeaveApplyLog.Id);
            foreach (var user in noticeUser)
            {
                smsReq.Users.Add(new NoticeUserMessageUser()
                {
                    Phone = user.Phone,
                    UserId = user.Id,
                    UserName = ComBusiness2.GetParentTeacherName(user),
                    OpendId = await GetOpenId(true, user.Id),
                    Url = url
                });
            }

            if (smsReq.Users.Any())
            {
                _wxService.NoticeUserMessage(smsReq);
            }
        }

        public async Task NoticeUserContractsNotArrivedConsumerEvent(NoticeUserContractsNotArrivedEvent request)
        {
            if (request.ClassRecordNotArrivedStudents == null || request.ClassRecordNotArrivedStudents.Count == 0)
            {
                return;
            }
            var classBucket = await _classDAL.GetClassBucket(request.ClassRecord.ClassId);
            if (classBucket == null || classBucket.EtClass == null)
            {
                return;
            }
            var myClass = classBucket.EtClass;
            var relationUserIds = EtmsHelper.AnalyzeMuIds(myClass.Teachers);
            var studentNames = new StringBuilder();
            foreach (var p in request.ClassRecordNotArrivedStudents)
            {
                var studentBucket = await _studentDAL.GetStudent(p.StudentId);
                if (studentBucket == null || studentBucket.Student == null)
                {
                    continue;
                }
                studentNames.Append($"{studentBucket.Student.Name},");
                if (studentBucket.Student.LearningManager != null)
                {
                    relationUserIds.Add(studentBucket.Student.LearningManager.Value);
                }
            }

            var noticeUser = new List<NoticeUserView>();
            if (relationUserIds.Any())
            {
                var trelationUsers = await _userDAL.GetUserAboutNotice(RoleOtherSetting.StudentContractsNotArrivedMy, relationUserIds);
                if (trelationUsers != null && trelationUsers.Any())
                {
                    noticeUser.AddRange(trelationUsers);
                }
            }
            var noticeAllUsers = await _userDAL.GetUserAboutNotice(RoleOtherSetting.StudentContractsNotArrived);
            if (noticeAllUsers.Any())
            {
                noticeUser.AddRange(noticeAllUsers);
            }
            if (!noticeUser.Any())
            {
                return;
            }
            noticeUser = noticeUser.Distinct(new ComparerNoticeUserView()).ToList();

            var classRecord = request.ClassRecord;
            var smsReq = new NoticeUserMessageRequest(await GetNoticeRequestBase(request.TenantId))
            {
                Users = new List<NoticeUserMessageUser>(),
                Title = "上课点名未到学员通知",
                OtDesc = classRecord.CheckOt.EtmsToMinuteString()
            };
            var tenantConfig = await _tenantConfigDAL.GetTenantConfig();
            var wxConfig = _appConfigurtaionServices.AppSettings.WxConfig;
            smsReq.TemplateIdShort = wxConfig.TemplateNoticeConfig.UserMessage;
            smsReq.Remark = tenantConfig.UserNoticeConfig.WeChatNoticeRemark;
            smsReq.Content = $"班级[{myClass.Name}]在{classRecord.ClassOt.EtmsToDateString()}(周{EtmsHelper.GetWeekDesc(classRecord.Week)}) {EtmsHelper.GetTimeDesc(classRecord.StartTime)}~{EtmsHelper.GetTimeDesc(classRecord.EndTime)}上课时，学员[{studentNames.ToString().TrimEnd(',')}]未到";

            var url = string.Format(wxConfig.TemplateNoticeConfig.UserClassRecordDetailUrl, classRecord.Id);
            foreach (var user in noticeUser)
            {
                smsReq.Users.Add(new NoticeUserMessageUser()
                {
                    Phone = user.Phone,
                    UserId = user.Id,
                    UserName = ComBusiness2.GetParentTeacherName(user),
                    OpendId = await GetOpenId(true, user.Id),
                    Url = url
                });
            }

            if (smsReq.Users.Any())
            {
                _wxService.NoticeUserMessage(smsReq);
            }
        }

        public async Task NoticeUserTryCalssApplyConsumerEvent(NoticeUserTryCalssApplyEvent request)
        {
            var noticeUser = await _userDAL.GetUserAboutNotice(RoleOtherSetting.TryCalssApply);
            if (!noticeUser.Any())
            {
                return;
            }

            var tryCalssApplyLog = request.TryCalssApplyLog;
            var smsReq = new NoticeUserMessageRequest(await GetNoticeRequestBase(request.TenantId))
            {
                Users = new List<NoticeUserMessageUser>(),
                Title = "试听申请通知",
                OtDesc = tryCalssApplyLog.ApplyOt.EtmsToMinuteString()
            };
            var tenantConfig = await _tenantConfigDAL.GetTenantConfig();
            var wxConfig = _appConfigurtaionServices.AppSettings.WxConfig;
            smsReq.TemplateIdShort = wxConfig.TemplateNoticeConfig.UserMessage;
            smsReq.Remark = tenantConfig.UserNoticeConfig.WeChatNoticeRemark;

            var phone = tryCalssApplyLog.Phone;
            if (tryCalssApplyLog.StudentId != null)
            {
                var studentBucket = await _studentDAL.GetStudent(tryCalssApplyLog.StudentId.Value);
                if (studentBucket != null && studentBucket.Student != null)
                {
                    phone = studentBucket.Student.Phone;
                }
            }
            smsReq.Content = $"手机号[{phone}]申请试听课程";

            var url = string.Format(wxConfig.TemplateNoticeConfig.UserTryCalssApplyDetailUrl, tryCalssApplyLog.Id);
            foreach (var user in noticeUser)
            {
                smsReq.Users.Add(new NoticeUserMessageUser()
                {
                    Phone = user.Phone,
                    UserId = user.Id,
                    UserName = ComBusiness2.GetParentTeacherName(user),
                    OpendId = await GetOpenId(true, user.Id),
                    Url = url
                });
            }

            if (smsReq.Users.Any())
            {
                _wxService.NoticeUserMessage(smsReq);
            }
        }

        public async Task EveryDayStatisticsConsumerEvent(EveryDayStatisticsEvent request)
        {
            var noticeUser = await _userDAL.GetUserAboutNotice(RoleOtherSetting.EverydayBusinessStatistics);
            if (!noticeUser.Any())
            {
                return;
            }
            var myDate = request.Time.Date;
            var minDate = myDate;
            var endDate = myDate.AddDays(1).Date;
            var minDateDesc = myDate.EtmsToDateString();
            var endDateDesc = endDate.EtmsToDateString();
            var potentialAddCount = await _jobAnalyze2DAL.GetStudentPotentialAddCount(minDateDesc, endDateDesc);
            var readingAddCount = await _jobAnalyze2DAL.GetStudentReadingAddCount(minDateDesc, endDateDesc);
            var orderAddCount = await _jobAnalyze2DAL.GetOrderAddCount(minDateDesc, endDateDesc);
            var result = new EveryDayStatisticsView()
            {
                StudentPotentialAddCount = potentialAddCount,
                StudentReadingAddCount = readingAddCount,
                OrderAddCount = orderAddCount
            };
            var deClassTimes = await _jobAnalyze2DAL.GetDeClassTimes(minDateDesc, endDateDesc);
            if (deClassTimes != null)
            {
                result.DeClassTimes = deClassTimes.TotalClassTimes.EtmsToString();
                result.DeClassTimesSum = deClassTimes.TotalDeSum;
            }
            var incomeTypeView = await _jobAnalyze2DAL.GetIncomeType(minDateDesc, endDateDesc);
            if (incomeTypeView != null && incomeTypeView.Any())
            {
                var inLog = incomeTypeView.FirstOrDefault(j => j.Type == EmIncomeLogType.AccountIn);
                if (inLog != null)
                {
                    result.IncomeIn = inLog.TotalSum;
                }
                var outLog = incomeTypeView.FirstOrDefault(j => j.Type == EmIncomeLogType.AccountOut);
                if (outLog != null)
                {
                    result.IncomeOut = outLog.TotalSum;
                }
            }
            var studentCheckStatusView = await _jobAnalyze2DAL.GetStudentCheckStatusView(minDateDesc);
            if (studentCheckStatusView != null && studentCheckStatusView.Any())
            {
                var myClassArrivedCountLog = studentCheckStatusView.FirstOrDefault(j => j.StudentCheckStatus == EmClassStudentCheckStatus.Arrived);
                if (myClassArrivedCountLog != null)
                {
                    result.ClassArrivedCount = myClassArrivedCountLog.Count;
                }
                var myClassBeLateCount = studentCheckStatusView.FirstOrDefault(j => j.StudentCheckStatus == EmClassStudentCheckStatus.BeLate);
                if (myClassBeLateCount != null)
                {
                    result.ClassBeLateCount = myClassBeLateCount.Count;
                }
                var myClassLeaveCount = studentCheckStatusView.FirstOrDefault(j => j.StudentCheckStatus == EmClassStudentCheckStatus.Leave);
                if (myClassLeaveCount != null)
                {
                    result.ClassLeaveCount = myClassLeaveCount.Count;
                }
                var myClassNotArrivedCount = studentCheckStatusView.FirstOrDefault(j => j.StudentCheckStatus == EmClassStudentCheckStatus.NotArrived);
                if (myClassNotArrivedCount != null)
                {
                    result.ClassNotArrivedCount = myClassNotArrivedCount.Count;
                }
            }

            var myCheckInCount = await _studentCheckOnLogDAL.GetStatisticsCheckInCount(myDate) ;
            var myCheckOutCount = await _studentCheckOnLogDAL.GetStatisticsCheckOutCount(myDate);
            var myCheckAttendClassCount = await _studentCheckOnLogDAL.GetStatisticsCheckAttendClassCount(myDate);
            
            var smsReq = new NoticeUserMessageRequest(await GetNoticeRequestBase(request.TenantId))
            {
                Users = new List<NoticeUserMessageUser>(),
                Title = "机构经营数据通知",
                OtDesc = myDate.EtmsToDateString()
            };
            var tenantConfig = await _tenantConfigDAL.GetTenantConfig();
            var wxConfig = _appConfigurtaionServices.AppSettings.WxConfig;
            smsReq.TemplateIdShort = wxConfig.TemplateNoticeConfig.UserMessage;
            smsReq.Remark = tenantConfig.UserNoticeConfig.WeChatNoticeRemark;

            var str = new StringBuilder();
            str.Append($"昨日新增意向学员{result.StudentPotentialAddCount}人;\n");
            str.Append($"昨日新增正式学员{result.StudentReadingAddCount}人;\n");
            str.Append($"昨日报课人数{result.OrderAddCount}人;\n");
            str.Append($"昨日收入{result.IncomeIn}元;\n");
            str.Append($"昨日支出{result.IncomeOut}元;\n");
            str.Append($"昨日考勤签到人数{myCheckInCount};\n");
            str.Append($"昨日考勤签退人数{myCheckOutCount};\n");
            str.Append($"昨日考勤记上课人数{myCheckAttendClassCount};\n");
            str.Append($"昨日消课课时{result.DeClassTimes};\n");
            str.Append($"昨日消课金额{result.DeClassTimesSum}元;\n");
            str.Append($"昨日点名到课人数{result.ClassArrivedCount}人;\n");
            str.Append($"昨日点名迟到人数{result.ClassBeLateCount}人;\n");
            str.Append($"昨日点名请假人数{result.ClassLeaveCount}人;\n");
            str.Append($"昨日点名未到人数{result.ClassNotArrivedCount}人;");
            smsReq.Content = str.ToString();

            foreach (var user in noticeUser)
            {
                smsReq.Users.Add(new NoticeUserMessageUser()
                {
                    Phone = user.Phone,
                    UserId = user.Id,
                    UserName = ComBusiness2.GetParentTeacherName(user),
                    OpendId = await GetOpenId(true, user.Id),
                });
            }

            if (smsReq.Users.Any())
            {
                _wxService.NoticeUserMessage(smsReq);
            }
        }
    }
}
