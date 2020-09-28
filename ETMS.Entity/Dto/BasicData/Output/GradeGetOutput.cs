using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.Dto.BasicData.Output
{
    public class GradeGetOutput
    {
        public List<GradeViewOutput> Grades { get; set; }
    }

    public class GradeViewOutput
    {
        public long CId { get; set; }

        public string Name { get; set; }
    }
}
