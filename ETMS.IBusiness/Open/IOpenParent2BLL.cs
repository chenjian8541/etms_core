using ETMS.Entity.Common;
using ETMS.Entity.Dto.OpenParent2.Request;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.IBusiness.Open
{
    public interface IOpenParent2BLL
    {
        Task<ResponseBase> WxMiniLogin(WxMiniLoginRequest request);

        Task<ResponseBase> WxMiniEditUserInfo(WxMiniEditUserInfoRequest request);

        Task<ResponseBase> WxMiniActivityRouteItemGetPaging(WxMiniActivityRouteItemGetPagingRequest request);

        Task<ResponseBase> WxMiniActivityHomeGet(WxMiniActivityHomeGetRequest request);

        Task<ResponseBase> WxMiniActivityHomeGet2(WxMiniActivityHomeGet2Request request);

        Task<ResponseBase> WxMiniActivityGetSimple(WxMiniActivityGetSimpleRequest request);

        Task<ResponseBase> WxMiniActivityDynamicBullet(WxMiniActivityDynamicBulletRequest request);

        Task<ResponseBase> WxMiniGroupPurchaseStartCheck(WxMiniGroupPurchaseStartCheckRequest request);

        Task<ResponseBase> WxMiniGroupPurchaseStartGo(WxMiniGroupPurchaseStartGoRequest request);

        Task<ResponseBase> WxMiniGroupPurchaseJoinCheck(WxMiniGroupPurchaseJoinCheckRequest request);

        Task<ResponseBase> WxMiniGroupPurchaseJoin(WxMiniGroupPurchaseJoinRequest request);

        Task<ResponseBase> WxMiniHagglingStartCheck(WxMiniHagglingStartCheckRequest request);

        Task<ResponseBase> WxMiniHagglingStartGo(WxMiniHagglingStartGoRequest request);

        Task<ResponseBase> WxMiniHagglingAssistGo(WxMiniHagglingAssistGoRequest request);
    }
}
