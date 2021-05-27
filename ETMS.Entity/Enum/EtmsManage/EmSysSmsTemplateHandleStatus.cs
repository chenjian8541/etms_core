using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.Enum.EtmsManage
{
    public struct EmSysSmsTemplateHandleStatus
    {
        /// <summary>
        /// 审核中
        /// </summary>
        public const byte Unreviewed = 0;

        /// <summary>
        /// 未通过
        /// </summary>
        public const byte NotPass = 1;

        /// <summary>
        /// 已通过
        /// </summary>
        public const byte Pass = 2;

        public static string GetEmSysSmsTemplateHandleStatusDesc(byte t)
        {
            switch (t)
            {
                case Unreviewed:
                    return "审核中";
                case NotPass:
                    return "未通过";
                case Pass:
                    return "已通过";
            }
            return string.Empty;
        }
    }
}
