using ETMS.Entity.Common;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace ETMS.Entity.Dto.Marketing.Request
{
    public class CouponsStudentSendRequest : RequestBase
    {
        public List<long> StudentIds { get; set; }

        public long CouponsId { get; set; }

        public override string Validate()
        {
            if (StudentIds == null || !StudentIds.Any())
            {
                return "请选择学员";
            }
            if (CouponsId == 0)
            {
                return "请选择优惠券";
            }
            return string.Empty;
        }
    }
}
