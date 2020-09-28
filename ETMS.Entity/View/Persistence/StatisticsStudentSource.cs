using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.View.Persistence
{
    [Serializable]
    public class StatisticsStudentSource
    {
        public long? SourceId { get; set; }

        public int TotalCount { get; set; }
    }
}
