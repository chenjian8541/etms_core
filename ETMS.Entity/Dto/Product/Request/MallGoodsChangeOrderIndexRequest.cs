using ETMS.Entity.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.Entity.Dto.Product.Request
{
    public class MallGoodsChangeOrderIndexRequest : RequestBase
    {
        public long Id1 { get; set; }

        public long OrderIndex1 { get; set; }

        public long Id2 { get; set; }

        public long OrderIndex2 { get; set; }

        public override string Validate()
        {
            if (Id1 <= 0 || Id2 <= 0)
            {
                return "请求数据格式错误";
            }
            return string.Empty;
        }
    }
}
