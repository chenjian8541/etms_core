using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.Temp.View
{
    [Serializable]
    public class MicroWebConfigBannerSetView
    {
        public bool IsShowInHome { get; set; }

        public List<string> Images { get; set; }
    }
}
