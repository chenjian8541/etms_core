using ETMS.Entity.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.Dto.External.Request
{
    public class ImportStudentRequest : RequestBase
    {
        public List<ImportStudentContentItem> ImportStudents { get; set; }

        public List<ImportStudentExtendFieldItem> StudentExtendFieldItems { get; set; }
        public override string Validate()
        {
            if (ImportStudents == null || ImportStudents.Count == 0)
            {
                return "导入的学员个数必须大于0";
            }
            return base.Validate();
        }
    }
}
