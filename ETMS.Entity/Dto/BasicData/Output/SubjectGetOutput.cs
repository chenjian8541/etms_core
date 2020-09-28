using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.Dto.BasicData.Output
{
    public class SubjectGetOutput
    {
        public List<SubjectViewOutput> Subjects { get; set; }
    }

    public class SubjectViewOutput
    {
        public long CId { get; set; }

        public string Name { get; set; }
    }
}
