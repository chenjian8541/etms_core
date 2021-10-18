using ETMS.Entity.Database.Source;
using ETMS.Entity.Dto.External.Request;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.Event.DataContract
{
    public class ImportExtendFieldExcelEvent : Event
    {
        public ImportExtendFieldExcelEvent(int tenantId) : base(tenantId)
        {
        }

        public List<EtStudent> Students { get; set; }

        public List<ImportStudentExtendFieldItem> StudentExtendFieldItems { get; set; }
    }
}
