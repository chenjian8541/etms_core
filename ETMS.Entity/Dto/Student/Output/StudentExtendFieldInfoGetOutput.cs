using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.Entity.Dto.Student.Output
{
    public class StudentExtendFieldInfoGetOutput
    {
        public List<string> DisplayNameList { get; set; }

        public List<string> FileNameList { get; set; }

        public List<StudentExtendFieldInfoValue> StudentExtendFieldInfoValueList { get; set; }
    }

    public class StudentExtendFieldInfoValue
    {
        public long StudentId { get; set; }

        public List<ExtendFieldInfoValue> InfoValueList { get; set; }
    }

    public class ExtendFieldInfoValue
    {
        public string FileName { get; set; }

        public string FileValue { get; set; }
    }
}
