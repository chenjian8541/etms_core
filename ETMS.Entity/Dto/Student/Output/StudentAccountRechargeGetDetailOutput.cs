using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.Dto.Student.Output
{
    public class StudentAccountRechargeGetDetailOutput
    {
        public long Id { get; set; }

        public string Phone { get; set; }

        public decimal BalanceSum { get; set; }

        public decimal BalanceReal { get; set; }

        public decimal BalanceGive { get; set; }

        public decimal RechargeSum { get; set; }

        public decimal RechargeGiveSum { get; set; }

        public DateTime Ot { get; set; }

        public List<StudentAccountRechargeBinder> RelationStudent { get; set; }
    }

    public class StudentAccountRechargeBinder 
    {
        public long StudentAccountRechargeBinderId { get; set; }

        public long StudentId { get; set; }

        public string StudentName { get; set; }

        public string StudentPhone { get; set; }

        public string StudentAvatar { get; set; }

        public string StudentAvatarUrl { get; set; }
    }
}
