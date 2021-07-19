using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.Entity.Enum
{
    public struct EmAttendanceRelationClassTimesType
    {
        /// <summary>
        /// 关联课次
        /// </summary>
        public const byte RelationClassTimes = 0;

        /// <summary>
        /// 直接扣课时
        /// </summary>
        public const byte GoDeStudentCourse = 1;
    }
}
