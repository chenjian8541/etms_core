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

namespace ETMS.Business
{
    public class TenantBLL : ITenantBLL
    {
        private readonly ISysTenantDAL _sysTenantDAL;

        private readonly ISmsLogDAL _smsLogDAL;

        private readonly ISysSafeSmsCodeCheckBLL _sysSafeSmsCodeCheckBLL;

        private readonly ISysVersionDAL _sysVersionDAL;

        private readonly ISysSmsLogDAL _sysSmsLogDAL;

        public TenantBLL(ISysTenantDAL sysTenantDAL, ISmsLogDAL studentSmsLogDAL, ISysSafeSmsCodeCheckBLL sysSafeSmsCodeCheckBLL,
            ISysVersionDAL sysVersionDAL, ISysSmsLogDAL sysSmsLogDAL)
        {
            this._sysTenantDAL = sysTenantDAL;
            this._smsLogDAL = studentSmsLogDAL;
            this._sysSafeSmsCodeCheckBLL = sysSafeSmsCodeCheckBLL;
            this._sysVersionDAL = sysVersionDAL;
            this._sysSmsLogDAL = sysSmsLogDAL;
        }

        public void InitTenantId(int tenantId)
        {
            this.InitDataAccess(tenantId, _smsLogDAL);
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
    }
}
