using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.Entity.Dto.Student.Request
{
    public struct EmStudentGetPagingBirthdayTimeType
    {
        /// <summary>
        /// 今天生日
        /// </summary>
        public const int Today = 0;

        /// <summary>
        /// 一周内生日
        /// </summary>
        public const int Week = 1;

        /// <summary>
        /// 一月内生日
        /// </summary>
        public const int Month = 2;
    }
}
