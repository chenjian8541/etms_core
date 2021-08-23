using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.Entity.Enum
{
    public struct EmTeacherSalaryComputeType
    {
        public const byte Class = 0;

        public const byte Course = 1;

        public const byte Global = 2;

        public static string GetTeacherSalaryComputeTypeDesc(byte t)
        {
            switch (t)
            {
                case Class:
                    return "按班级设置";
                case Course:
                    return "按课程设置";
                case Global:
                    return "统一设置";
            }
            return string.Empty;
        }

        public static string GetComputeTypeDescTag(byte t)
        {
            switch (t)
            {
                case Class:
                    return "班级";
                case Course:
                    return "课程";
                case Global:
                    return "统一设置";
            }
            return string.Empty;
        }
    }
}
