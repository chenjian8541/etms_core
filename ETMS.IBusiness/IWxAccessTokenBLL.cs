using ETMS.Entity.Dto.WxAccessToken.Output;
using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.IBusiness
{
    public interface IWxAccessTokenBLL : IBaseBLL
    {
        GetWxGzhAccessTokenOutput GetWxGzhAccessToken();
    }
}
