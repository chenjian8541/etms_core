using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.Entity.Dto.CoreBusiness.Output
{
    public class BarcodePayOutput : AgtPayServiceOutputBase
    {
        /// <summary>
        /// <see cref="ETMS.Entity.Enum.EtmsManage.EmLcsPayLogStatus"/>
        /// </summary>
        public int PayResultType { get; set; }

        public string OutTradeNo { get; set; }

        public string PayType { get; set; }
    }
}
