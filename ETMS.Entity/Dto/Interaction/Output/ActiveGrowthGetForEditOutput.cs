using ETMS.Entity.Dto.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.Entity.Dto.Interaction.Output
{
    public class ActiveGrowthGetForEditOutput
    {
        public long CId { get; set; }

        public string GrowthContent { get; set; }

        public List<Img> GrowthMediasKeys { get; set; }
    }
}
