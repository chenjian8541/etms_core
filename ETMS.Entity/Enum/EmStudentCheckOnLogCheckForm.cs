using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.Enum
{
    public struct EmStudentCheckOnLogCheckForm
    {
        /// <summary>
        /// 磁卡考勤
        /// </summary>
        public const byte Card = 0;

        /// <summary>
        /// 人脸考勤
        /// </summary>
        public const byte Face = 1;

        /// <summary>
        /// 老师补签
        /// </summary>
        public const byte TeacherManual = 2;

        public static string GetStudentCheckOnLogCheckFormDesc(byte b)
        {
            switch (b)
            {
                case Card:
                    return "磁卡考勤";
                case Face:
                    return "人脸考勤";
                case TeacherManual:
                    return "老师补签";
            }
            return string.Empty;
        }
    }
}
