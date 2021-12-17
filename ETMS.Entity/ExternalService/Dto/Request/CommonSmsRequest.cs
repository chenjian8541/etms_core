using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.Entity.ExternalService.Dto.Request
{
    public class CommonSmsRequest 
    {
        public List<CommonSmsItem> Items { get; set; }
    }

    public class CommonSmsItem {
        public string Phone { get; set; }

        public string SmsContent { get; set; }
    }
}
