using ETMS.Entity.Config;
using ETMS.Entity.Enum;
using ETMS.Entity.ExternalService.Dto.Request;
using ETMS.Event.DataContract;
using ETMS.ExternalService.Contract;
using ETMS.IBusiness.SendNotice;
using ETMS.IBusiness.Wechart;
using ETMS.IDataAccess;
using ETMS.IDataAccess.EtmsManage;
using ETMS.LOG;
using ETMS.Utility;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using ETMS.Business.Common;
using ETMS.Entity.Database.Source;
using ETMS.IBusiness;
using ETMS.Entity.Temp.Request;

namespace ETMS.Business.SendNotice
{
    public class StudentSendNotice3BLL : SendStudentNoticeBase, IStudentSendNotice3BLL
    {
        private readonly IWxService _wxService;

        private readonly IAppConfigurtaionServices _appConfigurtaionServices;

        private readonly ISmsService _smsService;

        private readonly ITenantConfigDAL _tenantConfigDAL;

        private readonly IStudentDAL _studentDAL;

        private readonly ICouponsDAL _couponsDAL;

        private readonly IParentStudentDAL _parentStudentDAL;

        private readonly IClassTimesDAL _classTimesDAL;

        private readonly IClassDAL _classDAL;

        private readonly ICourseDAL _courseDAL;

        private readonly IStudentAccountRechargeCoreBLL _studentAccountRechargeCoreBLL;

        private readonly IUserSendNoticeBLL _userSendNoticeBLL;

        private readonly IActiveGrowthRecordDAL _activeGrowthRecordDAL;

        public StudentSendNotice3BLL(IStudentWechatDAL studentWechatDAL, IComponentAccessBLL componentAccessBLL, ISysTenantDAL sysTenantDAL, IWxService wxService, IAppConfigurtaionServices appConfigurtaionServices, ISmsService smsService,
            ITenantConfigDAL tenantConfigDAL, IStudentDAL studentDAL, ICouponsDAL couponsDAL, IParentStudentDAL parentStudentDAL,
            IClassTimesDAL classTimesDAL, IClassDAL classDAL, ICourseDAL courseDAL, IStudentAccountRechargeCoreBLL studentAccountRechargeCoreBLL,
            IUserSendNoticeBLL userSendNoticeBLL, IActiveGrowthRecordDAL activeGrowthRecordDAL)
            : base(studentWechatDAL, componentAccessBLL, sysTenantDAL)
        {
            this._wxService = wxService;
            this._appConfigurtaionServices = appConfigurtaionServices;
            this._smsService = smsService;
            this._tenantConfigDAL = tenantConfigDAL;
            this._studentDAL = studentDAL;
            this._couponsDAL = couponsDAL;
            this._parentStudentDAL = parentStudentDAL;
            this._classTimesDAL = classTimesDAL;
            this._classDAL = classDAL;
            this._courseDAL = courseDAL;
            this._studentAccountRechargeCoreBLL = studentAccountRechargeCoreBLL;
            this._userSendNoticeBLL = userSendNoticeBLL;
            this._activeGrowthRecordDAL = activeGrowthRecordDAL;
        }

        public void InitTenantId(int tenantId)
        {
            this._studentAccountRechargeCoreBLL.InitTenantId(tenantId);
            this._userSendNoticeBLL.InitTenantId(tenantId);
            this.InitDataAccess(tenantId, _studentWechatDAL, _tenantConfigDAL, _studentDAL, _couponsDAL,
                _parentStudentDAL, _classTimesDAL, _classDAL, _courseDAL, _activeGrowthRecordDAL);
        }

        public async Task NoticeStudentCouponsGetConsumerEvent(NoticeStudentCouponsGetEvent request)
        {
            var tenantConfig = await _tenantConfigDAL.GetTenantConfig();
            if (!tenantConfig.StudentNoticeConfig.StudentCouponsWeChat)
            {
                return;
            }
            var couponsStudentGetList = await _couponsDAL.GetCouponsStudentGet(request.GenerateNo);
            if (couponsStudentGetList == null || couponsStudentGetList.Count == 0)
            {
                Log.Info($"[NoticeStudentCouponsGetConsumerEvent]未找到优惠券发送信息,GenerateNo:{request.GenerateNo}", this.GetType());
                return;
            }
            var myCoupons = await _couponsDAL.GetCoupons(couponsStudentGetList[0].CouponsId);
            if (myCoupons == null)
            {
                Log.Error($"[NoticeStudentCouponsGetConsumerEvent]未找到优惠券信息,CouponsId:{couponsStudentGetList[0].CouponsId}", this.GetType());
                return;
            }
            var req = new NoticeStudentCouponsGetRequest(await GetNoticeRequestBase(request.TenantId))
            {
                OtDesc = couponsStudentGetList[0].GetTime.EtmsToMinuteString(),
                Title = $"恭喜您获得一张[{EmCouponsType.GetCouponsTypeDesc(myCoupons.Type)}]，劵已经发至您账户下，点击查看详情！",
                CouponsConent = myCoupons.Title,
                Students = new List<NoticeStudentCouponsGetStudent>()
            };
            var wxConfig = _appConfigurtaionServices.AppSettings.WxConfig;
            req.TemplateIdShort = wxConfig.TemplateNoticeConfig.WxMessage;
            req.Url = string.Empty;
            req.Remark = tenantConfig.StudentNoticeConfig.WeChatNoticeRemark;
            var url = wxConfig.TemplateNoticeConfig.CouponsUrl;
            foreach (var p in couponsStudentGetList)
            {
                var studentBucket = await _studentDAL.GetStudent(p.StudentId);
                if (studentBucket == null || studentBucket.Student == null)
                {
                    Log.Warn($"[NoticeStudentCouponsGetConsumerEvent]未找到学员信息,StudentId:{p.StudentId}", this.GetType());
                    continue;
                }
                var student = studentBucket.Student;
                if (string.IsNullOrEmpty(student.Phone))
                {
                    continue;
                }
                req.Students.Add(new NoticeStudentCouponsGetStudent()
                {
                    Name = student.Name,
                    OpendId = await GetOpenId(true, student.Phone),
                    Phone = student.Phone,
                    StudentId = student.Id,
                    Url = url
                });
                if (!string.IsNullOrEmpty(student.PhoneBak) && EtmsHelper.IsMobilePhone(student.PhoneBak))
                {
                    req.Students.Add(new NoticeStudentCouponsGetStudent()
                    {
                        Name = student.Name,
                        OpendId = await GetOpenId(true, student.PhoneBak),
                        Phone = student.PhoneBak,
                        StudentId = student.Id,
                        Url = url
                    });
                }
            }
            if (req.Students.Count > 0)
            {
                _wxService.NoticeStudentCouponsGet(req);
            }
        }

        public async Task NoticeStudentCouponsExplainConsumetEvent(NoticeStudentCouponsExplainEvent request)
        {
            var tenantConfig = await _tenantConfigDAL.GetTenantConfig();
            if (!tenantConfig.StudentNoticeConfig.StudentCouponsWeChat)
            {
                return;
            }
            var myToExpire = await _couponsDAL.GetCouponsStudentGetToExpire(DateTime.Now, DateTime.Now.AddDays(3));
            if (myToExpire == null || myToExpire.Count() == 0)
            {
                Log.Info($"[NoticeStudentCouponsExplainConsumetEvent]未找到需要提醒的将过期的优惠券:tenantId:{request.TenantId}", this.GetType());
                return;
            }
            var req = new NoticeStudentCouponsExplainRequest(await GetNoticeRequestBase(request.TenantId))
            {
                Students = new List<NoticeStudentCouponsExplainItem>()
            };
            var wxConfig = _appConfigurtaionServices.AppSettings.WxConfig;
            req.TemplateIdShort = wxConfig.TemplateNoticeConfig.WxMessage;
            req.Url = string.Empty;
            req.Remark = tenantConfig.StudentNoticeConfig.WeChatNoticeRemark;

            var tempBoxCoupons = new DataTempBox<EtCoupons>();
            var tempBoxStudent = new DataTempBox<EtStudent>();
            var remindStudents = new List<long>();
            var url = wxConfig.TemplateNoticeConfig.CouponsUrl;
            var title = "您有一张优惠券即将到期！请尽快使用~ ";
            foreach (var p in myToExpire)
            {
                var isRemindStudent = remindStudents.FirstOrDefault(j => j == p.StudentId);
                if (isRemindStudent > 0)
                {
                    continue;
                }
                var student = await ComBusiness.GetStudent(tempBoxStudent, _studentDAL, p.StudentId);
                if (student == null)
                {
                    remindStudents.Add(p.StudentId);
                    Log.Warn($"[NoticeStudentCouponsExplainConsumetEvent]未找到学员信息,StudentId:{p.StudentId}", this.GetType());
                    continue;
                }
                if (string.IsNullOrEmpty(student.Phone))
                {
                    remindStudents.Add(p.StudentId);
                    continue;
                }
                var myCoupons = await ComBusiness.GetCoupons(tempBoxCoupons, _couponsDAL, p.CouponsId);
                if (myCoupons == null)
                {
                    Log.Warn($"[NoticeStudentCouponsExplainConsumetEvent]优惠券不存在,CouponsId:{p.CouponsId}", this.GetType());
                    continue;
                }
                var ot = p.ExpiredTime.EtmsToDateString();
                req.Students.Add(new NoticeStudentCouponsExplainItem()
                {
                    Name = student.Name,
                    OpendId = await GetOpenId(true, student.Phone),
                    Phone = student.Phone,
                    StudentId = student.Id,
                    Url = url,
                    Title = title,
                    OtDesc = ot,
                    CouponsConent = myCoupons.Title
                });
                if (!string.IsNullOrEmpty(student.PhoneBak) && EtmsHelper.IsMobilePhone(student.PhoneBak))
                {
                    req.Students.Add(new NoticeStudentCouponsExplainItem()
                    {
                        Name = student.Name,
                        OpendId = await GetOpenId(true, student.PhoneBak),
                        Phone = student.PhoneBak,
                        StudentId = student.Id,
                        Url = url,
                        Title = title,
                        OtDesc = ot,
                        CouponsConent = myCoupons.Title
                    });
                }
                remindStudents.Add(p.StudentId);
            }
            if (req.Students.Count > 0)
            {
                _wxService.NoticeStudentCouponsExplain(req);
            }
        }

        public async Task NoticeStudentAccountRechargeChangedConsumerEvent(NoticeStudentAccountRechargeChangedEvent request)
        {
            var tenantConfig = await _tenantConfigDAL.GetTenantConfig();
            if (!tenantConfig.StudentNoticeConfig.StudentAccountRechargeChangedWeChat && !tenantConfig.StudentNoticeConfig.StudentAccountRechargeChangedSms)
            {
                return;
            }
            var studentAccountRechargeView = await _studentAccountRechargeCoreBLL.GetStudentAccountRechargeByPhone(request.StudentAccountRecharge.Phone);
            var binderStudents = studentAccountRechargeView.Binders;
            if (binderStudents == null || binderStudents.Count() == 0)
            {
                Log.Info("[NoticeStudentAccountRechargeChangedConsumerEvent]充值账号未关联学员", request, this.GetType());
                return;
            }

            var req = new NoticeStudentAccountRechargeChangedRequest(await GetNoticeRequestBase(request.TenantId, tenantConfig.StudentNoticeConfig.StudentAccountRechargeChangedWeChat))
            {
                Students = new List<NoticeStudentAccountRechargeChangedStudent>(),
                AccountRechargePhone = request.StudentAccountRecharge.Phone,
                OtDesc = request.OtTime.EtmsToMinuteString(),
                BalanceDesc = request.StudentAccountRecharge.BalanceSum.ToString("F2"),
                BalanceRealDesc = request.StudentAccountRecharge.BalanceReal.ToString("F2"),
                BalanceGiveDesc = request.StudentAccountRecharge.BalanceGive.ToString("F2"),
                ChangeSumDesc = EtmsHelper.GetMoneyChangeDesc(request.AddBalanceReal + request.AddBalanceGive)
            };
            var wxConfig = _appConfigurtaionServices.AppSettings.WxConfig;
            if (tenantConfig.StudentNoticeConfig.StudentAccountRechargeChangedWeChat)
            {
                req.TemplateIdShort = wxConfig.TemplateNoticeConfig.StudentAccountRechargeChanged;
                req.Url = string.Format(wxConfig.TemplateNoticeConfig.StudentAccountRechargeUrl, request.StudentAccountRecharge.Id);
                req.Remark = tenantConfig.StudentNoticeConfig.WeChatNoticeRemark;
            }

            foreach (var s in binderStudents)
            {
                var studentBucket = await _studentDAL.GetStudent(s.StudentId);
                if (studentBucket == null || studentBucket.Student == null)
                {
                    Log.Warn($"[NoticeStudentAccountRechargeChangedConsumerEvent]未找到学员信息,TenantId:{request.TenantId},StudentId:{s.StudentId}", this.GetType());
                    continue;
                }
                var student = studentBucket.Student;
                if (string.IsNullOrEmpty(student.Phone))
                {
                    continue;
                }

                req.Students.Add(new NoticeStudentAccountRechargeChangedStudent()
                {
                    StudentId = student.Id,
                    Name = student.Name,
                    Phone = student.Phone,
                    OpendId = await GetOpenId(tenantConfig.StudentNoticeConfig.StudentAccountRechargeChangedWeChat, student.Phone)
                });
                if (!string.IsNullOrEmpty(student.PhoneBak) && EtmsHelper.IsMobilePhone(student.PhoneBak))
                {
                    req.Students.Add(new NoticeStudentAccountRechargeChangedStudent()
                    {
                        StudentId = student.Id,
                        Name = student.Name,
                        Phone = student.PhoneBak,
                        OpendId = await GetOpenId(tenantConfig.StudentNoticeConfig.StudentAccountRechargeChangedWeChat, student.PhoneBak)
                    });
                }
            }

            if (req.Students.Count > 0)
            {
                if (tenantConfig.StudentNoticeConfig.StudentAccountRechargeChangedSms)
                {
                    await _smsService.NoticeStudentAccountRechargeChanged(req);
                }
                if (tenantConfig.StudentNoticeConfig.StudentAccountRechargeChangedWeChat)
                {
                    _wxService.NoticeStudentAccountRechargeChanged(req);
                }
            }
        }

        public async Task NoticeStudentReservationConsumerEvent(NoticeStudentReservationEvent request)
        {
            var classTimesStudent = request.ClassTimesStudent;
            var classTimes = request.ClassTimes;
            if (classTimes == null)
            {
                classTimes = await _classTimesDAL.GetClassTimes(classTimesStudent.ClassTimesId);
            }
            if (classTimes == null)
            {
                Log.Error("[NoticeStudentReservationConsumerEvent]未找到课次", request, this.GetType());
                return;
            }
            var studentBucket = await _studentDAL.GetStudent(classTimesStudent.StudentId);
            if (studentBucket == null || studentBucket.Student == null)
            {
                Log.Error("[NoticeStudentReservationConsumerEvent]未找到学员信息", request, this.GetType());
                return;
            }
            var student = studentBucket.Student;
            if (string.IsNullOrEmpty(student.Phone))
            {
                return;
            }

            string title;
            if (request.OpType == NoticeStudentReservationOpType.Success)
            {
                title = "恭喜您成功预约课程";
            }
            else
            {
                title = "您预约的课程已取消";
            }
            var classBucket = await _classDAL.GetClassBucket(classTimes.ClassId);
            var courseBucket = await _courseDAL.GetCourse(classTimesStudent.CourseId);

            var req = new NoticeStudentOrUserReservationRequest(await GetNoticeRequestBase(request.TenantId))
            {
                StudentOrUsers = new List<NoticeStudentReservationStudent>(),
                Title = title,
                CourseDesc = $"{classBucket.EtClass.Name}({courseBucket.Item1.Name})",
                ClassOtDesc = $"{classTimes.ClassOt.EtmsToDateString()}(周{EtmsHelper.GetWeekDesc(classTimes.Week)}) {EtmsHelper.GetTimeDesc(classTimes.StartTime)}~{EtmsHelper.GetTimeDesc(classTimes.EndTime)}",
                StudentCount = classTimes.StudentCount.ToString()
            };
            var tenantConfig = await _tenantConfigDAL.GetTenantConfig();
            var wxConfig = _appConfigurtaionServices.AppSettings.WxConfig;
            req.TemplateIdShort = wxConfig.TemplateNoticeConfig.StudentReservation;
            req.Url = string.Empty;
            req.Remark = tenantConfig.StudentNoticeConfig.WeChatNoticeRemark;

            req.StudentOrUsers.Add(new NoticeStudentReservationStudent()
            {
                Name = student.Name,
                OpendId = await GetOpenId(true, student.Phone),
                Phone = student.Phone,
                StudentId = student.Id
            });

            if (!string.IsNullOrEmpty(student.PhoneBak) && EtmsHelper.IsMobilePhone(student.PhoneBak))
            {
                req.StudentOrUsers.Add(new NoticeStudentReservationStudent()
                {
                    Name = student.Name,
                    OpendId = await GetOpenId(true, student.PhoneBak),
                    Phone = student.PhoneBak,
                    StudentId = student.Id,
                });
            }

            if (req.StudentOrUsers.Count > 0)
            {
                _wxService.NoticeStudentOrUserReservation(req);
            }

            await this._userSendNoticeBLL.NoticeTeacherStudentReservation(request, req, classTimes, student);
        }

        public async Task NoticeStudentClassCheckSignRevokeConsumerEvent(NoticeStudentClassCheckSignRevokeEvent request)
        {
            var tenantConfig = await _tenantConfigDAL.GetTenantConfig();
            if (!tenantConfig.StudentNoticeConfig.ClassCheckSignWeChat)
            {
                return;
            }
            var classRecord = request.ClassRecord;
            var classRecordStudent = request.ClassRecordStudent;
            var classInfo = await _classDAL.GetClassBucket(classRecord.ClassId);
            if (classInfo == null || classInfo.EtClass == null)
            {
                Log.Warn($"[NoticeStudentClassCheckSignRevokeConsumerEvent]未找到上课班级,ClassRecordId:{classRecord.Id}", this.GetType());
                return;
            }

            var req = new NoticeStudentCustomizeMsgRequest(await GetNoticeRequestBase(request.TenantId, tenantConfig.StudentNoticeConfig.ClassCheckSignWeChat))
            {
                OtTime = DateTime.Now.EtmsToMinuteString(),
                Students = new List<NoticeStudentCustomizeMsgStudent>()
            };
            var wxConfig = _appConfigurtaionServices.AppSettings.WxConfig;
            if (tenantConfig.StudentNoticeConfig.ClassCheckSignWeChat)
            {
                req.TemplateIdShort = wxConfig.TemplateNoticeConfig.WxMessage;
                req.Remark = tenantConfig.StudentNoticeConfig.WeChatNoticeRemark;
            }

            foreach (var p in classRecordStudent)
            {
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

                var title = $"你所在的班级({classInfo.EtClass.Name})在{classRecord.ClassOt.EtmsToDateString()} {EtmsHelper.GetTimeDesc(classRecord.StartTime)}~{EtmsHelper.GetTimeDesc(classRecord.EndTime)}的上课记录已撤销";
                if (p.DeType == EmDeClassTimesType.ClassTimes)
                {
                    title = $"{title} 返还{p.DeClassTimes.EtmsToString()}课时";
                }
                req.Students.Add(new NoticeStudentCustomizeMsgStudent()
                {
                    Name = student.Name,
                    Msg = string.Empty,
                    OpendId = await GetOpenId(tenantConfig.StudentNoticeConfig.ClassCheckSignWeChat, student.Phone),
                    Phone = student.Phone,
                    StudentId = student.Id,
                    Title = title
                });
                if (!string.IsNullOrEmpty(student.PhoneBak) && EtmsHelper.IsMobilePhone(student.PhoneBak))
                {
                    req.Students.Add(new NoticeStudentCustomizeMsgStudent()
                    {
                        Name = student.Name,
                        Msg = string.Empty,
                        OpendId = await GetOpenId(tenantConfig.StudentNoticeConfig.ClassCheckSignWeChat, student.PhoneBak),
                        Phone = student.PhoneBak,
                        StudentId = student.Id,
                        Title = title
                    });
                }
            }

            if (req.Students.Count > 0)
            {
                if (tenantConfig.StudentNoticeConfig.ClassCheckSignWeChat)
                {
                    _wxService.NoticeStudentCustomizeMsg(req);
                }
            }
        }

        public async Task NoticeStudentActiveGrowthCommentConsumerEvent(NoticeStudentActiveGrowthCommentEvent request)
        {
            var pagingRequest = new TempActiveGrowthRecordDetailGetPagingRequest()
            {
                GrowthRecordId = request.ActiveGrowthRecordDetailComment.GrowthRecordId,
                IpAddress = string.Empty,
                IsDataLimit = false,
                LoginClientType = EmUserOperationLogClientType.PC,
                LoginTenantId = request.TenantId,
                LoginUserId = request.UserId,
                PageCurrent = 1,
                PageSize = 50
            };
            var activeGrowthRecordDetailPagingData = await _activeGrowthRecordDAL.GetDetailPaging(pagingRequest);
            if (activeGrowthRecordDetailPagingData.Item2 == 0)
            {
                return;
            }
            var req = new NoticeStudentMessageRequest(await GetNoticeRequestBase(request.TenantId, true))
            {
                Title = "老师评论了您的成长档案，赶快去看看吧！！！",
                OtDesc = request.ActiveGrowthRecordDetailComment.Ot.EtmsToMinuteString(),
                Content = "点击查看详情"
            };
            var wxConfig = _appConfigurtaionServices.AppSettings.WxConfig;
            var tenantConfig = await _tenantConfigDAL.GetTenantConfig();
            req.TemplateIdShort = wxConfig.TemplateNoticeConfig.WxMessage;
            req.Remark = tenantConfig.StudentNoticeConfig.WeChatNoticeRemark;

            await NoticeStudentActiveGrowthCommentConsumerEventHandle(request, activeGrowthRecordDetailPagingData.Item1, req,
                wxConfig.TemplateNoticeConfig.ParentActiveGrowthRecordDetailUrl);
            var totalPage = EtmsHelper.GetTotalPage(activeGrowthRecordDetailPagingData.Item2, pagingRequest.PageSize);
            pagingRequest.PageCurrent++;
            while (pagingRequest.PageCurrent <= totalPage)
            {
                LOG.Log.Info($"[NoticeStudentActiveGrowthCommentConsumerEvent]处理第{pagingRequest.PageCurrent}页的数据", this.GetType());
                var result = await _activeGrowthRecordDAL.GetDetailPaging(pagingRequest);
                NoticeStudentActiveGrowthCommentConsumerEventHandle(request, result.Item1, req,
                    wxConfig.TemplateNoticeConfig.ParentActiveGrowthRecordDetailUrl).Wait();
                pagingRequest.PageCurrent++;
            }
        }

        public async Task NoticeStudentActiveGrowthCommentConsumerEventHandle(NoticeStudentActiveGrowthCommentEvent request,
            IEnumerable<EtActiveGrowthRecordDetail> details, NoticeStudentMessageRequest req, string parentActiveGrowthRecordDetailUrl)
        {
            req.Students = new List<NoticeStudentMessageStudent>();
            if (details != null && details.Any())
            {
                foreach (var p in details)
                {
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
                    var url = string.Format(parentActiveGrowthRecordDetailUrl, p.Id, p.TenantId);
                    req.Students.Add(new NoticeStudentMessageStudent()
                    {
                        Name = student.Name,
                        OpendId = await GetOpenId(true, student.Phone),
                        Phone = student.Phone,
                        StudentId = student.Id,
                        Url = url
                    });
                    if (!string.IsNullOrEmpty(student.PhoneBak) && EtmsHelper.IsMobilePhone(student.PhoneBak))
                    {
                        req.Students.Add(new NoticeStudentMessageStudent()
                        {
                            Name = student.Name,
                            OpendId = await GetOpenId(true, student.PhoneBak),
                            Phone = student.PhoneBak,
                            StudentId = student.Id,
                            Url = url
                        });
                    }
                }
                if (req.Students.Any())
                {
                    _wxService.NoticeStudentMessage(req);
                }
            }
        }
    }
}
