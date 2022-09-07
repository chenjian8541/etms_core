using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.Event.DataContract
{
    public class StudentAutoAddAccountRechargeEvent : Event
    {
        public StudentAutoAddAccountRechargeEvent(int tenantId) : base(tenantId)
        {
        }

        public long StudentId { get; set; }

        public decimal AddMoney { get; set; }

        /// <summary>
        /// <see cref="ETMS.Entity.Enum.EmStudentAccountRechargeLogType"/>
        /// </summary>
        public int RechargeLogType { get; set; }

        public int PayType { get; set; }

        public new long UserId { get; set; }

        public string OrderNo { get; set; }

        public long OrderId { get; set; }

        public string Remark { get; set; }
    }
}
