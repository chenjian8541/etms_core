using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.Entity.Dto.PaymentService.Output
{
    public class LcsPayJspayCallbackOutput
    {
        public string return_code { get; set; }

        public string return_msg { get; set; }
    }
}
