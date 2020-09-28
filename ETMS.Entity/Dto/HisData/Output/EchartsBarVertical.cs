using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.Dto.HisData.Output
{
    public class EchartsBarVertical<T>
    {
        public EchartsBarVertical()
        {
            this.YData = new List<string>();
            this.XData = new List<T>();
        }

        public List<string> YData { get; set; }

        public List<T> XData { get; set; }
    }
}
