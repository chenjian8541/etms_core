using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.Enum
{
    public struct EmLimitStudentNumsType
    {
        /// <summary>
        /// 可超额
        /// </summary>
        public const byte CanOverflow = 0;

        /// <summary>
        /// 不可超额
        /// </summary>
        public const byte NotOverflow = 1;

        public static string GetLimitStudentNumsDesc(int studentNums, int? limitStudentNums, byte b)
        {
            if (limitStudentNums == null)
            {
                return $"{studentNums}/未设置";
            }
            if (b == CanOverflow)
            {
                return $"{studentNums}/{limitStudentNums.Value}(可超额)";
            }
            return $"{studentNums}/{limitStudentNums.Value}";
        }

        public static string GetLimitStudentNumsDesc2(int studentNums, int? limitStudentNums, byte b)
        {
            if (limitStudentNums == null)
            {
                return $"{studentNums}/-";
            }
            if (b == CanOverflow)
            {
                return $"{studentNums}/{limitStudentNums.Value}";
            }
            return $"{studentNums}/{limitStudentNums.Value}";
        }
    }
}
