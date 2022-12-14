using ETMS.Business.Common;
using ETMS.Entity.Common;
using ETMS.Entity.Dto.OpenApi99.Output;
using ETMS.Entity.Dto.OpenApi99.Request;
using ETMS.ExternalService.Contract;
using ETMS.IBusiness.OpenApi99;
using ETMS.IDataAccess.EtmsManage;
using ETMS.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.Business.OpenApi99
{
    public class TenantOpenBLL : ITenantOpenBLL
    {
        private readonly ISysTenantDAL _sysTenantDAL;

        private readonly ISmsService _smsService;

        private readonly ISysAIFaceBiduAccountDAL _sysAIFaceBiduAccountDAL;

        private readonly ITenantLcsAccountDAL _tenantLcsAccountDAL;

        public TenantOpenBLL(ISysTenantDAL sysTenantDAL, ISmsService smsService,
            ISysAIFaceBiduAccountDAL sysAIFaceBiduAccountDAL, ITenantLcsAccountDAL tenantLcsAccountDAL)
        {
            this._sysTenantDAL = sysTenantDAL;
            this._smsService = smsService;
            this._sysAIFaceBiduAccountDAL = sysAIFaceBiduAccountDAL;
            this._tenantLcsAccountDAL = tenantLcsAccountDAL;
        }

        public void InitTenantId(int tenantId)
        {
        }

        public async Task<ResponseBase> TenantInfoGet(OpenApi99Base request)
        {
            var myTenant = await _sysTenantDAL.GetTenant(request.LoginTenantId);
            if (!ComBusiness2.CheckTenantCanLogin(myTenant, out var myMsg))
            {
                return ResponseBase.CommonError(myMsg);
            }
            var aIFaceBiduAccount = await _sysAIFaceBiduAccountDAL.GetSysAIFaceBiduAccount(myTenant.BaiduCloudId);
            var strFaceApiAppid = string.Empty;
            var strFaceApiApiKey = string.Empty;
            var strFaceApiSecretKey = string.Empty;
            if (aIFaceBiduAccount != null)
            {
                strFaceApiAppid = EtmsHelper2.GetEncryptOpenApi99(aIFaceBiduAccount.Appid);
                strFaceApiApiKey = EtmsHelper2.GetEncryptOpenApi99(aIFaceBiduAccount.ApiKey);
                strFaceApiSecretKey = EtmsHelper2.GetEncryptOpenApi99(aIFaceBiduAccount.SecretKey);
            }
            var output = new TenantInfoGetOutput()
            {
                Address = myTenant.Address,
                ExDate = myTenant.ExDate,
                IdCard = myTenant.IdCard,
                LinkMan = myTenant.LinkMan,
                Name = myTenant.Name,
                MaxUserCount = myTenant.MaxUserCount,
                Phone = myTenant.Phone,
                SmsCount = myTenant.SmsCount,
                SmsSignature = myTenant.SmsSignature,
                TenantCode = myTenant.TenantCode,
                FaceApiApiKey = strFaceApiApiKey,
                FaceApiAppid = strFaceApiAppid,
                FaceApiSecretKey = strFaceApiSecretKey,
                BuyStatus = myTenant.BuyStatus,
                LcswApplyStatus = myTenant.LcswApplyStatus,
                LcswOpenStatus = myTenant.LcswOpenStatus,
                Ot = myTenant.Ot,
                Status = myTenant.Status
            };
            return ResponseBase.Success(output);
        }

        public async Task<ResponseBase> TenantLcsAccountGet(OpenApi99Base request)
        {
            var p = await _tenantLcsAccountDAL.GetTenantLcsAccount(request.LoginTenantId);
            if (p == null)
            {
                return ResponseBase.Success();
            }
            return ResponseBase.Success(new TenantLcsAccountGetOutput()
            {
                AccessToken = p.AccessToken,
                ChangeTime = p.ChangeTime,
                CreationTime = p.CreationTime,
                InstNo = p.InstNo,
                LcswApplyStatus = p.LcswApplyStatus,
                MerchantCompany = p.MerchantCompany,
                MerchantInfoData = p.MerchantInfoData,
                MerchantName = p.MerchantName,
                MerchantNo = p.MerchantNo,
                MerchantRquestData = p.MerchantRquestData,
                MerchantStatus = p.MerchantStatus,
                MerchantType = p.MerchantType,
                ResultCode = p.ResultCode,
                ReturnCode = p.ReturnCode,
                ReturnMsg = p.ReturnMsg,
                ReviewTime = p.ReviewTime,
                StoreCode = p.StoreCode,
                TerminalId = p.TerminalId,
                TerminalName = p.TerminalName,
                TraceNo = p.TraceNo
            });
        }

        public async Task<ResponseBase> SmsSend(SmsSendRequest request)
        {
            var sendSmsResult = await _smsService.TenantOpenApi99SendSms(new Entity.ExternalService.Dto.Request.TenantOpenApi99SendSmsRequest(request.LoginTenantId)
            {
                Phones = request.Phones,
                SmsContent = request.SmsContent
            });
            if (sendSmsResult.IsSuccess)
            {
                return ResponseBase.Success();
            }
            return ResponseBase.CommonError("发送短信失败");
        }

        public async Task<ResponseBase> SmsSendValidCode(SmsSendValidCodeRequest request)
        {
            var sendSmsResult = await _smsService.SysSafe(new Entity.ExternalService.Dto.Request.SmsSysSafeRequest(request.LoginTenantId)
            {
                LoginTenantId = request.LoginTenantId,
                Phone = request.Phone,
                ValidCode = request.ValidCode
            });
            if (sendSmsResult.IsSuccess)
            {
                return ResponseBase.Success();
            }
            return ResponseBase.CommonError("发送短信失败");
        }
    }
}
