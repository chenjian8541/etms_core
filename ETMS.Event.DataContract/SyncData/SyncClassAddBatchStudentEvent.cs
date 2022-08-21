using ETMS.Entity.Database.Source;
using ETMS.Entity.Dto.Common.Request;
using ETMS.Entity.Dto.Educational.Request;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.Event.DataContract
{
    public class SyncClassAddBatchStudentEvent : Event
    {
        public SyncClassAddBatchStudentEvent(int tenantId) : base(tenantId)
        { }

        public long ClassId { get; set; }

        public List<ClassAddBatchStudent> Students { get; set; }
    }

    public class ClassAddBatchStudent {

        public long StudentId { get; set; }

        public long CourseId { get; set; }
    }
}
