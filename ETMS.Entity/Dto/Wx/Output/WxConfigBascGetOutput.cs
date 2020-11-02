using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.Dto.Wx.Output
{
    public class WxConfigBascGetOutput
    {
        /// <summary>
        /// 昵称
        /// </summary>
        public string NickName { get; set; }

        /// <summary>
        /// 头像
        /// </summary>
        public string HeadImg { get; set; }

        /// <summary>
        /// 公众号类型  <see cref="ETMS.Entity.Enum.EmWechartAuthServiceTypeInfo"/>
        /// </summary>
        public string ServiceTypeInfo { get; set; }

        /// <summary>
        /// 公众号认证类型  <see cref="ETMS.Entity.Enum.EmWechartAuthVerifyTypeInfo"/>
        /// </summary>
        public string VerifyTypeInfo { get; set; }

        /// <summary>
        /// 二维码图片的 URL
        /// </summary>
        public string QrcodeUrl { get; set; }

        /// <summary>
        /// 主体名称
        /// </summary>
        public string PrincipalName { get; set; }

        public string ParentLoginUrl { get; set; }

        public string TeacherLoginUrl { get; set; }
    }
}
