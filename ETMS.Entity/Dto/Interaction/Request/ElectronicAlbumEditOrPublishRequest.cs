using ETMS.Entity.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.Entity.Dto.Interaction.Request
{
    public class ElectronicAlbumEditOrPublishRequest: OpenLinkBase
    {
        public string TempIdNo { get; set; }

        public string CIdNo { get; set; }

        public string RenderKey { get; set; }

        public string CoverKey { get; set; }
    }
}
