using ETMS.Entity.Common;
using ETMS.Entity.Dto.BasicData.Request;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.IBusiness
{
    public interface IGiftCategoryBLL : IBaseBLL
    {
        Task<ResponseBase> GiftCategoryAdd(GiftCategoryAddRequest request);

        Task<ResponseBase> GiftCategoryGet(GiftCategoryGetRequest request);

        Task<ResponseBase> GiftCategoryDel(GiftCategoryDelRequest request);
    }
}
