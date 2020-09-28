using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.Dto.BasicData.Output
{
    public class StudentExtendFieldGetOutput
    {
        public List<StudentExtendFieldViewOutput> Fields { get; set; }
    }

    public class StudentExtendFieldViewOutput
    {
        public long CId { get; set; }

        public string Name { get; set; }
    }
}
