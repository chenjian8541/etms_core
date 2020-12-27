using ETMS.Business.Common;
using ETMS.Entity.Config;
using ETMS.Entity.Database.Source;
using ETMS.Entity.Enum;
using ETMS.Entity.ExternalService.Dto.Request.User;
using ETMS.Event.DataContract;
using ETMS.ExternalService.Contract;
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

        public UserSendNoticeBLL(IEventPublisher eventPublisher, ITenantConfigDAL tenantConfigDAL, ICourseDAL courseDAL, IClassDAL classDAL,
            IJobAnalyzeDAL jobAnalyzeDAL, ITempUserClassNoticeDAL tempUserClassNoticeDAL, IClassRoomDAL classRoomDAL,
            IUserWechatDAL userWechatDAL, IComponentAccessBLL componentAccessBLL, ISysTenantDAL sysTenantDAL, IAppConfigurtaionServices appConfigurtaionServices,
            IUserDAL userDAL, ISmsService smsService, IWxService wxService, IActiveHomeworkDetailDAL activeHomeworkDetailDAL,
            IStudentDAL studentDAL)
            : base(userWechatDAL, componentAccessBLL, sysTenantDAL)
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
        }

        public void InitTenantId(int tenantId)
        {
            this.InitDataAccess(tenantId, _tenantConfigDAL, _courseDAL, _classDAL, _jobAnalyzeDAL, _tempUserClassNoticeDAL, _classRoomDAL,
                _userWechatDAL, _userDAL, _activeHomeworkDetailDAL, _studentDAL);
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
            if (classTimes.Status == EmClassTimesStatus.BeRollcall)
            {
                Log.Warn($"[NoticeTeacherOfClassTodayClassTimesConsumerEvent]已点名,无需发送上课通知:TenantId:{request.TenantId},ClassTimesId:{request.ClassTimesId}", this.GetType());
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
    }
}
