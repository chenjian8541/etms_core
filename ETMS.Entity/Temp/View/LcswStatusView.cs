using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.Entity.Temp.View
{
    public class LcswStatusView
    {
        /// <summary>
        /// 利楚扫呗申请状态
        /// <see cref="ETMS.Entity.Enum.EtmsManage.EmLcswApplyStatus"/>
        /// </summary>
        public int LcswApplyStatus { get; set; }

        /// <summary>
        /// 聚合支付(利楚扫呗)开启状态
        /// <see cref="ETMS.Entity.Enum.EmBool"/>
        /// </summary>
        public byte LcswOpenStatus { get; set; }
    }
}
