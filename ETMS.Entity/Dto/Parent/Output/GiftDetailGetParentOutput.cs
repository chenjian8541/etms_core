using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.Dto.Parent.Output
{
    public class GiftDetailGetParentOutput
    {
        public long Id { get; set; }

        public long? GiftCategoryId { get; set; }

        public string Img { get; set; }

        public string Name { get; set; }

        public int Points { get; set; }


        public string GiftContent { get; set; }

        public int Nums { get; set; }

        /// <summary>
        /// 库存不足是否允许兑换
        /// </summary>
        public bool IsAllowLackNums { get; set; }
    }
}
