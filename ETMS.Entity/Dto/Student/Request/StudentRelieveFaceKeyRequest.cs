﻿using ETMS.Entity.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.Dto.Student.Request
{
    public class StudentRelieveFaceKeyRequest : RequestBase
    {
        public long CId { get; set; }

        public override string Validate()
        {
            if (CId <= 0)
            {
                return "请求数据不合法";
            }
            return string.Empty;
        }
    }
}