using ETMS.Entity.EtmsManage.Common;
using ETMS.Entity.View;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.Entity.EtmsManage.Dto.SysCommon.Request
{
    public class LiveTeachingConfigSaveRequest : AgentRequestBase
    {
        public LiveTeachingConfig config { get; set; }

        public override string Validate()
        {
            if (config == null)
            {
                return "请输入配置信息"; 
           }
            return base.Validate();
        }
    }
}
