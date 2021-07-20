using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.Enum
{
    public struct EmStudentCheckOnLogCheckForm
    {
        /// <summary>
        /// 刷卡考勤
        /// </summary>
        public const byte Card = 0;

        /// <summary>
        /// 人脸考勤
        /// </summary>
        public const byte Face = 1;

        /// <summary>
        /// 手动考勤 （规则按刷卡考勤来）
        /// </summary>
        public const byte ManualCheck = 2;

        /// <summary>
        /// 考勤补签
        /// </summary>
        public const byte TeacherManual = 3;

        public static string GetStudentCheckOnLogCheckFormDesc(byte b)
        {
            switch (b)
            {
                case Card:
                    return "刷卡考勤";
                case Face:
                    return "人脸考勤";
                case ManualCheck:
                    return "手动考勤";
                case TeacherManual:
                    return "考勤补签";
            }
            return string.Empty;
        }
    }
}
