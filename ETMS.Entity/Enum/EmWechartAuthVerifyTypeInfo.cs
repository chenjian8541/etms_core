using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.Enum
{
    /// <summary>
    /// 公众号认证类型
    /// </summary>
    public struct EmWechartAuthVerifyTypeInfo
    {
        /// <summary>
        /// 未认证
        /// </summary>
        public const string VerifyType1Less = "-1";

        /// <summary>
        /// 微信认证
        /// </summary>
        public const string VerifyType0 = "0";

        /// <summary>
        /// 新浪微博认证
        /// </summary>
        public const string VerifyType1 = "1";

        /// <summary>
        /// 腾讯微博认证
        /// </summary>
        public const string VerifyType2 = "2";

        /// <summary>
        /// 已资质认证通过但还未通过名称认证
        /// </summary>
        public const string VerifyType3 = "3";

        /// <summary>
        /// 已资质认证通过、还未通过名称认证，但通过了新浪微博认证
        /// </summary>
        public const string VerifyType4 = "4";

        /// <summary>
        /// 已资质认证通过、还未通过名称认证，但通过了腾讯微博认证
        /// </summary>
        public const string VerifyType5 = "5";
    }
}
