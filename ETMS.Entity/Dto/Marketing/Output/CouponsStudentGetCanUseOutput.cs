using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.Dto.Marketing.Output
{
    public class CouponsStudentGetCanUseOutput
    {
        /// <summary>
        /// 主键
        /// </summary>
        public long CId { get; set; }

        /// <summary>
        /// 有效期描述
        /// </summary>
        public string EffectiveTimeDesc { get; set; }

        /// <summary>
        /// 获取时间
        /// </summary>
        public DateTime GetTime { get; set; }

        /// <summary>
        /// 状态  
        /// </summary>
        public byte LogStatus { get; set; }

        public string LogStatusDesc { get; set; }

        /// <summary>
        /// 单号
        /// </summary>
        public string OrderNo { get; set; }

        public string CouponsTitle { get; set; }

        /// <summary>
        /// 类型  <see cref="ETMS.Entity.Enum.EmCouponsType"/>
        /// </summary>
        public byte CouponsType { get; set; }

        public string CouponsTypeDesc { get; set; }

        public decimal CouponsValue { get; set; }

        public string CouponsValueDesc { get; set; }

        public decimal? CouponsMinLimit { get; set; }

        /// <summary>
        /// 状态  <see cref="ETMS.Entity.Enum.EmCouponsStatus"/>
        /// </summary>
        public byte CouponsStatus { get; set; }

        public string StudentName { get; set; }

        public string StudentPhone { get; set; }

        public string MinLimitDesc { get; set; }
    }
}
