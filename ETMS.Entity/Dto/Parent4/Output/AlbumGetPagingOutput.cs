using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.Entity.Dto.Parent4.Output
{
    public class AlbumGetPagingOutput
    {
        /// <summary>
        /// 主键
        /// </summary>
        public long CId { get; set; }

        public string Name { get; set; }

        public string CoverUrl { get; set; }

        public long StudentId { get; set; }
    }
}
