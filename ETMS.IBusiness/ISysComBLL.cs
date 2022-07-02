using ETMS.Entity.Common;
using ETMS.Entity.Dto.SysCom.Request;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.IBusiness
{
    public interface ISysComBLL : IBaseBLL
    {
        Task<ResponseBase> SysUpgradeGet(SysUpgradeGetRequest request);

        Task<ResponseBase> SysNotifyGet(RequestBase request);

        Task<ResponseBase> SysUpgradeSetRead(SysUpgradeSetReadRequest request);

        Task<ResponseBase> SysKefu(SysKefuRequest request);

        Task<ResponseBase> UploadConfigGet(RequestBase request);

        Task<ResponseBase> UploadConfigGetOpenLink(UploadConfigGetOpenLinkRequest request);

        Task<ResponseBase> ClientUpgradeGet(ClientUpgradeGetRequest request);

        Task<ResponseBase> SysBulletinGet(RequestBase request);

        Task<ResponseBase> SysBulletinSetRead(SysBulletinSetReadRequest request);
    }
}
