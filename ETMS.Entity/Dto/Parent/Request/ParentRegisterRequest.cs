using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.Entity.Dto.Parent.Request
{
    public class ParentRegisterRequest
    {
        public string TenantNo { get; set; }

        public string Phone { get; set; }

        public string StudentName { get; set; }

        public string Address { get; set; }

        public string Remark { get; set; }

        public string SmsCode { get; set; }

        public string Code { get; set; }
    }
}
