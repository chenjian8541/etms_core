using ETMS.Entity.Common;
using ETMS.Entity.EtmsManage.Dto.DataLog.Output;
using ETMS.Entity.EtmsManage.Dto.DataLog.Request;
using ETMS.IBusiness.EtmsManage;
using ETMS.IDataAccess.EtmsManage;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using ETMS.Entity.Database.Manage;
using ETMS.Business.EtmsManage.Common;
using ETMS.Entity.Enum.EtmsManage;

namespace ETMS.Business.EtmsManage
{
    public class SysSmsTemplateBLL : ISysSmsTemplateBLL
    {
        private readonly ISysSmsTemplateDAL _sysSmsTemplateDAL;

        private readonly ISysAgentLogDAL _sysAgentLogDAL;

        private readonly ISysTenantDAL _sysTenantDAL;

        public SysSmsTemplateBLL(ISysSmsTemplateDAL sysSmsTemplateDAL, ISysAgentLogDAL sysAgentLogDAL, ISysTenantDAL sysTenantDAL)
        {
            this._sysSmsTemplateDAL = sysSmsTemplateDAL;
            this._sysAgentLogDAL = sysAgentLogDAL;
            this._sysTenantDAL = sysTenantDAL;
        }

        public async Task<ResponseBase> SysSmsTemplateGetPaging(SysSmsTemplateGetPagingRequest request)
        {
            var pagingData = await _sysSmsTemplateDAL.GetPaging(request);
            var output = new List<SysSmsTemplateGetPagingOutput>();
            if (pagingData.Item1.Any())
            {
                var tempBoxTenant = new AgentDataTempBox<SysTenant>();
                foreach (var p in pagingData.Item1)
                {
                    var tenant = await AgentComBusiness.GetTenant(tempBoxTenant, _sysTenantDAL, p.TenantId);
                    if (tenant == null)
                    {
                        continue;
                    }
                    output.Add(new SysSmsTemplateGetPagingOutput()
                    {
                        AgentId = p.AgentId,
                        CreateOt = p.CreateOt,
                        HandleContent = p.HandleContent,
                        HandleOt = p.HandleOt,
                        HandleStatus = p.HandleStatus,
                        HandleStatusDesc = EmSysSmsTemplateHandleStatus.GetEmSysSmsTemplateHandleStatusDesc(p.HandleStatus),
                        SmsContent = p.SmsContent,
                        TenantId = p.TenantId,
                        TenantName = tenant.Name,
                        Type = p.Type,
                        TypeDesc = EmSysSmsTemplateType.GetSysSmsTemplateTypeDesc(p.Type),
                        UpdateOt = p.UpdateOt,
                        Id = p.Id
                    });
                }
            }
            return ResponseBase.Success(new ResponsePagingDataBase<SysSmsTemplateGetPagingOutput>(pagingData.Item2, output));
        }
        public async Task<ResponseBase> SysSmsTemplateHandle(SysSmsTemplateHandleRequest request)
        {
            var log = await _sysSmsTemplateDAL.GetSysSmsTemplate(request.Id);
            if (log == null)
            {
                return ResponseBase.CommonError("短信模板不存在");
            }
            if (log.HandleStatus != EmSysSmsTemplateHandleStatus.Unreviewed)
            {
                return ResponseBase.CommonError("短信模板已审核");
            }
            log.HandleStatus = request.NewHandleStatus;
            log.HandleUser = request.LoginUserId;
            log.HandleOt = DateTime.Now;
            log.HandleContent = request.HandleContent;
            await _sysSmsTemplateDAL.SaveSysSmsTemplate(log);

            await _sysAgentLogDAL.AddSysAgentOpLog(request, $"审核短信模板:{log.SmsContent}", EmSysAgentOpLogType.EtmsSysSetting);
            return ResponseBase.Success();
        }
    }
}