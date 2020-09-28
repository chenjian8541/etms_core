using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.ExternalService.Dto.Request
{
    public class SmsBase
    {
        public SmsBase(int tenantId)
        {
            this.LoginTenantId = tenantId;
        }

        /// <summary>
        /// 机构ID
        /// </summary>
        public int LoginTenantId { get; set; }
    }
}
