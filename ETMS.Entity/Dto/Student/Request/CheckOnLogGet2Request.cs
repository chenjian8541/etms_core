using ETMS.Entity.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.Entity.Dto.Student.Request
{
    public class CheckOnLogGet2Request : RequestBase
    {
        public long StudentCheckOnLogId { get; set; }

        public override string Validate()
        {
            if (StudentCheckOnLogId <= 0)
            {
                return "请求数据格式错误";
            }
            return base.Validate();
        }
    }
}
