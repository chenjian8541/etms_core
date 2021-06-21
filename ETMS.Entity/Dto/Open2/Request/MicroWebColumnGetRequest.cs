using ETMS.Entity.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.Dto.Open2.Request
{
    public class MicroWebColumnGetRequest : Open2Base
    {
        public long ColumnId { get; set; }

        public override string Validate()
        {
            return base.Validate();
        }
    }
}
