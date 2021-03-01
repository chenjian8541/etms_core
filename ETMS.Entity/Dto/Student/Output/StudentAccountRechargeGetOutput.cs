using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.Dto.Student.Output
{
    public class StudentAccountRechargeGetOutput
    {
        public long Id { get; set; }

        public string Phone { get; set; }

        public decimal BalanceSum { get; set; }

        public decimal BalanceReal { get; set; }

        public decimal BalanceGive { get; set; }

        public decimal RechargeSum { get; set; }

        public decimal RechargeGiveSum { get; set; }

        public DateTime Ot { get; set; }
    }
}
