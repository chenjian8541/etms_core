using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.Dto.Student.Output
{
    public class StudentExtendFieldInitOutput
    {
        public List<StudentExtendFieldItems> FieldItems { get; set; }

        public List<StudentExtendFieldValues> FieldValues { get; set; }
    }
}
