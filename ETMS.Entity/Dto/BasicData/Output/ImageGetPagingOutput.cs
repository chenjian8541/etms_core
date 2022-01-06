using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ETMS.Entity.Enum;

namespace ETMS.Entity.Dto.BasicData.Output
{
    public class ImageGetPagingOutput
    {
        public long CId { get; set; }

        /// <summary>
        /// <see cref="EmLibType"/>
        /// </summary>
        public int Type { get; set; }

        public string ImgKey { get; set; }

        public string ImgUrl { get; set; }
    }
}
