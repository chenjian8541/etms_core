using ETMS.Entity.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.Dto.Student.Request
{
    public class StudentDelListRequest : RequestBase
    {
        public List<long> CIds { get; set; }

        public override string Validate()
        {
            if (CIds == null || CIds.Count == 0)
            {
                return "请选择学员";
            }
            return string.Empty;
        }
    }
}