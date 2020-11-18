using ETMS.Entity.Common;
using ETMS.Entity.Dto.HisData.Request;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.IBusiness.SysOp
{
    public interface ISysDataClearBLL : IBaseBLL
    {
        Task<ResponseBase> ClearData(ClearDataRequest request);
    }
}
