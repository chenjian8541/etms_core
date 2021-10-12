using ETMS.Entity.Common;
using ETMS.Entity.Dto.BasicData.Request;
using ETMS.Event.DataContract;
using ETMS.IBusiness;
using ETMS.IDataAccess;
using ETMS.IDataAccess.EtmsManage;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using ETMS.IBusiness.SysOp;
using ETMS.Entity.Dto.BasicData.Output;
using ETMS.Entity.Enum.EtmsManage;
using ETMS.Utility;
using ETMS.Entity.Database.Manage;
using ETMS.Entity.Enum;
using ETMS.Entity.Database.Source;
using ETMS.Business.Common;

namespace ETMS.Business
{
    public class TenantBLL : ITenantBLL
    {
        private readonly ISysTenantDAL _sysTenantDAL;

        private readonly ISmsLogDAL _smsLogDAL;

        private readonly ISysSafeSmsCodeCheckBLL _sysSafeSmsCodeCheckBLL;

        private readonly ISysVersionDAL _sysVersionDAL;

        private readonly ISysSmsLogDAL _sysSmsLogDAL;

        private readonly IStudentDAL _studentDAL;

        private readonly ICourseDAL _courseDAL;

        private readonly IClassDAL _classDAL;

        private readonly INoticeConfigDAL _noticeConfigDAL;

        private readonly IUserOperationLogDAL _userOperationLogDAL;

        public TenantBLL(ISysTenantDAL sysTenantDAL, ISmsLogDAL studentSmsLogDAL, ISysSafeSmsCodeCheckBLL sysSafeSmsCodeCheckBLL,
            ISysVersionDAL sysVersionDAL, ISysSmsLogDAL sysSmsLogDAL, IStudentDAL studentDAL, ICourseDAL courseDAL, IClassDAL classDAL,
            INoticeConfigDAL noticeConfigDAL, IUserOperationLogDAL userOperationLogDAL)
        {
            this._sysTenantDAL = sysTenantDAL;
            this._smsLogDAL = studentSmsLogDAL;
            this._sysSafeSmsCodeCheckBLL = sysSafeSmsCodeCheckBLL;
            this._sysVersionDAL = sysVersionDAL;
            this._sysSmsLogDAL = sysSmsLogDAL;
            this._studentDAL = studentDAL;
            this._courseDAL = courseDAL;
            this._classDAL = classDAL;
            this._noticeConfigDAL = noticeConfigDAL;
            this._userOperationLogDAL = userOperationLogDAL;
        }

        public void InitTenantId(int tenantId)
        {
            this.InitDataAccess(tenantId, _smsLogDAL, _studentDAL, _courseDAL, _classDAL, _noticeConfigDAL, _userOperationLogDAL);
        }

        public async Task<ResponseBase> TenantGet(TenantGetRequest request)
        {
            var myTenant = await _sysTenantDAL.GetTenant(request.LoginTenantId);
            return ResponseBase.Success(myTenant);
        }

        public async Task<ResponseBase> TenantGetView(TenantGetRequest request)
        {
            var myTenant = await _sysTenantDAL.GetTenant(request.LoginTenantId);
            var version = await _sysVersionDAL.GetVersion(myTenant.VersionId);
            var output = new TenantGetViewOutput()
            {
                Phone = myTenant.Phone,
                VersionId = myTenant.VersionId,
                Address = myTenant.Address,
                BuyStatus = myTenant.BuyStatus,
                BuyStatusDesc = EmSysTenantBuyStatus.GetSysTenantBuyStatusDesc(myTenant.BuyStatus),
                ExDateDesc = myTenant.ExDate.Date.AddDays(1).AddMinutes(-1).EtmsToMinuteString(),
                LinkMan = myTenant.LinkMan,
                MaxUserCount = myTenant.MaxUserCount,
                Name = myTenant.Name,
                OtDesc = myTenant.Ot.EtmsToDateString(),
                SmsCount = myTenant.SmsCount,
                SmsSignature = myTenant.SmsSignature,
                Status = myTenant.Status,
                StatusDesc = EmSysTenantStatus.GetSysTenantStatusDesc(myTenant.Status, myTenant.ExDate),
                TenantCode = myTenant.TenantCode,
                VersionName = version.Name,
                IsLimitExDate = myTenant.ExDate.Date.AddMonths(-1) <= DateTime.Now.Date
            };
            return ResponseBase.Success(output);
        }

        public async Task TenantSmsDeductionEventConsume(TenantSmsDeductionEvent request)
        {
            var totalDeCount = 0;
            if (request.StudentSmsLogs != null && request.StudentSmsLogs.Count > 0)
            {
                totalDeCount += request.StudentSmsLogs.Sum(p => p.DeCount);
                await _smsLogDAL.AddStudentSmsLog(request.StudentSmsLogs);
            }
            if (request.UserSmsLogs != null && request.UserSmsLogs.Count > 0)
            {
                totalDeCount += request.UserSmsLogs.Sum(p => p.DeCount);
                await _smsLogDAL.AddUserSmsLog(request.UserSmsLogs);
            }
            await _sysTenantDAL.TenantSmsDeduction(request.TenantId, totalDeCount);

            //处理机构短信记录统计
            var smsLogs = new List<SysSmsLog>();
            var myTenant = await _sysTenantDAL.GetTenant(request.TenantId);
            if (request.StudentSmsLogs != null && request.StudentSmsLogs.Count > 0)
            {
                foreach (var p in request.StudentSmsLogs)
                {
                    smsLogs.Add(new SysSmsLog()
                    {
                        TenantId = p.TenantId,
                        AgentId = myTenant.AgentId,
                        DeCount = p.DeCount,
                        IsDeleted = p.IsDeleted,
                        Ot = p.Ot,
                        Phone = p.Phone,
                        Remark = string.Empty,
                        RetType = EmPeopleType.Student,
                        SmsContent = p.SmsContent,
                        Status = p.Status,
                        Type = p.Type
                    });
                }
            }
            if (request.UserSmsLogs != null && request.UserSmsLogs.Count > 0)
            {
                foreach (var p in request.UserSmsLogs)
                {
                    smsLogs.Add(new SysSmsLog()
                    {
                        TenantId = p.TenantId,
                        AgentId = myTenant.AgentId,
                        DeCount = p.DeCount,
                        IsDeleted = p.IsDeleted,
                        Ot = p.Ot,
                        Phone = p.Phone,
                        Remark = string.Empty,
                        RetType = EmPeopleType.User,
                        SmsContent = p.SmsContent,
                        Status = p.Status,
                        Type = p.Type
                    });
                }
            }
            if (smsLogs.Any())
            {
                await _sysSmsLogDAL.AddSysSmsLog(smsLogs);
            }
        }

        public async Task<ResponseBase> SysSafeSmsSend(SysSafeSmsSendRequest request)
        {
            var sysTenantInfo = await _sysTenantDAL.GetTenant(request.LoginTenantId);
            return await _sysSafeSmsCodeCheckBLL.SysSafeSmsCodeSend(request.LoginTenantId, sysTenantInfo.Phone);
        }

        public ResponseBase SysSafeSmsCheck(SysSafeSmsCheckRequest request)
        {
            return _sysSafeSmsCodeCheckBLL.SysSafeSmsCodeCheck(request.LoginTenantId, request.SmsCode);
        }

        public async Task<ResponseBase> NoticeConfigGet(NoticeConfigGetRequest request)
        {
            var log = await _noticeConfigDAL.GetNoticeConfig(EmNoticeConfigType.NoticePeopleLimit, request.PeopleType, request.ScenesType);
            if (log == null)
            {
                return ResponseBase.Success(null);
            }
            var output = new NoticeConfigGetOutput()
            {
                ExType = log.ExType,
                MyItems = new List<NoticeConfigGetItem>()
            };
            var ids = EtmsHelper.AnalyzeMuIds(log.ConfigValue);
            if (ids.Any())
            {
                switch (log.ExType)
                {
                    case EmNoticeConfigExType.Course:
                        foreach (var p in ids)
                        {
                            var myCourse = await _courseDAL.GetCourse(p);
                            if (myCourse == null || myCourse.Item1 == null)
                            {
                                continue;
                            }
                            output.MyItems.Add(new NoticeConfigGetItem()
                            {
                                Key = p,
                                Label = myCourse.Item1.Name
                            });
                        }
                        break;
                    case EmNoticeConfigExType.Class:
                        foreach (var p in ids)
                        {
                            var myClass = await _classDAL.GetClassBucket(p);
                            if (myClass == null || myClass.EtClass == null)
                            {
                                continue;
                            }
                            output.MyItems.Add(new NoticeConfigGetItem()
                            {
                                Key = p,
                                Label = myClass.EtClass.Name
                            });
                        }
                        break;
                    case EmNoticeConfigExType.People:
                        foreach (var p in ids)
                        {
                            var myStudent = await _studentDAL.GetStudent(p);
                            if (myStudent == null || myStudent.Student == null)
                            {
                                continue;
                            }
                            output.MyItems.Add(new NoticeConfigGetItem()
                            {
                                Key = p,
                                Label = myStudent.Student.Name
                            });
                        }
                        break;
                }
            }
            return ResponseBase.Success(output);
        }

        public async Task<ResponseBase> NoticeConfigSave(NoticeConfigSaveRequest request)
        {
            var entity = new EtNoticeConfig()
            {
                IsDeleted = EmIsDeleted.Normal,
                PeopleType = request.PeopleType,
                ScenesType = request.ScenesType,
                ExType = request.ExType,
                TenantId = request.LoginTenantId,
                Type = EmNoticeConfigType.NoticePeopleLimit,
                ConfigValue = EtmsHelper.GetMuIds(request.ConfigValues)
            };
            await _noticeConfigDAL.SaveNoticeConfig(entity);

            await _userOperationLogDAL.AddUserLog(request, "通知推送名单设置", EmUserOperationType.SystemConfigModify);
            return ResponseBase.Success();
        }

        public async Task<ResponseBase> PageBascDataGet(RequestBase request)
        {
            var output = new PageBascDataGetOutput()
            {
                IsOpenLcsPay = false
            };
            var myTenant = await _sysTenantDAL.GetTenant(request.LoginTenantId);
            output.IsOpenLcsPay = ComBusiness4.GetIsOpenLcsPay(myTenant.LcswApplyStatus, myTenant.LcswOpenStatus);
            return ResponseBase.Success(output);
        }
    }
}
