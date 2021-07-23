using ETMS.Entity.Database.Source;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.Event.DataContract
{
    public class UpdateStudentInfoEvent : Event
    {
        public UpdateStudentInfoEvent(int tenantId) : base(tenantId)
        { }

        public EtStudent MyStudent { get; set; }
    }
}

