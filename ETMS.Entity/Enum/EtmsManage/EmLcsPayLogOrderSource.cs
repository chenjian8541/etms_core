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

        /// <summary>
        /// 小程序
        /// </summary>
        public const int MiniProgram = 2;

        public static string GetOrderSourceDesc(int t)
        {
            switch (t)
            {
                case PC:
                    return "PC端";
                case WeChat:
                    return "微信端";
                case MiniProgram:
                    return "小程序";
            }
            return string.Empty;
        }
    }
}
