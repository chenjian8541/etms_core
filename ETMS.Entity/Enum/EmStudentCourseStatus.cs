using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.Enum
{
    /// <summary>
    /// 学员课程状态
    /// </summary>
    public struct EmStudentCourseStatus
    {
        /// <summary>
        /// 正常
        /// </summary>
        public const byte Normal = 0;

        /// <summary>
        /// 已停课
        /// </summary>
        public const byte StopOfClass = 1;

        /// <summary>
        /// 已结课  (过期自动结课)
        /// </summary>
        public const byte EndOfClass = 2;

        ///// <summary>
        ///// 已过期
        ///// </summary>
        //public const byte Expired = 3;


        public static string GetStudentCourseStatusDesc(byte type)
        {
            switch (type)
            {
                case EmStudentCourseStatus.Normal:
                    return "正常";
                case EmStudentCourseStatus.StopOfClass:
                    return "已停课";
                default:
                    return "已结课";
            }
        }
    }
}
