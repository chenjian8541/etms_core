using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.Enum
{
    public struct EmTempStudentNeedCheckClassStatus
    {
        /// <summary>
        /// 未记上课
        /// </summary>
        public const byte NotAttendClass = 0;

        /// <summary>
        /// 已记上课
        /// </summary>
        public const byte IsAttendClass = 1;
    }
}
