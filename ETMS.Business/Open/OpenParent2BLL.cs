using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ETMS.Business.Common;
using ETMS.Business.WxCore;
using ETMS.Entity.Common;
using ETMS.Entity.Config;
using ETMS.Entity.Database.Manage;
using ETMS.Entity.Dto.OpenParent2.Output;
using ETMS.Entity.Dto.OpenParent2.Request;
using ETMS.Entity.Enum;
using ETMS.IBusiness.Open;
using ETMS.IDataAccess.Activity;
using ETMS.IDataAccess.EtmsManage;

namespace ETMS.Business.Open
{
    public class OpenParent2BLL : MiniProgramAccessBll, IOpenParent2BLL
    {
        private readonly ISysWechatMiniPgmUserDAL _sysWechatMiniPgmUserDAL;

        private readonly ISysTenantDAL _sysTenantDAL;

        private readonly IActivityMainDAL _activityMainDAL;

        private readonly IActivityRouteDAL _activityRouteDAL;

        private readonly IActivityVisitorDAL _activityVisitorDAL;

        private readonly ISysActivityRouteItemDAL _sysActivityRouteItemDAL;
        public OpenParent2BLL(IAppConfigurtaionServices appConfigurtaionServices, ISysWechatMiniPgmUserDAL sysWechatMiniPgmUserDAL, ISysTenantDAL sysTenantDAL,
           IActivityMainDAL activityMainDAL, IActivityRouteDAL activityRouteDAL, IActivityVisitorDAL activityVisitorDAL,
           ISysActivityRouteItemDAL sysActivityRouteItemDAL)
            : base(appConfigurtaionServices)
        {
            this._sysWechatMiniPgmUserDAL = sysWechatMiniPgmUserDAL;
            this._sysTenantDAL = sysTenantDAL;
            this._activityMainDAL = activityMainDAL;
            this._activityRouteDAL = activityRouteDAL;
            this._activityVisitorDAL = activityVisitorDAL;
            this._sysActivityRouteItemDAL = sysActivityRouteItemDAL;
        }

        private void InitTenantId(int tenantId)
        {
            _activityMainDAL.InitTenantId(tenantId);
            _activityRouteDAL.InitTenantId(tenantId);
            _activityVisitorDAL.InitTenantId(tenantId);
        }

        public async Task<ResponseBase> WxMiniLogin(WxMiniLoginRequest request)
        {
            var loginResult = base.WxLogin(request.Code);
            if (loginResult == null)
            {
                return ResponseBase.CommonError("微信登录失败");
            }
            var log = await _sysWechatMiniPgmUserDAL.GetWechatMiniPgmUser(loginResult.openid);
            if (log == null)
            {
                log = new SysWechatMiniPgmUser()
                {
                    AvatarUrl = string.Empty,
                    CreateTime = DateTime.Now,
                    IsDeleted = EmIsDeleted.Normal,
                    NickName = string.Empty,
                    Phone = string.Empty,
                    Remark = string.Empty,
                    TenantId = null,
                    UpdateTime = null,
                    OpenId = loginResult.openid,
                    Unionid = loginResult.unionid
                };
                await _sysWechatMiniPgmUserDAL.AddWechatMiniPgmUser(log);
            }
            var miniPgmUserId = log.Id;
            var strSignature = ParentSignatureLib.GetOpenParent2Signature(miniPgmUserId);
            return ResponseBase.Success(new WxMiniLoginOutput()
            {
                S = strSignature,
                U = miniPgmUserId,
                OpenId = loginResult.openid,
                Unionid = loginResult.unionid
            });
        }

        public async Task<ResponseBase> WxMiniEditUserInfo(WxMiniEditUserInfoRequest request)
        {
            var log = await _sysWechatMiniPgmUserDAL.GetWechatMiniPgmUser(request.MiniPgmUserId);
            if (log == null)
            {
                LOG.Log.Error("[WxMiniEditUserInfo]用户未注册", request, this.GetType());
                return ResponseBase.CommonError("用户未注册");
            }
            log.NickName = request.NickName;
            log.AvatarUrl = request.AvatarUrl;
            log.UpdateTime = DateTime.Now;
            await _sysWechatMiniPgmUserDAL.EditWechatMiniPgmUser(log);

            return ResponseBase.Success();
        }

        public async Task<ResponseBase> WxMiniActivityRouteItemGetPaging(WxMiniActivityRouteItemGetPagingRequest request)
        {
            var pagingData = await _sysActivityRouteItemDAL.GetPagingRouteItem(request);
            var output = new List<WxMiniActivityRouteItemGetPagingOutput>();
            if (pagingData.Item1.Any())
            {
                foreach (var p in pagingData.Item1)
                {
                    output.Add(new WxMiniActivityRouteItemGetPagingOutput()
                    {
                        ActivityCoverImage = p.ActivityCoverImage,
                        ActivityEndTime = p.ActivityEndTime,
                        ActivityId = p.ActivityId,
                        ActivityName = p.ActivityName,
                        ActivityOriginalPrice = p.ActivityOriginalPrice,
                        ActivityRouteId = p.ActivityRouteId,
                        ActivityScenetype = p.ActivityScenetype,
                        ActivityStartTime = p.ActivityStartTime,
                        ActivityTenantName = p.ActivityTenantName,
                        ActivityTitle = p.ActivityTitle,
                        ActivityType = p.ActivityType,
                        ActivityTypeStyleClass = p.ActivityTypeStyleClass,
                        CreateTime = p.CreateTime,
                        EtActivityRouteItemId = p.EtActivityRouteItemId,
                        IsTeamLeader = p.IsTeamLeader,
                        MiniPgmUserId = p.MiniPgmUserId,
                        NickName = p.NickName,
                        PayFinishTime = p.PayFinishTime,
                        PaySum = p.PaySum,
                        ScenetypeStyleClass = p.ScenetypeStyleClass,
                        StudentFieldValue1 = p.StudentFieldValue1,
                        StudentFieldValue2 = p.StudentFieldValue2,
                        StudentName = p.StudentName,
                        StudentPhone = p.StudentPhone,
                        TenantId = p.TenantId
                    });
                }
            }
            return ResponseBase.Success(new ResponsePagingDataBase<WxMiniActivityRouteItemGetPagingOutput>(pagingData.Item2, output));
        }

        public async Task<ResponseBase> WxMiniActivityHomeGet(WxMiniActivityHomeGetRequest request)
        {
            return ResponseBase.Success();
        }

        public async Task<ResponseBase> WxMiniActivityHomeGet2(WxMiniActivityHomeGet2Request request)
        {
            return ResponseBase.Success();
        }

        public async Task<ResponseBase> WxMiniActivityGetSimple(WxMiniActivityGetSimpleRequest request)
        {
            return ResponseBase.Success();
        }

        public async Task<ResponseBase> WxMiniActivityDynamicBullet(WxMiniActivityDynamicBulletRequest request)
        {
            return ResponseBase.Success();
        }

        public async Task<ResponseBase> WxMiniGroupPurchaseStartCheck(WxMiniGroupPurchaseStartCheckRequest request)
        {
            return ResponseBase.Success();
        }

        public async Task<ResponseBase> WxMiniGroupPurchaseStartGo(WxMiniGroupPurchaseStartGoRequest request)
        {
            return ResponseBase.Success();
        }

        public async Task<ResponseBase> WxMiniGroupPurchaseJoinCheck(WxMiniGroupPurchaseJoinCheckRequest request)
        {
            return ResponseBase.Success();
        }

        public async Task<ResponseBase> WxMiniGroupPurchaseJoin(WxMiniGroupPurchaseJoinRequest request)
        {
            return ResponseBase.Success();
        }

        public async Task<ResponseBase> WxMiniHagglingStartCheck(WxMiniHagglingStartCheckRequest request)
        {
            return ResponseBase.Success();
        }

        public async Task<ResponseBase> WxMiniHagglingStartGo(WxMiniHagglingStartGoRequest request)
        {
            return ResponseBase.Success();
        }

        public async Task<ResponseBase> WxMiniHagglingAssistGo(WxMiniHagglingAssistGoRequest request)
        {
            return ResponseBase.Success();
        }
    }
}
