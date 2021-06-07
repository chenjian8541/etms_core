using ETMS.Entity.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.Dto.Interaction.Request
{
    public class MicroWebColumnGetRequest : RequestBase
    {
        public long Id { get; set; }

        public override string Validate()
        {
            return string.Empty;
        }
    }
}
