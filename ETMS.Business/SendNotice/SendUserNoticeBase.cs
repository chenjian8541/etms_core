using ETMS.Business.SendNotice;
using System;
using System.Collections.Generic;
using System.Text;
using ETMS.IDataAccess;
using ETMS.IBusiness.Wechart;
using ETMS.IDataAccess.EtmsManage;
using System.Threading.Tasks;

namespace ETMS.Business
{
    public class SendUserNoticeBase : SendNoticeBase
    {
        protected readonly IUserWechatDAL _userWechatDAL;

        public SendUserNoticeBase(IUserWechatDAL userWechatDAL, IComponentAccessBLL componentAccessBLL, ISysTenantDAL sysTenantDAL)
            : base(componentAccessBLL, sysTenantDAL)
        {
            this._userWechatDAL = userWechatDAL;
        }

        protected async Task<string> GetOpenId(bool isSendWeChat, long userId)
        {
            if (!isSendWeChat)
            {
                return string.Empty;
            }
            var wx = await _userWechatDAL.GetUserWechat(userId);
            return wx?.WechatOpenid;
        }
    }
}
