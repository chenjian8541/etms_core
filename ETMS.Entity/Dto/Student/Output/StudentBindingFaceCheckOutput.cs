using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.Dto.Student.Output
{
    public class StudentBindingFaceCheckOutput
    {
        public bool IsCanBinding { get; set; }

        public bool IsSameStudent { get; set; }

        public string ErrMsg { get; set; }

        public string SameStudentName { get; set; }

        public string SameStudentPhone { get; set; }
    }
}
