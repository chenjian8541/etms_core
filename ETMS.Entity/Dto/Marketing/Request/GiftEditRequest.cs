using ETMS.Entity.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.Dto.Marketing.Request
{
    public class GiftEditRequest : RequestBase
    {
        public long CId { get; set; }

        public long? GiftCategoryId { get; set; }

        public string Name { get; set; }

        /// <summary>
        /// 多个图片以”|”隔开
        /// </summary>
        public string ImgPathKeys { get; set; }

        public int Nums { get; set; }

        /// <summary>
        /// 库存不足是否允许兑换
        /// </summary>
        public bool IsLimitNums { get; set; }
        public int Points { get; set; }

        public string GiftContent { get; set; }

        public int NumsLimit { get; set; }

        public string Remark { get; set; }

        public override string Validate()
        {
            if (CId <= 0)
            {
                return "请求数据不合法";
            }
            if (string.IsNullOrEmpty(Name))
            {
                return "礼品名称不能为空";
            }
            if (Points <= 0)
            {
                return "所需积分必须大于0";
            }
            return base.Validate();
        }
    }
}
