using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.Entity.Dto.Common
{
    public class CompareValue
    {
        public string Desc { get; set; }

        /// <summary>
        /// <see cref="ETMS.Entity.Enum.EmCompareValueType"/>
        /// </summary>
        public byte Type { get; set; }
    }
}
