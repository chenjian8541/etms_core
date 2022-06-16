using ETMS.Entity.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.Entity.Dto.Activity.Request
{
    public class ActivityMainGetForEditRequest : RequestBase
    {
        public long ActivityMainId { get; set; }

        public override string Validate()
        {
            if (ActivityMainId <= 0)
            {
                return "请求数据格式错误";
            }
            return string.Empty;
        }
    }
}
