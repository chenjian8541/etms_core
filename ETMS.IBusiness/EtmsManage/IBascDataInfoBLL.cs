using ETMS.Entity.Common;
using ETMS.Entity.Dto.BasicData.Request;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.IBusiness.EtmsManage
{
    public interface IBascDataInfoBLL
    {
        Task<ResponseBase> GetRegions(GetRegionsRequrst requrst);

        Task<ResponseBase> GetBanks(GetBanksRequrst requrst);

        Task<ResponseBase> GetIndustry(GetIndustryRequrst requrst);
    }
}
