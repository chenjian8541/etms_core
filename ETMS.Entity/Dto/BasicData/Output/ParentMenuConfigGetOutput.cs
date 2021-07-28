using ETMS.Entity.Config;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.Entity.Dto.BasicData.Output
{
    public class ParentMenuConfigGetOutput
    {
        public List<ParentMenuConfigOutput> Home { get; set; }

        public List<ParentMenuConfigOutput> Me { get; set; }
    }
}
