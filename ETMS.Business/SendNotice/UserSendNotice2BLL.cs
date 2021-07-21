using ETMS.Business.Common;
using ETMS.Entity.Config;
using ETMS.Entity.Database.Source;
using ETMS.Entity.Enum;
using ETMS.Entity.ExternalService.Dto.Request.User;
using ETMS.Entity.Temp.Compare;
using ETMS.Event.DataContract;
using ETMS.ExternalService.Contract;
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
    public class UserSendNotice2BLL : SendUserNoticeBase, IUserSendNotice2BLL
    {
        private readonly IEventPublisher _eventPublisher;

        private readonly IAppConfigurtaionServices _appConfigurtaionServices;

        private readonly ITenantConfigDAL _tenantConfigDAL;

        private readonly IActiveGrowthRecordDAL _activeGrowthRecordDAL;

        private readonly IUserDAL _userDAL;

        private readonly IWxService _wxService;

        private readonly IClassDAL _classDAL;

        private readonly IStudentCheckOnLogDAL _studentCheckOnLogDAL;

        private readonly IClassTimesDAL _classTimesDAL;

        private readonly IStudentDAL _studentDAL;

        public UserSendNotice2BLL(IEventPublisher eventPublisher, ITenantConfigDAL tenantConfigDAL, IComponentAccessBLL componentAccessBLL,
               IUserWechatDAL userWechatDAL, ISysTenantDAL sysTenantDAL, IAppConfigurtaionServices appConfigurtaionServices,
               IActiveGrowthRecordDAL activeGrowthRecordDAL, IUserDAL userDAL, IWxService wxService, IClassDAL classDAL, ITenantLibBLL tenantLibBLL,
               IStudentCheckOnLogDAL studentCheckOnLogDAL, IClassTimesDAL classTimesDAL, IStudentDAL studentDAL)
             : base(userWechatDAL, componentAccessBLL, sysTenantDAL, tenantLibBLL)
        {
            this._eventPublisher = eventPublisher;
            this._appConfigurtaionServices = appConfigurtaionServices;
            this._tenantConfigDAL = tenantConfigDAL;
            this._activeGrowthRecordDAL = activeGrowthRecordDAL;
            this._userDAL = userDAL;
            this._wxService = wxService;
            this._classDAL = classDAL;
            this._studentCheckOnLogDAL = studentCheckOnLogDAL;
            this._classTimesDAL = classTimesDAL;
            this._studentDAL = studentDAL;
        }

        public void InitTenantId(int tenantId)
        {
            this._tenantLibBLL.InitTenantId(tenantId);
            this.InitDataAccess(tenantId, _tenantConfigDAL, _userWechatDAL, _activeGrowthRecordDAL, _userDAL, _classDAL,
                _studentCheckOnLogDAL, _classTimesDAL, _studentDAL);
        }

        public async Task NoticeUserActiveGrowthCommentConsumerEvent(NoticeUserActiveGrowthCommentEvent request)
        {
            var comment = request.ActiveGrowthRecordDetailComment;
            var activeGrowthRecordBucket = await _activeGrowthRecordDAL.GetActiveGrowthRecord(comment.GrowthRecordId);
            if (activeGrowthRecordBucket == null || activeGrowthRecordBucket.ActiveGrowthRecord == null)
            {
                return;
            }
            var activeGrowthRecord = activeGrowthRecordBucket.ActiveGrowthRecord;
            if (activeGrowthRecord.SendType == EmActiveGrowthRecordSendType.No)
            {
                return;
            }

            var myNoticeUsers = new List<EtUser>();
            if (activeGrowthRecord.Type == EmActiveGrowthRecordType.Class)   //通知班级老师
            {
                var classIds = EtmsHelper.AnalyzeMuIds(activeGrowthRecord.RelatedIds);
                if (classIds.Any())
                {
                    foreach (var classId in classIds)
                    {
                        var classBucket = await _classDAL.GetClassBucket(classId);
                        if (classBucket == null || classBucket.EtClass == null)
                        {
                            continue;
                        }
                        var teachers = EtmsHelper.AnalyzeMuIds(classBucket.EtClass.Teachers);
                        if (teachers.Any())
                        {
                            foreach (var p in teachers)
                            {
                                var user = await _userDAL.GetUser(p);
                                if (user != null)
                                {
                                    myNoticeUsers.Add(user);
                                }
                            }
                        }
                    }
                }
            }

            var noticeUser = await _userDAL.GetUserAboutNotice(RoleOtherSetting.ReceiveInteractiveStudent);
            if (noticeUser == null || noticeUser.Count == 0)
            {
                if (myNoticeUsers.Count == 0)
                {
                    return;
                }
            }
            else
            {
                myNoticeUsers.AddRange(noticeUser);
            }
            myNoticeUsers = myNoticeUsers.Distinct(new ComparerEtUser()).ToList();

            var smsReq = new NoticeUserMessageRequest(await GetNoticeRequestBase(request.TenantId))
            {
                Users = new List<NoticeUserMessageUser>(),
                Title = "学员评论成长档案提醒",
                OtDesc = comment.Ot.EtmsToMinuteString(),
                Content = "点击查看详情"
            };
            var tenantConfig = await _tenantConfigDAL.GetTenantConfig();
            var wxConfig = _appConfigurtaionServices.AppSettings.WxConfig;
            smsReq.TemplateIdShort = wxConfig.TemplateNoticeConfig.UserMessage;
            smsReq.Remark = tenantConfig.UserNoticeConfig.WeChatNoticeRemark;

            var url = string.Format(wxConfig.TemplateNoticeConfig.TeacherActiveGrowthRecordDetailUrl, activeGrowthRecord.Id);
            foreach (var user in myNoticeUsers)
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

        public async Task NoticeUserAboutStudentCheckOnConsumerEvent(NoticeUserAboutStudentCheckOnEvent request)
        {
            var log = await _studentCheckOnLogDAL.GetStudentCheckOnLog(request.StudentCheckOnLogId);
            if (log == null)
            {
                Log.Error($"[NoticeUserAboutStudentCheckOnConsumerEvent]未找到考勤记录:{JsonConvert.SerializeObject(request)}", this.GetType());
                return;
            }

            var studentBucket = await _studentDAL.GetStudent(log.StudentId);
            if (studentBucket == null || studentBucket.Student == null)
            {
                Log.Error($"[NoticeUserAboutStudentCheckOnConsumerEvent]未找到考勤的学员:{JsonConvert.SerializeObject(request)}", this.GetType());
                return;
            }

            var tenantConfig = await _tenantConfigDAL.GetTenantConfig();
            var myNoticeUsers = new List<EtUser>();
            if (tenantConfig.UserNoticeConfig.StudentCheckOnWeChat)
            {
                if (log.ClassTimesId != null)
                {
                    var myClassTimes = await _classTimesDAL.GetClassTimes(log.ClassTimesId.Value);
                    if (myClassTimes != null)
                    {
                        var myTeachers = string.Empty;
                        if (!string.IsNullOrEmpty(myClassTimes.Teachers))
                        {
                            myTeachers = myClassTimes.Teachers;
                        }
                        else
                        {
                            var myClass = await _classDAL.GetClassBucket(myClassTimes.ClassId);
                            myTeachers = myClass.EtClass.Teachers;
                        }
                        var teacherIds = EtmsHelper.AnalyzeMuIds(myTeachers);
                        if (teacherIds.Any())
                        {
                            foreach (var p in teacherIds)
                            {
                                var user = await _userDAL.GetUser(p);
                                if (user != null)
                                {
                                    myNoticeUsers.Add(user);
                                }
                            }
                        }
                    }
                }
            }

            var noticeUser = await _userDAL.GetUserAboutNotice(RoleOtherSetting.StudentCheckOnWeChat);
            if (noticeUser == null || noticeUser.Count == 0)
            {
                if (myNoticeUsers.Count == 0)
                {
                    return;
                }
            }
            else
            {
                myNoticeUsers.AddRange(noticeUser);
            }

            myNoticeUsers = myNoticeUsers.Distinct(new ComparerEtUser()).ToList();

            var title = "学员考勤提醒—签到成功";
            if (log.CheckType == EmStudentCheckOnLogCheckType.CheckOut)
            {
                title = "学员考勤提醒—签退成功";
            }
            var smsReq = new NoticeUserMessageRequest(await GetNoticeRequestBase(request.TenantId))
            {
                Users = new List<NoticeUserMessageUser>(),
                Title = title,
                OtDesc = log.CheckOt.EtmsToMinuteString(),
                Content = studentBucket.Student.Name
            };
            var wxConfig = _appConfigurtaionServices.AppSettings.WxConfig;
            smsReq.TemplateIdShort = wxConfig.TemplateNoticeConfig.UserMessage;
            smsReq.Remark = tenantConfig.UserNoticeConfig.WeChatNoticeRemark;

            var url = string.Empty;
            if (log.CheckForm == EmStudentCheckOnLogCheckForm.Face)
            {
                url = string.Format(wxConfig.TemplateNoticeConfig.TeacherStudentCheckLogUrl, log.Id);
            }
            foreach (var user in myNoticeUsers)
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
    }
}
