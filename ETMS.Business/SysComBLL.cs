using ETMS.Entity.Common;
using ETMS.Entity.Dto.SysCom.Output;
using ETMS.Entity.Dto.SysCom.Request;
using ETMS.IBusiness;
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

        public SysComBLL(ISysUpgradeMsgDAL sysUpgradeMsgDAL)
        {
            this._sysUpgradeMsgDAL = sysUpgradeMsgDAL;
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
    }
}
