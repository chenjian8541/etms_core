using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.Dto.HisData.Output
{
    public class EchartsScatter<T>
    {
        public EchartsScatter()
        {
            this.MyData = new List<List<T>>();
        }

        public List<List<T>> MyData { get; set; }
    }
}
