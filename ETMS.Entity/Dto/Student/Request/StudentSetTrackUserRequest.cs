using ETMS.Entity.Common;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace ETMS.Entity.Dto.Student.Request
{
    public class StudentSetTrackUserRequest : RequestBase
    {
        public List<long> StudentCIds { get; set; }

        public long NewTrackUser { get; set; }

        public override string Validate()
        {
            if (StudentCIds == null || !StudentCIds.Any())
            {
                return "请求数据格式错误";
            }
            if (NewTrackUser <= 0)
            {
                return "请选择跟进人";
            }
            return base.Validate();
        }
    }
}
