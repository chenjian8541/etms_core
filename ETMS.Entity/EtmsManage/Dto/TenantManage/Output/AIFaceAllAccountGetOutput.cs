using ETMS.Entity.EtmsManage.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.EtmsManage.Dto.TenantManage.Output
{
    public class AIFaceAllAccountGetOutput
    {
        public List<SelectItem<int>> TenantAccounts { get; set; }

        public List<SelectItem<int>> BiduAccounts { get; set; }
    }
}
