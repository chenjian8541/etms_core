using ETMS.Entity.Common;
using ETMS.Entity.ExternalService.Dto.Request;
using ETMS.ExternalService.Contract;
using ETMS.IBusiness.SysOp;
using ETMS.IDataAccess;
using ETMS.Utility;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.Business.SysOp
{
    public class SysSafeSmsCodeCheckBLL : ISysSafeSmsCodeCheckBLL
    {
        private readonly ISysSafeSmsCodeDAL _sysSafeSmsCodeDAL;

        private readonly ISmsService _smsService;

        public SysSafeSmsCodeCheckBLL(ISysSafeSmsCodeDAL sysSafeSmsCodeDAL, ISmsService smsService)
        {
            this._sysSafeSmsCodeDAL = sysSafeSmsCodeDAL;
            this._smsService = smsService;
        }

        public async Task<ResponseBase> SysSafeSmsCodeSend(int tenantId, string phone)
        {
            var smsCode = RandomHelper.GetSmsCode();
            var sendSmsRes = await _smsService.SysSafe(new SmsSysSafeRequest(tenantId)
            {
                Phone = phone,
                ValidCode = smsCode
            });
            if (!sendSmsRes.IsSuccess)
            {
                return ResponseBase.CommonError("发送短信失败,请稍后再试");
            }
            _sysSafeSmsCodeDAL.AddSysSafeSmsCode(tenantId, smsCode);
            return ResponseBase.Success();
        }

        public ResponseBase SysSafeSmsCodeCheck(int tenantId, string smsCode)
        {
            var safeSms = _sysSafeSmsCodeDAL.GetSysSafeSmsCode(tenantId);
            if (safeSms == null || safeSms.ExpireAtTime < DateTime.Now || safeSms.SmsCode != smsCode)
            {
                return ResponseBase.CommonError("验证码错误");
            }
            _sysSafeSmsCodeDAL.RemoveSysSafeSmsCode(tenantId);
            return ResponseBase.Success();
        }
    }
}
