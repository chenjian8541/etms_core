using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.Dto.User.Output
{
    public class GetLoginInfoH5Output
    {
        /// <summary>
        /// 名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 手机号码
        /// </summary>
        public string Phone { get; set; }

        /// <summary>
        /// 头像地址url
        /// </summary>
        public string AvatarUrl { get; set; }

        /// <summary>
        /// 昵称 （家长端显示）
        /// </summary>
        public string NickName { get; set; }

        /// <summary>
        /// 机构名称
        /// </summary>
        public string OrgName { get; set; }
    }
}
