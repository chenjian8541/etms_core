﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.Entity.Dto.OpenParent2.Output
{
    public class WxMiniHagglingStartGoOutput
    {
        public bool IsMustPay { get; set; }

        public WxMiniGroupPurchasePayInfo PayInfo { get; set; }
    }
}