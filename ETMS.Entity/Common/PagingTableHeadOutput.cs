using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.Entity.Common
{
    public class PagingTableHeadOutput
    {
        public long Id { get; set; }

        public string Label { get; set; }

        public int Index { get; set; }

        public string Property { get; set; }

        public object OtherInfo { get; set; }

        /// <summary>
        /// <see cref="ETMS.Entity.Enum.PagingTableHeadType"/>
        /// </summary>
        public byte Type { get; set; }
    }
}
