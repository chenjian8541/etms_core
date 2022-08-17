using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.Event.DataContract
{
    public class SyncStudentAccountRechargeRelationStudentIdsEvent : Event
    {
        public SyncStudentAccountRechargeRelationStudentIdsEvent(int tenantId) : base(tenantId)
        { }

        public long StudentAccountRechargeId { get; set; }
    }
}
