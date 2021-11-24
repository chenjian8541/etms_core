using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.Enum
{
    /// <summary>
    /// 学员类型
    /// </summary>
    public struct EmClassStudentType
    {
        /// <summary>
        /// 班级学员
        /// </summary>
        public const byte ClassStudent = 0;

        /// <summary>
        /// 临时学员
        /// </summary>
        public const byte TempStudent = 1;

        /// <summary>
        /// 试听学员
        /// </summary>
        public const byte TryCalssStudent = 2;

        /// <summary>
        /// 补课学员
        /// </summary>
        public const byte MakeUpStudent = 3;

        public static string GetClassStudentTypeDesc(byte t)
        {
            switch (t)
            {
                case EmClassStudentType.ClassStudent:
                    return "班级学员";
                case EmClassStudentType.TempStudent:
                    return "临时学员";
                case EmClassStudentType.TryCalssStudent:
                    return "试听学员";
                case EmClassStudentType.MakeUpStudent:
                    return "补课学员";
            }
            return string.Empty;
        }

        public static string GetClassStudentTypeDesc2(byte t)
        {
            switch (t)
            {
                case EmClassStudentType.ClassStudent:
                    return "班";
                case EmClassStudentType.TempStudent:
                    return "临";
                case EmClassStudentType.TryCalssStudent:
                    return "试";
                case EmClassStudentType.MakeUpStudent:
                    return "补";
            }
            return string.Empty;
        }
    }
}
