using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.EtmsManage.Common
{
    public class SelectItem<T>
    {
        public string Label { get; set; }

        public T Value { get; set; }
    }
}
