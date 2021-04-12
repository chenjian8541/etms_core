using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace ETMS.Entity.Database.Manage
{
    [Table("SysTenantWechartError")]
    public class SysTenantWechartError : EManageEntity<int>
    {
        public int TenantId { get; set; }

        public DateTime Ot { get; set; }

        public string TemplateIdShort { get; set; }

        public string AuthorizerAppid { get; set; }

        public string ErrMsg { get; set; }
    }
}
