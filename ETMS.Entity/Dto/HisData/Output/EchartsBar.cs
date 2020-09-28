using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.Dto.HisData.Output
{
    public class EchartsBar<T>
    {
        public EchartsBar()
        {
            this.XData = new List<string>();
            this.MyData = new List<T>();
        }

        public List<string> XData { get; set; }

        public List<T> MyData { get; set; }
    }
}
