using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.Entity.Enum.EtmsManage
{
    public struct EmLcsPayLogOrderSource
    {
        /// <summary>
        /// PC端
        /// </summary>
        public const int PC = 0;

        /// <summary>
        /// 微信端
        /// </summary>
        public const int WeChat = 1;

        public static string GetOrderSourceDesc(int t)
        {
            return t == PC ? "PC端" : "微信端";
        }
    }
}
