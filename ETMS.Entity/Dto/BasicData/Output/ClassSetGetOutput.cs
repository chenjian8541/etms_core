using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.Dto.BasicData.Output
{
    public class ClassSetGetOutput
    {
        public List<ClassSetViewOutput> ClassSets { get; set; }
    }

    public class ClassSetViewOutput
    {
        public long CId { get; set; }

        public string StartTimeDesc { get; set; }

        public string EndTimeDesc { get; set; }
    }
}
