using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.Enum
{
    public struct EmMicroWebConfigType
    {
        /// <summary>
        /// banner 设置
        /// </summary>
        public const int BannerSet = 0;

        /// <summary>
        /// 系统默认栏目配置
        /// </summary>
        public const int MicroWebDefaultColumnSet = 1;

        /// <summary>
        /// 地址配置
        /// </summary>
        public const int TenantAddressSet = 2;

        /// <summary>
        /// 缓存整个首页
        /// </summary>
        public const int HomeWeb = 3;
    }
}
