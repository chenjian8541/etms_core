﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.Pay.Lcsw.Dto.Request
{
    public class RequestPayBase
    {
        public string accessToken { get; set; }

        /// <summary>
        /// 商户号
        /// </summary>
        public string merchant_no { get; set; }

        /// <summary>
        /// 终端号
        /// </summary>
        public string terminal_id { get; set; }

        /// <summary>
        /// 终端交易时间，yyyyMMddHHmmss，全局统一时间格式
        /// </summary>
        public string terminal_time { get; set; }
    }
}
