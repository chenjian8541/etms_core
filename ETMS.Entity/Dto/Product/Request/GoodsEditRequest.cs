﻿using ETMS.Entity.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.Dto.Product.Request
{
    public class GoodsEditRequest : RequestBase
    {
        public long CId { get; set; }

        public string Name { get; set; }

        public decimal Price { get; set; }

        public string Remark { get; set; }

        public string LimitQuantity { get; set; }

        public byte Status { get; set; }

        public override string Validate()
        {
            if (CId <= 0)
            {
                return "请求数据不合法";
            }
            if (string.IsNullOrEmpty(Name))
            {
                return "名称不能为空";
            }
            if (Price <= 0)
            {
                return "零售单价必须大于0";
            }
            return base.Validate();
        }
    }
}