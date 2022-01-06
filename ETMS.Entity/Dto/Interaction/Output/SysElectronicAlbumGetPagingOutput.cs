using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.Entity.Dto.Interaction.Output
{
    public class SysElectronicAlbumGetPagingOutput
    {
        /// <summary>
        /// 主键
        /// </summary>
        public long CId { get; set; }

        /// <summary>
        /// <see cref="EmElectronicAlbumType"/>
        /// </summary>
        public int Type { get; set; }

        public string Name { get; set; }

        public string CoverKey { get; set; }

        public string RenderData { get; set; }
    }
}
