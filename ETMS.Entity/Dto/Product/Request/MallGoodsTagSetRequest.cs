using ETMS.Entity.Common;
using ETMS.Entity.View.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.Entity.Dto.Product.Request
{
    public class MallGoodsTagSetRequest : RequestBase
    {
        public List<long> Ids { get; set; }

        public List<MallGoodsTagItem> TagItems { get; set; }

        public override string Validate()
        {
            if (Ids == null || Ids.Count == 0)
            {
                return "请求数据格式错误";
            }
            if (TagItems != null && TagItems.Count > 2)
            {
                return "一件商品最多设置两个标签";
            }
            return string.Empty;
        }
    }
}