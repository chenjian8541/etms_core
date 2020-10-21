using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.Dto.Parent.Output
{
    public struct EmOverdueStatusOutput
    {
        /// <summary>
        /// 正常
        /// </summary>
        public const byte Normal = 0;

        /// <summary>
        /// 逾期
        /// </summary>
        public const byte Overdue = 1;

        public static Tuple<byte, string> GetOverdueStatusOutput(DateTime? exDate, DateTime submitOt)
        {
            if (exDate == null || exDate.Value > submitOt)
            {
                return Tuple.Create(Normal, "正常");
            }
            return Tuple.Create(Overdue, "逾期未交");
        }
    }
}
