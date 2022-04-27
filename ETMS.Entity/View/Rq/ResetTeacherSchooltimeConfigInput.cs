using ETMS.Entity.Database.Source;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.Entity.View.Rq
{
    public class ResetTeacherSchooltimeConfigInput
    {
        public long TeacherId { get; set; }

        public List<ResetTeacherSchooltimeConfigInputItem> Items { get; set; }

        public EtTeacherSchooltimeConfigExclude ExcludeConfig { get; set; }
    }

    public class ResetTeacherSchooltimeConfigInputItem
    {
        public EtTeacherSchooltimeConfig TeacherSchooltimeConfig { get; set; }

        public List<EtTeacherSchooltimeConfigDetail> TeacherSchooltimeConfigDetails { get; set; }
    }
}
