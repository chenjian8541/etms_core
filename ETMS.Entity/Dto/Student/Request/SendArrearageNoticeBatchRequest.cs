using ETMS.Entity.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.Dto.Student.Request
{
    public class SendArrearageNoticeBatchRequest : RequestBase
    {
        public List<long> OrderIds { get; set; }

        public override string Validate()
        {
            if (OrderIds == null || OrderIds.Count == 0)
            {
                return "请选择订单";
            }
            return string.Empty;
        }
    }
}
