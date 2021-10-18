using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.Entity.Dto.External.Request
{
    public class ImportStudentExtendFieldItem
    {
        public string StudentName { get; set; }

        public string Phone { get; set; }

        public List<ImportStudentExtendInfo> ExtendInfoList { get; set; }
    }
}
