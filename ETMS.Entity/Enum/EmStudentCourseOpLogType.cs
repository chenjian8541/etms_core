using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.Enum
{
    public struct EmStudentCourseOpLogType
    {
        /// <summary>
        /// 停课
        /// </summary>
        public const int CourseStop = 0;

        /// <summary>
        /// 复课
        /// </summary>
        public const int CourseRestore = 1;

        /// <summary>
        /// 有效期设置
        /// </summary>
        public const int SetExpirationDate = 2;

        /// <summary>
        /// 课时清零
        /// </summary>
        public const int CourseClearance = 3;

        /// <summary>
        /// 修正课时
        /// </summary>
        public const int CourseChangeTimes = 4;

        /// <summary>
        /// 结课
        /// </summary>
        public const int CourseOver = 5;

        /// <summary>
        /// 超上课时标记处理
        /// </summary>
        public const int MarkExceedClassTimes = 6;

        public static string GetStudentCourseOpLogTypeDesc(int type)
        {
            switch (type)
            {
                case CourseStop:
                    return "停课";
                case CourseRestore:
                    return "复课";
                case SetExpirationDate:
                    return "有效期设置";
                case CourseClearance:
                    return "课时清零";
                case CourseChangeTimes:
                    return "修正课时";
                case CourseOver:
                    return "结课";
                case MarkExceedClassTimes:
                    return "超上课时标记处理";
            }
            return string.Empty;
        }
    }
}
