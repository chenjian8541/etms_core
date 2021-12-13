using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.Entity.ExternalService.Dto.Request
{
    public class ComSendSmscodeRequest
    {
        public string Phone { get; set; }

        public string ValidCode { get; set; }
    }
}
