using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.Dto.BasicData.Output
{
    public class StudentRelationshipGetOutput
    {
        public List<StudentRelationshipViewOutput> StudentRelationships { get; set; }
    }

    public class StudentRelationshipViewOutput
    {
        public long CId { get; set; }

        public string Name { get; set; }
    }
}
