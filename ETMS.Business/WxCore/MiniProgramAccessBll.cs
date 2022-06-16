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

namespace ETMS.Business.WxCore
{
    public abstract class MiniProgramAccessBll
    {
        protected readonly MiniProgramConfig _miniProgramConfig;

        public MiniProgramAccessBll(IAppConfigurtaionServices appConfigurtaionServices)
        {
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
                var lineColor = new LineColor(221, 51, 238);
                var result = await WxAppApi.GetWxaCodeUnlimitAsync(_miniProgramConfig.WxOpenAppId, ms, scene, page, lineColor: lineColor);
                ms.Position = 0;
                return AliyunOssUtil.PutObject(tenantId, imgKey, imgTag, ms);
            }
        }
    }
}
