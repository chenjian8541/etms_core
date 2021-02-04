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
        /// 结课
        /// </summary>
        public const byte StudentCourseOver = 3;

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

        /// <summary>
        /// 修改点名记录
        /// </summary>
        public const byte ModifyStudentClassRecordAdd = 8;

        /// <summary>
        /// 修改点名记录
        /// </summary>
        public const byte ModifyStudentClassRecordDe = 9;

        /// <summary>
        /// 学员考勤记上课
        /// </summary>
        public const byte StudentCheckIn = 10;

        /// <summary>
        /// 撤销学员考勤记上课
        /// </summary>
        public const byte StudentCheckInRevoke = 11;

        /// <summary>
        /// 课时清零
        /// </summary>
        public const byte CourseClearance = 12;

        public static string GetStudentCourseConsumeSourceType(int b)
        {
            switch (b)
            {
                case ClassCheckSign:
                    return "点名";
                case AutoConsumeDay:
                    return "按天自动消耗";
                case ModifyStudentClassRecordAdd:
                    return "修改点名记录";
                case ModifyStudentClassRecordDe:
                    return "修改点名记录";
                case StudentCourseOver:
                    return "结课";
                case MakeUpExceedClassTimes:
                    return "补扣超上课时";
                case UndoStudentClassRecord:
                    return "撤销点名记录";
                case SetExpirationDate:
                    return "设置起止时间";
                case CorrectStudentCourse:
                    return "修正剩余课时";
                case StudentCheckIn:
                    return "考勤记上课";
                case StudentCheckInRevoke:
                    return "撤销考勤记上课";
                case CourseClearance:
                    return "课时清零";
            }
            return string.Empty;
        }

        public static string GetStudentCourseConsumeSourceTypeTag(int b)
        {
            switch (b)
            {
                case ClassCheckSign:
                case AutoConsumeDay:
                case ModifyStudentClassRecordDe:
                case StudentCourseOver:
                case MakeUpExceedClassTimes:
                case SetExpirationDate:
                case CorrectStudentCourse:
                case StudentCheckIn:
                case CourseClearance:
                    return "消耗";
                case UndoStudentClassRecord:
                case ModifyStudentClassRecordAdd:
                case StudentCheckInRevoke:
                    return "返还";
            }
            return string.Empty;
        }
    }
}
