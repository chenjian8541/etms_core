using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ETMS.Entity.Config;
using ETMS.Utility;
using Senparc.Weixin;
using Senparc.Weixin.WxOpen.AdvancedAPIs.Sns;
using Senparc.Weixin.WxOpen.AdvancedAPIs.WxApp;
using Senparc.Weixin.WxOpen.Entities;
using Senparc.Weixin.WxOpen.Helpers;

namespace ETMS.Business.WxCore
{
    public abstract class MiniProgramAccessBll
    {
        protected readonly IAppConfigurtaionServices _appConfigurtaionServices;

        protected readonly MiniProgramConfig _miniProgramConfig;

        public MiniProgramAccessBll(IAppConfigurtaionServices appConfigurtaionServices)
        {
            this._appConfigurtaionServices = appConfigurtaionServices;
            this._miniProgramConfig = appConfigurtaionServices.AppSettings.SenparcConfig.SenparcWeixinSetting.MiniProgramConfig;
        }

        public JsCode2JsonResult WxLogin(string code)
        {
            var jsonResult = SnsApi.JsCode2Json(_miniProgramConfig.WxOpenAppId, _miniProgramConfig.WxOpenAppSecret, code);
            if (jsonResult.errcode == ReturnCode.请求成功)
            {
                return jsonResult;
            }
            return null;
        }

        public async Task<string> GenerateQrCode(int tenantId, string imgTag, string imgKey, string page, string scene)
        {
            using (var ms = new MemoryStream())
            {
                //var lineColor = new LineColor(221, 51, 238);
                var result = await WxAppApi.GetWxaCodeUnlimitAsync(_miniProgramConfig.WxOpenAppId, ms, scene, page);
                ms.Position = 0;
                return AliyunOssUtil.PutObject(tenantId, imgKey, imgTag, ms);
            }
        }

        public DecodedPhoneNumber WxDecodedPhoneNumber(string sessionKey, string encryptedData, string iv)
        {
            var result = EncryptHelper.DecryptPhoneNumberBySessionKey(sessionKey, encryptedData, iv);
            return result;
        }
    }
}
