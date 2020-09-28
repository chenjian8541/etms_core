using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.Enum
{
    /// <summary>
    /// 学员类型
    /// </summary>
    public struct EmStudentType
    {
        /// <summary>
        /// 潜在学员
        /// </summary>
        public const byte HiddenStudent = 0;

        /// <summary>
        /// 在读学员
        /// </summary>
        public const byte ReadingStudent = 1;

        /// <summary>
        /// 历史学员（毕业）
        /// </summary>
        public const byte HistoryStudent = 2;

        public static List<byte> GetAllStudentType()
        {
            var allStudentType = new List<byte>();
            allStudentType.Add(HiddenStudent);
            allStudentType.Add(ReadingStudent);
            allStudentType.Add(HistoryStudent);
            return allStudentType;
        }

        public static string GetStudentTypeDesc(byte type)
        {
            switch (type)
            {
                case EmStudentType.HiddenStudent:
                    return "潜在学员";
                case EmStudentType.ReadingStudent:
                    return "在读学员";
                default:
                    return "历史学员";
            }
        }
    }
}
