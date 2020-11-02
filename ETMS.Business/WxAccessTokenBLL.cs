using ETMS.Entity.CacheBucket;
using ETMS.Entity.Config;
using ETMS.Entity.Dto.WxAccessToken.Output;
using ETMS.IBusiness;
using ETMS.IDataAccess;
using ETMS.WxApi;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Business
{
    public class WxAccessTokenBLL : IWxAccessTokenBLL
    {
        //private readonly ITempDataCacheDAL _tempDataCacheDAL;

        //private readonly IAppConfigurtaionServices _appConfigurtaionServices;

        //public WxAccessTokenBLL(ITempDataCacheDAL tempDataCacheDAL, IAppConfigurtaionServices appConfigurtaionServices)
        //{
        //    this._tempDataCacheDAL = tempDataCacheDAL;
        //    this._appConfigurtaionServices = appConfigurtaionServices;
        //}

        public void InitTenantId(int tenantId)
        {
        }

        //public GetWxGzhAccessTokenOutput GetWxGzhAccessToken()
        //{
        //    var wxConfig = this._appConfigurtaionServices.AppSettings.WxConfig;
        //    var log = _tempDataCacheDAL.GetWxGzhAccessTokenBucket(wxConfig.Appid);
        //    if (log != null && !string.IsNullOrEmpty(log.AccessToken) && log.AppId == wxConfig.Appid)
        //    {
        //        if (log.ExTime < DateTime.Now)
        //        {
        //            return new GetWxGzhAccessTokenOutput()
        //            {
        //                AccessToken = log.AccessToken,
        //                Appid = wxConfig.Appid,
        //                Secret = wxConfig.Secret
        //            };
        //        }
        //        else
        //        {
        //            LOG.Log.Info($"[GetWxGzhAccessToken]WxGzhAccessToken已过期,{JsonConvert.SerializeObject(log)}", this.GetType());
        //        }
        //    }
        //    var getAccessTokenResult = BaseServices.GetAccessToken(wxConfig.Appid, wxConfig.Secret);
        //    if (getAccessTokenResult.ErrCode == 0)
        //    {
        //        var extime = DateTime.Now.AddSeconds(getAccessTokenResult.expires_in / 2);
        //        var newBucket = new WxGzhAccessTokenBucket()
        //        {
        //            AccessToken = getAccessTokenResult.access_token,
        //            AppId = wxConfig.Appid,
        //            ExpiresIn = getAccessTokenResult.expires_in,
        //            ExTime = extime
        //        };
        //        _tempDataCacheDAL.SetWxGzhAccessTokenBucket(newBucket, wxConfig.Appid);
        //        return new GetWxGzhAccessTokenOutput()
        //        {
        //            AccessToken = getAccessTokenResult.access_token,
        //            Appid = wxConfig.Appid,
        //            Secret = wxConfig.Secret
        //        };
        //    }
        //    else
        //    {
        //        return new GetWxGzhAccessTokenOutput() { };
        //    }
        //}
    }
}
