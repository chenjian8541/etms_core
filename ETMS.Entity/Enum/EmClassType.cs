using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.Enum
{
    /// <summary>
    /// 班级类型
    /// </summary>
    public struct EmClassType
    {
        /// <summary>
        /// 一对多
        /// </summary>
        public const byte OneToMany = 0;

        /// <summary>
        /// 一对一
        /// </summary>
        public const byte OneToOne = 1;

        public static string GetClassTypeDesc(byte t)
        {
            switch (t)
            {
                case OneToMany:
                    return "一对多";
                case OneToOne:
                    return "一对一";
            }
            return string.Empty;
        }
    }
}
