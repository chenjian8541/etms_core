using ETMS.Entity.EtmsManage.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.Entity.EtmsManage.Dto.Agent3.Request
{
    public class SysSysNoticeBulletinDelRequest : AgentRequestBase
    {
        public int Id { get; set; }

        public override string Validate()
        {
            if (Id <= 0)
            {
                return "Id不能为空";
            }
            return base.Validate();
        }
    }
}