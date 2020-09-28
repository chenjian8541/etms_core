﻿using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.ExternalService.ExProtocol.ZhuTong.Response
{
    /// <summary>
    /// https://doc.zthysms.com/web/#/1?page_id=13
    /// </summary>
    public class SendSmsTpRes
    {
        public int code { get; set; }

        public string msg { get; set; }

        public static bool IsSuccess(SendSmsTpRes res)
        {
            return res.code == 200;
        }
    }
}
