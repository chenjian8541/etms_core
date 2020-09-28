using ETMS.Entity.Database.Source;
using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Event.DataContract
{
    public class ClassCheckSignEvent : Event
    {
        public ClassCheckSignEvent(int tenantId) : base(tenantId)
        { }

        public string ClassName { get; set; }

        public EtClassRecord ClassRecord { get; set; }

        public List<EtClassRecordStudent> ClassRecordStudents { get; set; }
    }
}
