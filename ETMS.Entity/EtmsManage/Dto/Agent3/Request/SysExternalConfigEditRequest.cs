using ETMS.Entity.EtmsManage.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.Entity.EtmsManage.Dto.Agent3.Request
{
    public class SysExternalConfigEditRequest : AgentRequestBase
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public string Data1 { get; set; }

        public string Data2 { get; set; }

        public override string Validate()
        {
            if (string.IsNullOrEmpty(Name))
            {
                return "请输入名称";
            }
            if (string.IsNullOrEmpty(Data1))
            {
                return "请输入Data1";
            }
            return string.Empty;
        }
    }
}
