using ETMS.Entity.Common;
using ETMS.Entity.Dto.Product.Request;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.IBusiness
{
    public interface IMallGoodsBLL : IBaseBLL
    {
        Task<ResponseBase> MallGoodsAdd(MallGoodsAddRequest request);

        Task<ResponseBase> MallGoodsEdit(MallGoodsEditRequest request);

        Task<ResponseBase> MallGoodsDel(MallGoodsDelRequest request);

        Task<ResponseBase> MallGoodsChangeOrderIndex(MallGoodsChangeOrderIndexRequest request);

        Task<ResponseBase> MallGoodsGet(MallGoodsGetRequest request);

        Task<ResponseBase> MallGoodsSaveConfig(MallGoodsSaveConfigRequest request);

        Task<ResponseBase> MallGoodsGetConfig(MallGoodsGetConfigRequest request);

        Task<ResponseBase> MallGoodsGetPaging(MallGoodsGetPagingRequest request);
    }
}
