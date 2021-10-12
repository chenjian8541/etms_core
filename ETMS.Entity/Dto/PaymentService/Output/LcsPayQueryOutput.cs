using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.Entity.Dto.PaymentService.Output
{
    public class LcsPayQueryOutput
    {
        /// <summary>
        /// <see cref="ETMS.Entity.Enum.EtmsManage.EmLcsPayLogStatus"/>
        /// </summary>
        public int PayStatus { get; set; }
    }
}
