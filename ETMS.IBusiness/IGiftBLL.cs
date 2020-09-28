using ETMS.Entity.Common;
using ETMS.Entity.Dto.Marketing.Request;
using ETMS.Entity.Dto.Parent.Request;
using ETMS.Event.DataContract;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.IBusiness
{
    public interface IGiftBLL : IBaseBLL
    {
        Task<ResponseBase> GiftAdd(GiftAddRequest request);

        Task<ResponseBase> GiftEdit(GiftEditRequest request);

        Task<ResponseBase> GiftGet(GiftGetRequest request);

        Task<ResponseBase> GiftDel(GiftDelRequest request);

        Task<ResponseBase> GiftGetPaging(GiftGetPagingRequest request);

        Task GiftExchangeEvent(GiftExchangeEvent giftExchange);

        Task<ResponseBase> GetExchangeLogPaging(GetExchangeLogPagingRequest request);

        Task<ResponseBase> GetExchangeLogDetailPaging(GetExchangeLogDetailPagingRequest request);

        Task<ResponseBase> GetExchangeLogDetail(GetExchangeLogDetailRequest request);

        Task<ResponseBase> ExchangeLogHandle(ExchangeLogHandleRequest request);

        Task<ResponseBase> GiftExchangeReception(GiftExchangeReceptionRequest request);

        Task<ResponseBase> GiftCategoryGetParent(GiftCategoryGetParentRequest request);

        Task<ResponseBase> GiftGetParent(GiftGetParentRequest request);

        Task<ResponseBase> GiftDetailGetParent(GiftDetailGetParentRequest request);

        Task<ResponseBase> GiftExchangeSelfHelp(GiftExchangeRequest request);

        Task<ResponseBase> GetExchangeLogDetailParentPaging(StudentGiftExchangeLogGetReqeust request);
    }
}
