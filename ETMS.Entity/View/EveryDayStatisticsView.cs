using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.Entity.View
{
    public class EveryDayStatisticsView
    {
        public int StudentPotentialAddCount { get; set; }

        public int StudentReadingAddCount { get; set; }

        public int OrderAddCount { get; set; }
        
        public decimal IncomeIn { get; set; }

        public decimal IncomeOut { get; set; }

        public string DeClassTimes { get; set; } 

        public decimal DeClassTimesSum { get; set; }

        public int ClassArrivedCount { get; set; }

        public int ClassBeLateCount { get; set; }

        public int ClassLeaveCount { get; set; }

        public int ClassNotArrivedCount { get; set; }
    }
}
