using ETMS.Entity.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.Dto.Interaction.Request
{
    public class MicroWebColumnChangeStatusRequest : RequestBase
    {
        public long Id { get; set; }

        public byte NewStatus { get; set; }

        public override string Validate()
        {
            return string.Empty;
        }
    }
}
