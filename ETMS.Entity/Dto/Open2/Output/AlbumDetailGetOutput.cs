using ETMS.Entity.Dto.Common.Output;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.Entity.Dto.Open2.Output
{
    public class AlbumDetailGetOutput
    {
        public long Id { get; set; }

        public string Name { get; set; }

        public string CoverUrl { get; set; }

        public string RenderUrl { get; set; }

        public ShareContent ShareContent { get; set; }
    }
}
