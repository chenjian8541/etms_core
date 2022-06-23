using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.Entity.Enum.EtmsManage
{
    public struct EmSysTryApplyLogClientType
    {
        /// <summary>
        /// PC
        /// </summary>
        public const int PC = 0;

        /// <summary>
        /// 微信
        /// </summary>
        public const int WeChat = 1;

        /// <summary>
        /// 安卓端
        /// </summary>
        public const int Android = 2;

        /// <summary>
        /// 推广
        /// </summary>
        public const int Promote30 = 30;

        /// <summary>
        /// 家长端 
        /// 注：只是个标记
        /// </summary>
        public const int WxParent = 99;
    }
}
