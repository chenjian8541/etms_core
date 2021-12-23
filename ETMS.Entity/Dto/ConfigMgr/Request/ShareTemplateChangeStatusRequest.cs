using ETMS.Entity.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.Entity.Dto.ConfigMgr.Request
{
    public class ShareTemplateChangeStatusRequest : RequestBase
    {
        public long Id { get; set; }

        public byte NewStatus { get; set; }

        public override string Validate()
        {
            if (Id <= 0)
            {
                return "请求数据格式错误";
            }
            return string.Empty;
        }
    }
}
