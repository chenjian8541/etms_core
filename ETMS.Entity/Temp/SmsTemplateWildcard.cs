using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.Temp
{
    public class SmsTemplateWildcard
    {
        public int Type { get; set; }

        public List<string> Words { get; set; }
    }
}
