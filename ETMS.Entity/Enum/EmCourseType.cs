using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.Enum
{
    /// <summary>
    /// 课程类型
    /// </summary>
    public struct EmCourseType
    {
        /// <summary>
        /// 一对多
        /// </summary>
        public const byte OneToMany = 0;

        /// <summary>
        /// 一对一
        /// </summary>
        public const byte OneToOne = 1;

        public static string GetCourseTypeDesc(byte type)
        {
            return type == EmCourseType.OneToMany ? "一对多" : "一对一";
        }
    }
}
