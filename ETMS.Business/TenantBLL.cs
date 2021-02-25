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

namespace ETMS.Business
{
    public class TenantBLL : ITenantBLL
    {
        private readonly ISysTenantDAL _sysTenantDAL;

        private readonly ISmsLogDAL _smsLogDAL;

        private readonly ISysSafeSmsCodeCheckBLL _sysSafeSmsCodeCheckBLL;

        private readonly ISysVersionDAL _sysVersionDAL;

        public TenantBLL(ISysTenantDAL sysTenantDAL, ISmsLogDAL studentSmsLogDAL, ISysSafeSmsCodeCheckBLL sysSafeSmsCodeCheckBLL,
            ISysVersionDAL sysVersionDAL)
        {
            this._sysTenantDAL = sysTenantDAL;
            this._smsLogDAL = studentSmsLogDAL;
            this._sysSafeSmsCodeCheckBLL = sysSafeSmsCodeCheckBLL;
            this._sysVersionDAL = sysVersionDAL;
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
                VersionName = version.Name
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
        }

        public async Task<ResponseBase> SysSafeSmsSend(SysSafeSmsSendRequest request)
        {
            var sysTenantInfo = await _sysTenantDAL.GetTenant(request.LoginTenantId);
            return await _sysSafeSmsCodeCheckBLL.SysSafeSmsCodeSend(request.LoginTenantId, sysTenantInfo.Phone);
        }
    }
}
