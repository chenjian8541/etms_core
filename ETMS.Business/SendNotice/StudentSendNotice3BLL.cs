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

        public StudentSendNotice3BLL(IStudentWechatDAL studentWechatDAL, IComponentAccessBLL componentAccessBLL, ISysTenantDAL sysTenantDAL, IWxService wxService, IAppConfigurtaionServices appConfigurtaionServices, ISmsService smsService,
            ITenantConfigDAL tenantConfigDAL, IStudentDAL studentDAL, ICouponsDAL couponsDAL)
            : base(studentWechatDAL, componentAccessBLL, sysTenantDAL)
        {
            this._wxService = wxService;
            this._appConfigurtaionServices = appConfigurtaionServices;
            this._smsService = smsService;
            this._tenantConfigDAL = tenantConfigDAL;
            this._studentDAL = studentDAL;
            this._couponsDAL = couponsDAL;
        }

        public void InitTenantId(int tenantId)
        {
            this.InitDataAccess(tenantId, _studentWechatDAL, _tenantConfigDAL, _studentDAL, _couponsDAL);
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
    }
}
