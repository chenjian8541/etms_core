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

        Task<ResponseBase> SysUpgradeSetRead(SysUpgradeSetReadRequest request);

        Task<ResponseBase> SysKefu(SysKefuRequest request);

        ResponseBase UploadConfigGet(RequestBase request);
    }
}
