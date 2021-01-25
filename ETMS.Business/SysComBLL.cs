using ETMS.Entity.Common;
using ETMS.Entity.Dto.SysCom.Output;
using ETMS.Entity.Dto.SysCom.Request;
using ETMS.Entity.Enum;
using ETMS.IBusiness;
using ETMS.IBusiness.EtmsManage;
using ETMS.IDataAccess.EtmsManage;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.Business
{
    public class SysComBLL : ISysComBLL
    {
        private readonly ISysUpgradeMsgDAL _sysUpgradeMsgDAL;

        private readonly ISysTenantDAL _sysTenantDAL;

        private readonly ISysExplainDAL _sysExplainDAL;

        private readonly ISysAgentDAL _sysAgentDAL;

        private readonly ISysAppsettingsBLL _sysAppsettingsBLL;

        public SysComBLL(ISysUpgradeMsgDAL sysUpgradeMsgDAL, ISysTenantDAL sysTenantDAL, ISysExplainDAL sysExplainDAL,
            ISysAgentDAL sysAgentDAL, ISysAppsettingsBLL sysAppsettingsBLL)
        {
            this._sysUpgradeMsgDAL = sysUpgradeMsgDAL;
            this._sysTenantDAL = sysTenantDAL;
            this._sysExplainDAL = sysExplainDAL;
            this._sysAgentDAL = sysAgentDAL;
            this._sysAppsettingsBLL = sysAppsettingsBLL;
        }

        public void InitTenantId(int tenantId)
        {
        }

        public async Task<ResponseBase> SysUpgradeGet(SysUpgradeGetRequest request)
        {
            var output = new SysUpgradeGetOutput()
            {
                IsHaveUpgrade = false
            };
            var upgradeGetMsg = await _sysUpgradeMsgDAL.GetLastSysUpgradeMsg();
            if (upgradeGetMsg == null || upgradeGetMsg.StartTime <= DateTime.Now)
            {
                return ResponseBase.Success(output);
            }
            if (await _sysUpgradeMsgDAL.GetUserIsRead(upgradeGetMsg.Id, request.LoginTenantId, request.LoginUserId))
            {
                return ResponseBase.Success(output);
            }
            output.IsHaveUpgrade = true;
            output.UpgradeInfo = new UpgradeInfo()
            {
                EndTime = upgradeGetMsg.EndTime,
                StartTime = upgradeGetMsg.StartTime,
                Title = upgradeGetMsg.Title,
                UpContent = upgradeGetMsg.UpContent,
                UpgradeId = upgradeGetMsg.Id,
                VersionNo = upgradeGetMsg.VersionNo
            };
            return ResponseBase.Success(output);
        }

        public async Task<ResponseBase> SysUpgradeSetRead(SysUpgradeSetReadRequest request)
        {
            await _sysUpgradeMsgDAL.SetRead(request.UpgradeId, request.LoginTenantId, request.LoginUserId);
            return ResponseBase.Success();
        }

        public async Task<ResponseBase> SysKefu(SysKefuRequest request)
        {
            var output = new SysKefuOutput()
            {
                HelpCenterInfos = new List<KefuHelpCenter>(),
                KefuInfo = new KefuInfo(),
                UpgradeIngos = new List<UpgradeIngo>()
            };
            var helpCenterInfos = await _sysExplainDAL.GetSysExplainByType(EmSysExplainType.HelpCenter);
            if (helpCenterInfos != null && helpCenterInfos.Count > 0)
            {
                foreach (var p in helpCenterInfos)
                {
                    output.HelpCenterInfos.Add(new KefuHelpCenter()
                    {
                        RelationUrl = p.RelationUrl,
                        Title = p.Title,
                        Id = p.Id
                    });
                }
            }

            var upgradeIngos = await _sysExplainDAL.GetSysExplainByType(EmSysExplainType.UpgradeMsg);
            if (upgradeIngos != null && upgradeIngos.Count > 0)
            {
                foreach (var p in upgradeIngos)
                {
                    output.UpgradeIngos.Add(new UpgradeIngo()
                    {
                        Title = p.Title,
                        RelationUrl = p.RelationUrl,
                        Id = p.Id
                    });
                }
            }

            var myTenant = await _sysTenantDAL.GetTenant(request.LoginTenantId);
            var agent = await _sysAgentDAL.GetAgent(myTenant.AgentId);
            if (string.IsNullOrEmpty(agent.SysAgent.KefuQQ) && string.IsNullOrEmpty(agent.SysAgent.KefuPhone))
            {
                var kefuInfo = await _sysAppsettingsBLL.GetDefalutCustomerServiceInfo();
                output.KefuInfo.qq = kefuInfo.QQ;
                output.KefuInfo.Phone = kefuInfo.Phone;
            }
            else
            {
                output.KefuInfo.qq = agent.SysAgent.KefuQQ;
                output.KefuInfo.Phone = agent.SysAgent.KefuPhone;
            }
            return ResponseBase.Success(output);
        }
    }
}
