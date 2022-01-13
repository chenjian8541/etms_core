using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.Entity.Dto.BasicData.Output
{
    public class AudioGetPagingOutput
    {
        public long CId { get; set; }

        /// <summary>
        /// <see cref="EmLibType"/>
        /// </summary>
        public int Type { get; set; }

        public string AudioKey { get; set; }

        public string AudioUrl { get; set; }

        public string Name { get; set; }

        public DateTime CreateTime { get; set; }

        public bool IsPlay { get; set; }
    }
}
