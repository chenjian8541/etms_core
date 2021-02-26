using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.View
{
    public class StatisticsStudentAccountRechargeView
    {
        public int AccountCount { get; set; }

        public decimal BalanceSum { get; set; }

        public decimal BalanceReal { get; set; }

        public decimal BalanceGive { get; set; }

        public decimal RechargeSum { get; set; }

        public decimal RechargeGiveSum { get; set; }
    }
}
