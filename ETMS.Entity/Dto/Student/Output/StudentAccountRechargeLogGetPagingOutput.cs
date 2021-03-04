using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.Dto.Student.Output
{
    public class StudentAccountRechargeLogGetPagingOutput
    {
        public long Id { get; set; }

        public long StudentAccountRechargeId { get; set; }

        public string Phone { get; set; }

        public long? RelatedOrderId { get; set; }

        public string CgNo { get; set; }

        /// <summary>
        /// <see cref="ETMS.Entity.Enum.EmStudentAccountRechargeLogType"/>
        /// </summary>
        public int Type { get; set; }

        public string TypeDesc { get; set; }

        public decimal CgBalanceReal { get; set; }

        public string CgBalanceRealDesc { get; set; }

        public decimal CgBalanceGive { get; set; }

        public string CgBalanceGiveDesc { get; set; }

        public decimal CgServiceCharge { get; set; }

        public string CgServiceChargeDesc { get; set; }

        public long UserId { get; set; }

        public string UserDesc { get; set; }

        public DateTime Ot { get; set; }

        public string CommissionUser { get; set; }

        public string CommissionUserDesc { get; set; }

        /// <summary>
        /// <see cref="ETMS.Entity.Enum.EmStudentAccountRechargeLogStatus"/>
        /// </summary>
        public byte Status { get; set; }

        public string Remark { get; set; }

        public List<string> RelationStudent { get; set; }
    }
}
