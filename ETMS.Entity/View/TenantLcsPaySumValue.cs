using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.Entity.View
{
    public class TenantLcsPaySumValue
    {
        /// <summary>
        /// 状态
        /// <see cref="ETMS.Entity.Enum.EtmsManage.EmLcsPayLogStatus"/>
        /// </summary>
        public int Status { get; set; }

        public decimal TotalValue { get; set; }
    }
}
