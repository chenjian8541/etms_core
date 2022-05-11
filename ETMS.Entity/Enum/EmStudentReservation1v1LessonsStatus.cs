using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.Entity.Enum
{
    public struct EmStudentReservation1v1LessonsStatus
    {
        /// <summary>
        /// 正常
        /// </summary>
        public const int Normal = 1;

        /// <summary>
        /// 已预约
        /// </summary>
        public const int IsReservation = 2;

        /// <summary>
        /// 已点名
        /// </summary>
        public const int IsRollcall = 3;

        /// <summary>
        /// 无效(已过时间,无法预约)
        /// </summary>
        public const int Invalid = 4;

        /// <summary>
        /// 时间被占用
        /// </summary>
        public const int Occupy = 5;
    }
}
