using ETMS.Entity.Dto.Parent3.Request;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.Entity.View
{
    public class MallPrepayView
    {
        public long Id { get; set; }

        public byte Type { get; set; }

        public long LcsPayLogId { get; set; }

        /// <summary>
        /// <see cref="ETMS.Entity.Enum.EmMallPrepayStatus"/>
        /// </summary>
        public byte Status { get; set; }

        public ParentBuyMallGoodsSubmitRequest Request { get; set; }
    }
}
