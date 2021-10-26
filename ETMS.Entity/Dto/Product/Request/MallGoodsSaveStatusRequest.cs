using ETMS.Entity.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.Entity.Dto.Product.Request
{
    public class MallGoodsSaveStatusRequest : RequestBase
    {
        /// <summary>
        /// <see cref="ETMS.Entity.Enum.EmMallGoodsStatus"/>
        /// </summary>
        public byte MallGoodsStatus { get; set; }
    }
}
