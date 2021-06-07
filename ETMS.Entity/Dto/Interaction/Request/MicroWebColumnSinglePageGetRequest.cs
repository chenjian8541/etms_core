using ETMS.Entity.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.Dto.Interaction.Request
{
    public class MicroWebColumnSinglePageGetRequest : RequestBase
    {
        public long ColumnId { get; set; }

        public override string Validate()
        {
            return string.Empty;
        }
    }
}
