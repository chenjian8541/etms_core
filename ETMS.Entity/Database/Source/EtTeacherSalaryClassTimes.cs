using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace ETMS.Entity.Database.Source
{
    [Table("EtTeacherSalaryClassTimes")]
    public class EtTeacherSalaryClassTimes : BaseTeacherSalaryClassTimes
    {
        public long CourseId { get; set; }
    }
}
