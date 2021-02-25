using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace ETMS.Entity.Database.Source
{
    /// <summary>
    /// 学员储值账户
    /// </summary>
    [Table("EtStudentAccountRecharge")]
    public class EtStudentAccountRecharge: Entity<long>
    {
        public string Phone { get; set; }

        public decimal BalanceSum { get; set; }

        public decimal BalanceReal { get; set; }

        public decimal BalanceGive { get; set; }

        public decimal RechargeSum { get; set; }

        public decimal RechargeGiveSum { get; set; }

        public DateTime Ot { get; set; }
    }
}
