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
            return await _parentBLL.ParentLoginBySms(new ParentLoginBySmsRequest()
            {
                Code = string.Empty,
                IpAddress = string.Empty,
                Phone = request.Phone,
                SmsCode = request.SmsCode,
                StudentWechartId = string.Empty,
                TenantNo = request.TenantNo
            });
        }
    }
}
