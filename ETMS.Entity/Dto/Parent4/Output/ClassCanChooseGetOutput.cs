using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.Entity.Dto.Parent4.Output
{
    public class ClassCanChooseGetOutput
    {
        public long Id { get; set; }

        public string Name { get; set; }

        public string TeachersDesc { get; set; }

        public string LimitStudentNumsDesc { get; set; }

        public bool IsCanChoose { get; set; }
    }
}
