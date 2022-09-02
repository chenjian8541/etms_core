using ETMS.Entity.Enum.EtmsManage;
using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.Enum
{
    public struct EmUserOperationLogClientType
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
        /// 安卓端和IOS
        /// </summary>
        public const int Android = 2;

        /// <summary>
        /// PC端安装程序
        /// </summary>
        public const int PcInstall = 3;

        /// <summary>
        /// 家长端 
        /// 注：只是个标记
        /// </summary>
        public const int WxParent = 99;

        public static string GetClientTypeDesc(int type)
        {
            switch (type)
            {
                case 0:
                    return "PC端";
                case 1:
                    return "微信公众号";
                case 2:
                    return "手机APP";
                case 3:
                    return "PC端考勤助手";
            }
            return string.Empty;
        }

        public static int GetClientUpgradeClientType(int logClientType)
        {
            if (logClientType == 2)
            {
                return EmSysClientUpgradeClientType.Android;
            }
            return -1;
        }
    }
}
