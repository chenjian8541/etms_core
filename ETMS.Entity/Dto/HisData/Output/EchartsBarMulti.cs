using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.Dto.HisData.Output
{
    public class EchartsBarMulti
    {
        public EchartsBarMulti()
        {
            this.SourceItems = new List<List<string>>();
        }

        public List<List<string>> SourceItems { get; set; }
    }
}
