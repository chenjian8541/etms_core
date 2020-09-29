using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.Dto.Parent.Output
{
    public class StudentCouponsExpiredGetOutput
    {
        /// <summary>
        /// 主键
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// 有效期描述
        /// </summary>
        public string EffectiveTimeDesc { get; set; }

        public string CouponsTitle { get; set; }

        /// <summary>
        /// 类型  <see cref="ETMS.Entity.Enum.EmCouponsType"/>
        /// </summary>
        public byte CouponsType { get; set; }

        public string CouponsTypeDesc { get; set; }

        public decimal CouponsValue { get; set; }

        public string CouponsValueDesc { get; set; }

        public decimal? CouponsMinLimit { get; set; }

        public string StudentName { get; set; }

        public string MinLimitDesc { get; set; }
    }
}
