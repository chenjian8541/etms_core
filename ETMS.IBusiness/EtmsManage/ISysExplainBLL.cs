using ETMS.Entity.Common;
using ETMS.Entity.EtmsManage.Dto.Explain.Request;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.IBusiness.EtmsManage
{
    public interface ISysExplainBLL
    {
        Task<ResponseBase> SysExplainAdd(SysExplainAddRequest request);

        Task<ResponseBase> SysExplainEdit(SysExplainEditRequest request);

        Task<ResponseBase> SysExplainDel(SysExplainDelRequest request);

        Task<ResponseBase> SysExplainPaging(SysExplainPagingRequest request);
    }
}
