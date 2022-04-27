using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.Entity.Dto.Educational2.Output
{
    public class TeacherSchooltimeConfigGetPagingOutput
    {
        public long CId { get; set; }

        public string Name { get; set; }

        public IEnumerable<string> RuleDescs { get; set; }

        public IEnumerable<string> ExcludeDate { get; set; }
    }
}
