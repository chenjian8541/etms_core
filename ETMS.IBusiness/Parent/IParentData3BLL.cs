using ETMS.Entity.Common;
using ETMS.Entity.Dto.Parent.Request;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.IBusiness
{
    public interface IParentData3BLL : IBaseBLL
    {
        Task<ResponseBase> WxMessageDetailPaging(WxMessageDetailPagingRequest request);

        Task<ResponseBase> WxMessageDetailGet(WxMessageDetailGetRequest request);

        Task<ResponseBase> WxMessageDetailSetRead(WxMessageDetailSetReadRequest request);

        Task<ResponseBase> WxMessageDetailSetConfirm(WxMessageDetailSetConfirmRequest request);

        Task<ResponseBase> WxMessageGetUnreadCount(WxMessageGetUnreadCountRequest request);

        Task<ResponseBase> TryCalssApply(TryCalssApplyRequest request);

        Task<ResponseBase> CheckOnLogGet(CheckOnLogGetRequest request);
    }
}
