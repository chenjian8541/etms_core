using ETMS.Entity.Common;
using ETMS.Entity.Dto.Parent3.Request;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.IBusiness.Parent
{
    public interface IParentData4BLL : IBaseBLL
    {
        Task<ResponseBase> ParentBuyMallGoodsPrepay(ParentBuyMallGoodsPrepayRequest request);

        Task<ResponseBase> ParentBuyMallGoodsSubmit(ParentBuyMallGoodsSubmitRequest request);
    }
}
