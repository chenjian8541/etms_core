using ETMS.Entity.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.Entity.Dto.Educational.Request
{
    public class ClassChangeOnlineSelStatusRequest : RequestBase
    {
        public List<long> Ids { get; set; }

        public byte NewOnlineSelStatus { get; set; }

        public override string Validate()
        {
            if (Ids == null || Ids.Count == 0)
            {
                return "请选择班级";
            }
            return string.Empty;
        }
    }
}
