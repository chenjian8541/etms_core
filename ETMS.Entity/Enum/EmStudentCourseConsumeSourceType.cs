using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.Enum
{
    /// <summary>
    /// 学员课程消耗数据来源
    /// </summary>
    public struct EmStudentCourseConsumeSourceType
    {
        /// <summary>
        /// 点名
        /// </summary>
        public const byte ClassCheckSign = 0;

        /// <summary>
        /// 按天自动消耗
        /// </summary>
        public const byte AutoConsumeDay = 1;

        /// <summary>
        /// 修改上课记录
        /// </summary>
        public const byte ModifyStudentClassRecord = 2;

        /// <summary>
        /// 结课
        /// </summary>
        public const byte StopStudentCourse = 3;

        /// <summary>
        /// 补扣超上课时
        /// </summary>
        public const byte MakeUpExceedClassTimes = 4;

        /// <summary>
        /// 撤销点名记录
        /// </summary>
        public const byte UndoStudentClassRecord = 5;

        /// <summary>
        /// 设置起止时间
        /// </summary>
        public const byte SetExpirationDate = 6;

        /// <summary>
        /// 修正剩余课时
        /// </summary>
        public const byte CorrectStudentCourse = 7;

        public static string GetStudentCourseConsumeSourceType(int b)
        {
            switch (b)
            {
                case ClassCheckSign:
                    return "点名";
                case AutoConsumeDay:
                    return "按天自动消耗";
                case ModifyStudentClassRecord:
                    return "修改上课记录";
                case StopStudentCourse:
                    return "结课";
                case MakeUpExceedClassTimes:
                    return "补扣超上课时";
                case UndoStudentClassRecord:
                    return "撤销点名记录";
                case SetExpirationDate:
                    return "设置起止时间";
                case CorrectStudentCourse:
                    return "修正剩余课时";
            }
            return string.Empty;
        }

        public static string GetStudentCourseConsumeSourceTypeTag(int b)
        {
            switch (b)
            {
                case ClassCheckSign:
                case AutoConsumeDay:
                case ModifyStudentClassRecord:
                case StopStudentCourse:
                case MakeUpExceedClassTimes:
                case SetExpirationDate:
                case CorrectStudentCourse:
                    return "消耗";
                case UndoStudentClassRecord:
                    return "返还";
            }
            return string.Empty;
        }
    }
}
