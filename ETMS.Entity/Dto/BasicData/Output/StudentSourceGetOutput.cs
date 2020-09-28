using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.Dto.BasicData.Output
{
    public class StudentSourceGetOutput
    {
        public List<StudentSourceViewOutput> StudentSources { get; set; }
    }

    public class StudentSourceViewOutput
    {
        public long CId { get; set; }

        public string Name { get; set; }

    }
}
