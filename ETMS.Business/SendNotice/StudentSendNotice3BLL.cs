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

        public StudentSendNotice3BLL(IStudentWechatDAL studentWechatDAL, IComponentAccessBLL componentAccessBLL, ISysTenantDAL sysTenantDAL, IWxService wxService, IAppConfigurtaionServices appConfigurtaionServices, ISmsService smsService,
            ITenantConfigDAL tenantConfigDAL, IStudentDAL studentDAL, ICouponsDAL couponsDAL, IParentStudentDAL parentStudentDAL,
            IClassTimesDAL classTimesDAL, IClassDAL classDAL, ICourseDAL courseDAL, IStudentAccountRechargeCoreBLL studentAccountRechargeCoreBLL)
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
        }

        public void InitTenantId(int tenantId)
        {
            this._studentAccountRechargeCoreBLL.InitTenantId(tenantId);
            this.InitDataAccess(tenantId, _studentWechatDAL, _tenantConfigDAL, _studentDAL, _couponsDAL,
                _parentStudentDAL, _classTimesDAL, _classDAL, _courseDAL);
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
                await _classTimesDAL.GetClassTimes(classTimesStudent.ClassTimesId);
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

            var title = string.Empty;
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

            var req = new NoticeStudentReservationRequest(await GetNoticeRequestBase(request.TenantId))
            {
                Students = new List<NoticeStudentReservationStudent>(),
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

            req.Students.Add(new NoticeStudentReservationStudent()
            {
                Name = student.Name,
                OpendId = await GetOpenId(true, student.Phone),
                Phone = student.Phone,
                StudentId = student.Id
            });

            if (!string.IsNullOrEmpty(student.PhoneBak) && EtmsHelper.IsMobilePhone(student.PhoneBak))
            {
                req.Students.Add(new NoticeStudentReservationStudent()
                {
                    Name = student.Name,
                    OpendId = await GetOpenId(true, student.PhoneBak),
                    Phone = student.PhoneBak,
                    StudentId = student.Id,
                });
            }

            if (req.Students.Count > 0)
            {
                _wxService.NoticeStudentReservation(req);
            }
        }
    }
}
