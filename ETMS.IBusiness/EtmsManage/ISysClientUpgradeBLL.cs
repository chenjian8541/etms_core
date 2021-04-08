using ETMS.Entity.Common;
using ETMS.Entity.EtmsManage.Dto.ClientUpgrade.Request;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.IBusiness.EtmsManage
{
    public interface ISysClientUpgradeBLL
    {
        Task<ResponseBase> SysClientUpgradeAdd(SysClientUpgradeAddRequest request);

        Task<ResponseBase> SysClientUpgradeEdit(SysClientUpgradeEditRequest request);

        Task<ResponseBase> SysClientUpgradeDel(SysClientUpgradeDelRequest request);

        Task<ResponseBase> SysClientUpgradePaging(SysClientUpgradePagingRequest request);
    }
}
