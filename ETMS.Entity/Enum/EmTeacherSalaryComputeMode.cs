using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.Entity.Enum
{
    public struct EmTeacherSalaryComputeMode
    {
        /// <summary>
        /// 按授课课时
        /// </summary>
        public const byte TeacherClassTimes = 0;

        /// <summary>
        /// 按到课人次
        /// </summary>
        public const byte StudentAttendeesCount = 1;

        /// <summary>
        /// 按课消金额
        /// </summary>
        public const byte StudentDeSum = 2;

        /// <summary>
        /// 按学员课时
        /// </summary>
        public const byte StudentClassTimes = 3;

        public static string GetTeacherSalaryComputeModeDesc(byte t)
        {
            switch (t)
            {
                case TeacherClassTimes:
                    return "按授课课时";
                case StudentAttendeesCount:
                    return "按到课人次";
                case StudentDeSum:
                    return "按课消金额";
                case StudentClassTimes:
                    return "按学员课时";
            }
            return string.Empty;
        }

        public static string GetTeacherSalaryComputeModeDesc2(byte t)
        {
            switch (t)
            {
                case TeacherClassTimes:
                    return "授课课时";
                case StudentAttendeesCount:
                    return "到课人次";
                case StudentDeSum:
                    return "课消金额";
                case StudentClassTimes:
                    return "学员课时";
            }
            return string.Empty;
        }

        public static string GetModelUnitDesc(byte t)
        {
            switch (t)
            {
                case EmTeacherSalaryComputeMode.TeacherClassTimes:
                    return "元/课时";
                case EmTeacherSalaryComputeMode.StudentAttendeesCount:
                    return "元/人次";
                case EmTeacherSalaryComputeMode.StudentDeSum:
                    return "%课消比";
                case EmTeacherSalaryComputeMode.StudentClassTimes:
                    return "元/课时";
            }
            return string.Empty;
        }
    }
}
