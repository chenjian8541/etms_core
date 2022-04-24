using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace ETMS.Entity.Database.Source
{
    [Table("EtTeacherSchooltimeConfigExclude")]
    public class EtTeacherSchooltimeConfigExclude : Entity<long>
    {
        public long TeacherId { get; set; }

        public string ExcludeDateContent { get; set; }
    }
}
