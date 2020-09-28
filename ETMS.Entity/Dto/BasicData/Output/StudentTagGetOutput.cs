using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.Dto.BasicData.Output
{
    public class StudentTagGetOutput
    {
        public List<StudentTagViewOutput> StudentTags { get; set; }
    }

    public class StudentTagViewOutput
    {
        public long CId { get; set; }

        public string Name { get; set; }

        //public string DisplayStyle { get; set; }
    }
}
