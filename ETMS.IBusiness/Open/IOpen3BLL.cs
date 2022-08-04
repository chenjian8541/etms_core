using ETMS.Entity.Common;
using ETMS.Entity.Dto.Open3.Request;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.IBusiness.Open
{
    public interface IOpen3BLL : IBaseBLL
    {
        Task<ResponseBase> AchievementDetailGet(AchievementDetailGetRequest request);
    }
}
