using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace ETMS.Entity.Database.Source
{
    /// <summary>
    /// 机构储值统计
    /// </summary>
    [Table("EtStatisticsStudentAccountRecharge")]
    public class EtStatisticsStudentAccountRecharge : Entity<long>
    {
        public int AccountCount { get; set; }

        public decimal BalanceSum { get; set; }

        public decimal BalanceReal { get; set; }

        public decimal BalanceGive { get; set; }

        public decimal RechargeSum { get; set; }

        public decimal RechargeGiveSum { get; set; }
    }
}
