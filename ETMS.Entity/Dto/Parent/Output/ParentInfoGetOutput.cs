using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.Dto.Parent.Output
{
    public class ParentInfoGetOutput
    {
        /// <summary>
        /// 昵称
        /// </summary>
        public string Nickname { get; set; }

        /// <summary>
        /// 头像
        /// </summary>
        public string Headimgurl { get; set; }

        /// <summary>
        /// 手机号码
        /// </summary>
        public string Phone { get; set; }

        /// <summary>
        /// 是否显示退出
        /// </summary>
        public bool IsShowLoginout { get; set; }

        /// <summary>
        /// 充值账户ID
        /// </summary>
        public long? StudentAccountRechargeId { get; set; }

        /// <summary>
        /// 是否展示 "推荐有礼"
        /// </summary>
        public bool IsShowStudentRecommend { get; set; }

        public string TenantNo { get; set; }

        public string TenantName { get; set; }

        public string StuNo { get; set; }
    }
}
