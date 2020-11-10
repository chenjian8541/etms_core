using ETMS.Entity.Common;
using ETMS.Entity.EtmsManage.Dto.Agent.Request;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.IBusiness.EtmsManage
{
    public interface ISysUpgradeMsgBLL
    {
        Task<ResponseBase> SysUpgradeMsgAdd(SysUpgradeMsgAddRequest request);

        Task<ResponseBase> SysUpgradeMsgPaging(SysUpgradeMsgPagingRequest request);
    }
}
