using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.Dto.Parent.Output
{
    public class StudentAccountRechargeGetOutput
    {
        public long Id { get; set; }

        public string Phone { get; set; }

        public string BalanceSumDesc { get; set; }

        public string BalanceRealDesc { get; set; }

        public string BalanceGiveDesc { get; set; }

        public DateTime Ot { get; set; }

         public List<AccountRechargeBinder> Students { get; set; }
    }

    public class AccountRechargeBinder {

        public long StudentId { get; set; }

        public string StudentName { get; set; }

        public string StudentPhone { get; set; }

        public string StudentAvatarUrl { get; set; }

    }
}
