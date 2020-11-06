using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.Enum
{
    public struct EmActiveWxMessageType
    {
        /// <summary>
        /// 按班级
        /// </summary>
        public const byte Class = 0;

        /// <summary>
        /// 按学员
        /// </summary>
        public const byte Student = 1;

        /// <summary>
        /// 全体学员
        /// </summary>
        public const byte AllStudent = 3;

        public static string GetWxMessageTypeDesc(byte t)
        {
            switch (t)
            {
                case Class:
                    return "按班级";
                case Student:
                    return "按学员";
                case AllStudent:
                    return "全体学员";
            }
            return string.Empty;
        }
    }
}
