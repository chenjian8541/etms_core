using ETMS.Entity.Common;
using ETMS.Entity.Dto.Activity.Request;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.IBusiness
{
    public interface IActivityBLL : IBaseBLL
    {
        Task<ResponseBase> ActivitySystemGetPaging(ActivitySystemGetPagingRequest request);

        Task<ResponseBase> ActivityMainCreateInit(ActivityMainCreateInitRequest request);

        Task<ResponseBase> ActivityMainSaveOfGroupPurchase(ActivityMainSaveOfGroupPurchaseRequest request);

        Task<ResponseBase> ActivityMainSaveAndPublishOfGroupPurchase(ActivityMainSaveOfGroupPurchaseRequest request);

        Task<ResponseBase> ActivityMainSaveOfHaggle(ActivityMainSaveOrPublishOfHaggleRequest request);

        Task<ResponseBase> ActivityMainSaveAndPublishOfHaggle(ActivityMainSaveOrPublishOfHaggleRequest request);

        Task<ResponseBase> ActivityMainEdit(ActivityMainEditRequest request);

        Task<ResponseBase> ActivityMainGetPaging(ActivityMainGetPagingRequest request);

        Task<ResponseBase> ActivityMainPublish(ActivityMainPublishRequest request);

        Task<ResponseBase> ActivityMainOff(ActivityMainOffRequest request);

        Task<ResponseBase> ActivityMainSetOn(ActivityMainSetOnRequest request);

        Task<ResponseBase> ActivityMainSetShowInParent(ActivityMainSetShowInParentRequest request);

        Task<ResponseBase> ActivityMainGetSimple(ActivityMainGetSimpleRequest request);

        Task<ResponseBase> ActivityMainDel(ActivityMainDelRequest request);

        Task<ResponseBase> ActivityMainGetForEdit(ActivityMainGetForEditRequest request);

        Task<ResponseBase> ActivityRouteGetPaging(ActivityRouteGetPagingRequest request);

        Task<ResponseBase> ActivityRouteItemGet(ActivityRouteItemGetRequest request);

        Task<ResponseBase> ActivityHaggleLogGet(ActivityHaggleLogGetRequest request);
    }
}
