using ETMS.Entity.Common;
using ETMS.Entity.Enum;
using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.Dto.Marketing.Request
{
    public class CouponsAddRequest : RequestBase
    {
        /// <summary>
        /// 类型  <see cref="ETMS.Entity.Enum.EmCouponsType"/>
        /// </summary>
        public byte Type { get; set; }

        public string CashValue { get; set; }

        public string ClassTimesValue { get; set; }

        public string DiscountValue { get; set; }

        public string EndOffset { get; set; }

        public List<string> ExpiredTimeDesc { get; set; }

        public byte ExpiredType { get; set; }

        public string LimitGetAll { get; set; }

        public string LimitGetSingle { get; set; }

        public byte LimitType { get; set; }

        public string MinLimit { get; set; }

        /// <summary>
        /// 优惠券标题
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// 发行总量
        /// </summary>
        public int TotalCount { get; set; }

        /// <summary>
        /// 使用说明
        /// </summary>
        public string UseExplain { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; }


        public override string Validate()
        {
            if (string.IsNullOrEmpty(Title))
            {
                return "优惠券标题不能为空";
            }
            if (this.TotalCount <= 0)
            {
                return "发行总量必须大于0";
            }
            return base.Validate();
        }
    }
}
