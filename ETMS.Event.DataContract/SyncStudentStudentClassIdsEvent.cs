using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.Event.DataContract
{
    public class SyncStudentStudentClassIdsEvent : Event
    {
        public SyncStudentStudentClassIdsEvent(int tenantId,long studentId) : base(tenantId)
        {
            this.StudentId = studentId;
        }

        public long StudentId { get; set; }
    }
}

