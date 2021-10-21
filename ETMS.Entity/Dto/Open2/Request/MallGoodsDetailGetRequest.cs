using ETMS.Entity.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.Entity.Dto.Open2.Request
{
    public class MallGoodsDetailGetRequest : Open2Base
    {
        public long GId { get; set; }

        public override string Validate()
        {
            if (GId <= 0)
            {
                return "请求数据格式错误";
            }
            return base.Validate();
        }
    }
}
