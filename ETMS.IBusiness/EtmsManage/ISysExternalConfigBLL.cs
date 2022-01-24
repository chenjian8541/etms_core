using ETMS.Entity.Common;
using ETMS.Entity.EtmsManage.Common;
using ETMS.Entity.EtmsManage.Dto.Agent3.Request;
using System;
using System.Threading.Tasks;

namespace ETMS.IBusiness.EtmsManage
{
    public interface ISysExternalConfigBLL
    {
        Task<ResponseBase> SysExternalConfigAdd(SysExternalConfigAddRequest request);

        Task<ResponseBase> SysExternalConfigEdit(SysExternalConfigEditRequest request);

        Task<ResponseBase> SysExternalConfigDel(SysExternalConfigDelRequest request);

        [Obsolete("使用SysExternalConfigGetList")]
        Task<ResponseBase> SysExternalConfigPaging(SysExternalConfigPagingRequest request);

        Task<ResponseBase> SysExternalConfigGetList(AgentRequestBase request);
    }
}
