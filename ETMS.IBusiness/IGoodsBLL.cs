using ETMS.Entity.Common;
using ETMS.Entity.Dto.Product.Request;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.IBusiness
{
    public interface IGoodsBLL : IBaseBLL
    {
        Task<ResponseBase> GoodsAdd(GoodsAddRequest request);

        Task<ResponseBase> GoodsEdit(GoodsEditRequest request);

        Task<ResponseBase> GoodsGet(GoodsGetRequest request);

        Task<ResponseBase> GoodsDel(GoodsDelRequest request);

        Task<ResponseBase> GoodsStatusChange(GoodsStatusChangeRequest request);

        Task<ResponseBase> GoodsGetPaging(GoodsGetPagingRequest request);

        Task<ResponseBase> GoodsInventoryLogAdd(GoodsInventoryLogAddRequest request);

        Task<ResponseBase> GoodsInventoryLogGetPaging(GoodsInventoryLogGetPagingRequest request);
    }
}
