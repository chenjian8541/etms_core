using ETMS.Entity.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.Dto.Interaction.Request
{
    public class MicroWebColumnGetListRequest : RequestBase
    {
        public bool IsOnlyEnable { get; set; }
    }
}
