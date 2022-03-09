using ETMS.Entity.Database.Source;
using ETMS.Entity.View;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.Event.DataContract
{
    public class SyncStudentLogOfSurplusCourseEvent : Event
    {
        public SyncStudentLogOfSurplusCourseEvent(int tenantId) : base(tenantId)
        { }

        public int Type { get; set; }

        public IEnumerable<SyncStudentLogOfSurplusCourseView> Logs { get; set; }
    }

    public struct SyncStudentLogOfSurplusCourseEventType
    {
        public const int ClassRecordStudent = 0;

        public const int StudentCourseConsumeLog = 1;
    }
}
