using ETMS.Business.SendNotice;
using ETMS.Business.WxCore;
using ETMS.Entity.ExternalService.Dto.Request;
using ETMS.IBusiness.Wechart;
using ETMS.IDataAccess;
using ETMS.IDataAccess.EtmsManage;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.Business
{
    public abstract class SendStudentNoticeBase : SendNoticeBase
    {
        protected readonly IStudentWechatDAL _studentWechatDAL;

        public SendStudentNoticeBase(IStudentWechatDAL studentWechatDAL, IComponentAccessBLL componentAccessBLL, ISysTenantDAL sysTenantDAL)
            : base(componentAccessBLL, sysTenantDAL)
        {
            this._studentWechatDAL = studentWechatDAL;
        }

        protected async Task<string> GetOpenId(bool isSendWeChat, string phone)
        {
            if (!isSendWeChat)
            {
                return string.Empty;
            }
            var wx = await _studentWechatDAL.GetStudentWechatByPhone(phone);
            return wx?.WechatOpenid;
        }
    }
}
