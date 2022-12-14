using ETMS.Entity.EtmsManage.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.Entity.EtmsManage.Dto.DataLog.Request
{
    public class TryApplyLogHandleRequest : AgentRequestBase
    {
        public int Id { get; set; }

        public string HandleContent { get; set; }

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