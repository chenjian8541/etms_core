using ETMS.Entity.EtmsManage.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.Entity.EtmsManage.Dto.Agent3.Request
{
    public class SysSysNoticeBulletinAddRequest : AgentRequestBase
    {
        public string Title { get; set; }

        public string LinkUrl { get; set; }

        public DateTime? EndTime { get; set; }

        public byte IsAdvertise { get; set; }

        public override string Validate()
        {
            if (string.IsNullOrEmpty(Title))
            {
                return "请输入标题";
            }
            return string.Empty;
        }
    }
}
