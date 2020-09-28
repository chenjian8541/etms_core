using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.Dto.BasicData.Output
{
    public class GiftCategoryGetOutput
    {
        public List<GiftCategoryViewOutput> GiftCategorys { get; set; }
    }

    public class GiftCategoryViewOutput
    {
        public long CId { get; set; }

        public string Name { get; set; }

        public string Remark { get; set; }

        public string Label { get; set; }

        public long Value { get; set; }
    }
}
