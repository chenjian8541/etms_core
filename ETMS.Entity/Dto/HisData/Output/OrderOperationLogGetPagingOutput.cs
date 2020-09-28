using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.Dto.HisData.Output
{
    public class OrderOperationLogGetPagingOutput
    {
        public long CId { get; set; }

        public DateTime Ot { get; set; }

        public int OpType { get; set; }

        public string OpTypeDesc { get; set; }

        public string OpContent { get; set; }

        public long UserId { get; set; }

        public string UserDesc { get; set; }
    }
}
