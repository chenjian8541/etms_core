using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.Entity.View
{
    public class TenantAgtPayInfoView
    {
        public bool IsOpenAgtPay { get; set; }

        /// <summary>
        /// 聚合支付状态类型 <see cref="ETMS.Entity.Enum.EtmsManage.EmAgtPayType"/>
        /// </summary>
        public int AgtPayType { get; set; }

        public string AgtPayDesc { get; set; }

        public string AgtPayDesc2 { get; set; }
    }
}
