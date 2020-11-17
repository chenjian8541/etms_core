using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace ETMS.Entity.Database.Manage
{
    [Table("SysWechartAuthTemplateMsg")]
    public class SysWechartAuthTemplateMsg : EManageEntity<long>
    {
        public string AuthorizerAppid { get; set; }

        public string TemplateIdShort { get; set; }

        public string TemplateId { get; set; }

        public DateTime UpOt { get; set; }
    }
}
