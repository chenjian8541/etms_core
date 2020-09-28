using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.Dto.HisData.Output
{
    public class EchartsPie<T>
    {
        public EchartsPie()
        {
            this.LegendData = new List<string>();
            this.MyData = new List<EchartsPieData<T>>();
        }
        public List<string> LegendData { get; set; }

        public List<EchartsPieData<T>> MyData { get; set; }
    }

    public class EchartsPieData<T>
    {
        public string Name { get; set; }

        public T Value { get; set; }
    }
}
