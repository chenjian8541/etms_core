using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.Dto.BasicData.Output
{
    public class StudentGrowingTagGetOutput
    {
        public List<StudentGrowingTagViewOutput> StudentGrowingTags { get; set; }
    }

    public class StudentGrowingTagViewOutput
    {
        public long CId { get; set; }

        public string Name { get; set; }
    }
}
