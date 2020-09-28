using ETMS.Entity.Dto.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.Dto.Marketing.Output
{
    public class GiftGetPagingOutput
    {
        public long CId { get; set; }

        public string Name { get; set; }

        public int Nums { get; set; }

        public bool IsLimitNums { get; set; }

        public List<Img> Imgs { get; set; }

        public int Points { get; set; }

        public long Value { get; set; }

        public string Label { get; set; }
    }
}
