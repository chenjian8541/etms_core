using ETMS.Entity.Dto.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.Dto.Marketing.Output
{
    public class GiftGetOutput
    {
        public long? GiftCategoryId { get; set; }

        public string Name { get; set; }

        public int Nums { get; set; }

        public int Points { get; set; }

        /// <summary>
        /// 库存不足是否允许兑换
        /// </summary>
        public bool IsLimitNums { get; set; }

        public List<Img> Imgs { get; set; }

        public string GiftContent { get; set; }

        public int NumsLimit { get; set; }

        public string Remark { get; set; }
    }
}
