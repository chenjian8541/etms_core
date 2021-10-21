using ETMS.Entity.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.Entity.Dto.Product.Request
{
    public class MallGoodsGetPagingRequest : RequestPagingBase
    {
        public string Name { get; set; }

        public byte? ProductType { get; set; }

        /// <summary>
        /// 获取SQL语句
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            var condition = new StringBuilder(DataFilterWhere);
            if (!string.IsNullOrEmpty(Name))
            {
                condition.Append($" AND (Name like '%{Name}%' OR RelatedName '%{Name}%')");
            }
            else if (ProductType != null)
            {
                condition.Append($" AND [ProductType] = {ProductType.Value}");
            }
            return condition.ToString();
        }
    }
}
