using ETMS.Entity.Common;
using ETMS.Entity.EtmsManage.Common;
using ETMS.Entity.EtmsManage.Dto.SysCommon.Request;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.IBusiness.EtmsManage
{
    public interface ISysCommonBLL
    {
        Task<ResponseBase> EtmsGlobalConfigGet(EtmsGlobalConfigGetRequest request);

        Task<ResponseBase> EtmsGlobalConfigSave(EtmsGlobalConfigSaveRequest request);

        ResponseBase UploadConfigGet(AgentRequestBase request);

        Task<ResponseBase> LiveTeachingConfigGet(AgentRequestBase request);

        Task<ResponseBase> LiveTeachingConfigSave(LiveTeachingConfigSaveRequest request);
    }
}
