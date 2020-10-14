using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.Dto.WxAccessToken.Output
{
    public class GetWxGzhAccessTokenOutput
    {
        public string Appid { get; set; }

        public string Secret { get; set; }

        public string AccessToken { get; set; }
    }
}
