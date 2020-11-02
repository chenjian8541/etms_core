using ETMS.Entity.Common;
using ETMS.Entity.Dto.Wx.Request;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.IBusiness.Wechart
{
    public interface IWxAccessBLL:IBaseBLL
    {
        Task<ResponseBase> WxConfigBascGet(WxConfigBascGetRequest request);
    }
}
