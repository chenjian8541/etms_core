using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.View.Persistence
{
    [Serializable]
    public class StatisticsStudentType
    {
        public byte StudentType { get; set; }

        public int TotalCount { get; set; }
    }
}
