using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.Entity.View
{
    public class LiveTeachingConfig
    {
        public bool IsOpen { get; set; }

        public List<LiveTeachingTimeRule> Rules { get; set; }

        public string LiveTitle { get; set; }

        public string LiveUrl { get; set; }

        public string LiveNo { get; set; }

        public string LivePwd { get; set; }
    }

    public class LiveTeachingTimeRule
    {
        public int Week { get; set; }

        public string WeekDesc { get; set; }

        public int StartTime { get; set; }

        public string StartTimeDesc { get; set; }

        public int EndTime { get; set; }

        public string EndTimeDesc { get; set; }
    }
}
