using ETMS.IBusiness;
using ETMS.Entity.Common;
using ETMS.Entity.Config;
using ETMS.Entity.Dto.Open2.Request;
using ETMS.IBusiness.Wechart;
using ETMS.LOG;
using ETMS.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Threading.Tasks;
using ETMS.Entity.Dto.OpenParent.Request;
using ETMS.Business.Common;
using ETMS.IDataAccess.EtmsManage;
using ETMS.IDataAccess;
using ETMS.Entity.Dto.Parent.Request;
using ETMS.Entity.Dto.Parent.Output;
using ETMS.Entity.Dto.OpenParent.Output;

namespace ETMS.Business
{
    public class OpenParentBLL : IOpenParentBLL
    {
        private readonly IParentBLL _parentBLL;

        public OpenParentBLL(IParentBLL parentBLL)
        {
            this._parentBLL = parentBLL;
        }

        public async Task<ResponseBase> ParentLoginSendSms(ParentOpenLoginSendSmsRequest request)
        {
            return await _parentBLL.ParentLoginSendSms(new ParentLoginSendSmsRequest()
            {
                Code = string.Empty,
                Phone = request.Phone,
                TenantNo = request.TenantNo
            });
        }

        public async Task<ResponseBase> ParentLoginBySms(ParentOpenLoginBySmsRequest request)
        {
            var res = await _parentBLL.ParentLoginBySms(new ParentLoginBySmsRequest()
            {
                Code = string.Empty,
                IpAddress = string.Empty,
                Phone = request.Phone,
                SmsCode = request.SmsCode,
                StudentWechartId = string.Empty,
                TenantNo = request.TenantNo
            });
            if (res.IsResponseSuccess())
            {
                var result = res.resultData as ParentLoginBySmsOutput;
                return ResponseBase.Success(new ParentLoginBySmsOpenOutput()
                {
                    LoginStatus = ParentLoginBySmsOutputLoginStatus.Success,
                    ExpiresIn = result.ExpiresIn,
                    L = result.L,
                    S = result.S
                });
            }
            if (res.ExtCode == StatusCode.ParentUnBindStudent)
            {
                return ResponseBase.Success(new ParentLoginBySmsOpenOutput()
                {
                    LoginStatus = ParentLoginBySmsOutputLoginStatus.Unregistered
                });
            }
            return res;
        }

        public async Task<ResponseBase> ParentRegisterSendSms(ParentRegisterSendSmsRequest request)
        {
            return await _parentBLL.ParentLoginSendSms(new ParentLoginSendSmsRequest()
            {
                Code = string.Empty,
                Phone = request.Phone,
                TenantNo = request.TenantNo
            });
        }

        public async Task<ResponseBase> ParentRegister(ParentRegisterOpenRequest request)
        {
            return await _parentBLL.ParentRegister(new ParentRegisterRequest()
            {
                Address = request.Address,
                Phone = request.Phone,
                Remark = request.Remark,
                StudentName = request.StudentName,
                SmsCode = request.SmsCode,
                TenantNo = request.TenantNo,
                Code = string.Empty
            });
        }
    }
}
