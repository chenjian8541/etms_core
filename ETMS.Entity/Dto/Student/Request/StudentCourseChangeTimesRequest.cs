﻿using ETMS.Entity.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.Dto.Student.Request
{
    public class StudentCourseChangeTimesRequest: RequestBase
    {
        public long CId { get; set; }

        public string NewSurplusQuantity { get; set; }

        public DateTime? EndTime { get; set; }

        public List<string> Ot { get; set; }

        public string Remark { get; set; }

        public override string Validate()
        {
            if (CId <= 0)
            {
                return "请求数据格式错误";
            }
            return string.Empty;
        }
    }
}
