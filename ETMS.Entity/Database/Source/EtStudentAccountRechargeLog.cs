using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace ETMS.Entity.Database.Source
{
    /// <summary>
    /// 学员储值账户变动记录
    /// </summary>
    [Table("EtStudentAccountRechargeLog")]
    public class EtStudentAccountRechargeLog : Entity<long>
    {
        public long StudentAccountRechargeId { get; set; }

        public long? RelatedOrderId { get; set; }

        public string CgNo { get; set; }

        /// <summary>
        /// <see cref="ETMS.Entity.Enum.EmStudentAccountRechargeLogType"/>
        /// </summary>
        public int Type { get; set; }

        public decimal CgBalanceReal { get; set; }

        public decimal CgBalanceGive { get; set; }

        public decimal CgServiceCharge { get; set; }

        public long UserId { get; set; }

        public DateTime Ot { get; set; }

        public string CommissionUser { get; set; }

        /// <summary>
        /// <see cref="ETMS.Entity.Enum.EmStudentAccountRechargeLogStatus"/>
        /// </summary>
        public byte Status { get; set; }

        public string Remark { get; set; }
    }
}
