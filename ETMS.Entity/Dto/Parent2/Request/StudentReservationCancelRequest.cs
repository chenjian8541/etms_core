using ETMS.Entity.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.Dto.Parent2.Request
{
    public class StudentReservationCancelRequest : ParentRequestBase
    {
        public long StudentId { get; set; }

        public long ClassTimesId { get; set; }

        public override string Validate()
        {
            if (StudentId <= 0 || ClassTimesId <= 0)
            {
                return "请求数据格式错误";
            }
            return string.Empty;
        }
    }
}
