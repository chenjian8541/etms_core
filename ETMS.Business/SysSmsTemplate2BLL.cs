using ETMS.Entity.Common;
using ETMS.Entity.Dto.BasicData.Output;
using ETMS.Entity.Dto.BasicData.Request;
using ETMS.IBusiness;
using ETMS.IDataAccess;
using ETMS.IDataAccess.EtmsManage;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using ETMS.Entity.Database.Manage;
using ETMS.Entity.Enum.EtmsManage;
using ETMS.Entity.Enum;
using ETMS.Business.Common;

namespace ETMS.Business
{
    public class SysSmsTemplate2BLL : ISysSmsTemplate2BLL
    {
        private readonly ISysSmsTemplateDAL _sysSmsTemplateDAL;

        private readonly IUserOperationLogDAL _userOperationLogDAL;

        private readonly ISysTenantDAL _sysTenantDAL;

        public SysSmsTemplate2BLL(ISysSmsTemplateDAL sysSmsTemplateDAL, IUserOperationLogDAL userOperationLogDAL, ISysTenantDAL sysTenantDAL)
        {
            this._sysSmsTemplateDAL = sysSmsTemplateDAL;
            this._userOperationLogDAL = userOperationLogDAL;
            this._sysTenantDAL = sysTenantDAL;
        }

        public void InitTenantId(int tenantId)
        {
            this.InitDataAccess(tenantId, _userOperationLogDAL);
        }

        public async Task<string> GetSmsTemplate(int tenantId, int type)
        {
            var hisData = await _sysSmsTemplateDAL.GetSysSmsTemplateByTypeDb(tenantId, type);
            if (hisData != null && hisData.HandleStatus == EmSysSmsTemplateHandleStatus.Pass)
            {
                return hisData.SmsContent;
            }
            var sysDefaultSysSmsTemplate = await _sysSmsTemplateDAL.GetSysSmsTemplates(0);
            var myData = sysDefaultSysSmsTemplate.FirstOrDefault(p => p.Type == type);
            return myData.SmsContent;
        }

        public async Task<ResponseBase> SysSmsTemplateGet(SysSmsTemplateGetRequest request)
        {
            var mySysSmsTemplates = await _sysSmsTemplateDAL.GetSysSmsTemplatesAll(request.LoginTenantId);
            var sysDefaultSysSmsTemplate = await _sysSmsTemplateDAL.GetSysSmsTemplates(0);
            var output = new List<SysSmsTemplateGetSmsContent>();
            for (var myType = 0; myType < 50; myType++)
            {
                SysSmsTemplate tempTypeSmsTemplate = null;
                if (mySysSmsTemplates.Any())
                {
                    tempTypeSmsTemplate = mySysSmsTemplates.FirstOrDefault(p => p.Type == myType);
                }
                if (tempTypeSmsTemplate == null)
                {
                    tempTypeSmsTemplate = sysDefaultSysSmsTemplate.FirstOrDefault(p => p.Type == myType);
                }
                if (tempTypeSmsTemplate == null)
                {
                    continue;
                }
                output.Add(new SysSmsTemplateGetSmsContent()
                {
                    Type = tempTypeSmsTemplate.Type,
                    AgentId = tempTypeSmsTemplate.AgentId,
                    CreateOt = tempTypeSmsTemplate.CreateOt,
                    HandleContent = tempTypeSmsTemplate.HandleContent,
                    HandleOt = tempTypeSmsTemplate.HandleOt,
                    HandleStatus = tempTypeSmsTemplate.HandleStatus,
                    HandleStatusDesc = EmSysSmsTemplateHandleStatus.GetEmSysSmsTemplateHandleStatusDesc(tempTypeSmsTemplate.HandleStatus),
                    Id = tempTypeSmsTemplate.TenantId == 0 ? 0 : tempTypeSmsTemplate.Id,
                    SmsContent = tempTypeSmsTemplate.SmsContent,
                    TenantId = tempTypeSmsTemplate.TenantId,
                    TypeDesc = EmSysSmsTemplateType.GetSysSmsTemplateTypeDesc(tempTypeSmsTemplate.Type),
                    UpdateOt = tempTypeSmsTemplate.UpdateOt
                });
            }
            return ResponseBase.Success(new SysSmsTemplateGetOutput()
            {
                SmsContent = output.OrderBy(p => p.Type),
                SmsTemplate = SmsTemplateAnalyze.SmsTemplate
            });
        }

        public async Task<ResponseBase> SysSmsTemplateSave(SysSmsTemplateSaveRequest request)
        {
            var hisData = await _sysSmsTemplateDAL.GetSysSmsTemplateByTypeDb(request.LoginTenantId, request.Type);
            if (hisData != null)
            {
                hisData.HandleStatus = EmSysSmsTemplateHandleStatus.Unreviewed;
                hisData.SmsContent = request.SmsContent;
            }
            else
            {
                var myTenant = await _sysTenantDAL.GetTenant(request.LoginTenantId);
                var now = DateTime.Now;
                hisData = new SysSmsTemplate()
                {
                    SmsContent = request.SmsContent,
                    HandleStatus = EmSysSmsTemplateHandleStatus.Unreviewed,
                    AgentId = myTenant.AgentId,
                    CreateOt = now,
                    HandleContent = string.Empty,
                    HandleOt = null,
                    HandleUser = null,
                    IsDeleted = EmIsDeleted.Normal,
                    Remark = string.Empty,
                    TenantId = myTenant.Id,
                    Type = request.Type,
                    UpdateOt = now
                };
            }
            await _sysSmsTemplateDAL.SaveSysSmsTemplate(hisData);

            await _userOperationLogDAL.AddUserLog(request, $"短信模板设置:{request.SmsContent}", EmUserOperationType.SmsSmsTemplate);
            return ResponseBase.Success();
        }

        public async Task<ResponseBase> SysSmsTemplateDel(SysSmsTemplateResetRequest request)
        {
            var hisData = await _sysSmsTemplateDAL.GetSysSmsTemplateByTypeDb(request.LoginTenantId, request.Type);
            if (hisData == null)
            {
                return ResponseBase.CommonError("未找到短信模板设置信息");
            }
            await _sysSmsTemplateDAL.DelSysSmsTemplate(hisData.Id);

            await _userOperationLogDAL.AddUserLog(request, $"重置短信模板:{hisData.SmsContent}", EmUserOperationType.SmsSmsTemplate);
            return ResponseBase.Success();
        }
    }
}
