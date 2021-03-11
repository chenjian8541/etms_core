using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.Dto.Parent.Output
{
    public class StudentAccountRechargeLogGetPagingOutput
    {
        public long Id { get; set; }

        public long StudentAccountRechargeId { get; set; }

        public string CgNo { get; set; }

        /// <summary>
        /// <see cref="ETMS.Entity.Enum.EmStudentAccountRechargeLogType"/>
        /// </summary>
        public int Type { get; set; }

        /// <summary>
        /// <see cref="ETMS.Entity.Enum.EmValueChangeType"/>
        /// </summary>
        public int ChangeType { get; set; }

        public string TypeDesc { get; set; }

        public string CgBalanceTotalDesc { get; set; }

        public string CgBalanceRealDesc { get; set; }

        public string CgBalanceGiveDesc { get; set; }

        public string OtDesc { get; set; }
    }
}
