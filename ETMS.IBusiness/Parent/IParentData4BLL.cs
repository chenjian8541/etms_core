using ETMS.Entity.Common;
using ETMS.Entity.Dto.Parent3.Request;
using ETMS.Entity.Dto.Parent4.Request;
using ETMS.Event.DataContract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.IBusiness.Parent
{
    public interface IParentData4BLL : IBaseBLL
    {
        Task<ResponseBase> ClassCanChooseGet(ClassCanChooseGetRequest request);

        Task<ResponseBase> ParentBuyMallGoodsPrepay(ParentBuyMallGoodsPrepayRequest request);

        Task<ResponseBase> ParentBuyMallGoodsPayInit(ParentBuyMallGoodsSubmitRequest request);

        Task<ResponseBase> ParentBuyMallGoodsSubmit(ParentBuyMallGoodsSubmitRequest request);

        Task ParentBuyMallGoodsPaySuccessConsumerEvent(ParentBuyMallGoodsPaySuccessEvent request);

        Task<ResponseBase> MallGoodsGetPaging(MallGoodsGetPagingRequest request);

        Task<ResponseBase> TeacherEvaluateGetPaging(TeacherEvaluateGetPagingRequest request);
    }
}
