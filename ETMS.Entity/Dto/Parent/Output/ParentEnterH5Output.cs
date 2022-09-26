using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.Entity.Dto.Parent.Output
{
    public class ParentEnterH5Output
    {
        public bool IsOtherLogin { get; set; }

        public ParentLoginBySmsOutput LoginIngo { get; set; }
    }
}
