﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.Entity.View
{
    public class GetOpenParent2RequestInfo
    {
        public string OpenId { get; set; }

        public string Unionid { get; set; }

        public long MiniPgmUserId { get; set; }
    }
}