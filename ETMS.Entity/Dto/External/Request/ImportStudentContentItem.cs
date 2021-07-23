using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.Dto.External.Request
{
    public class ImportStudentContentItem
    {
        public string StudentName { get; set; }

        public string Phone { get; set; }

        public string PhoneRelationshipDesc { get; set; }

        public string GenderDesc { get; set; }

        public DateTime? Birthday { get; set; }

        public string SchoolName { get; set; }

        public string GradeDesc { get; set; }

        public string SourceDesc { get; set; }

        public string PhoneBak { get; set; }

        public string HomeAddress { get; set; }

        public string Remark { get; set; }

        public string CardNo { get; set; }
    }
}
