using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.Dto.BasicData.Output
{
    public class ClassCategoryGetOutput
    {
        public List<ClassCategoryViewOutput> ClassCategorys { get; set; }
    }

    public class ClassCategoryViewOutput
    {
        public long CId { get; set; }

        public string Name { get; set; }

        public string Remark { get; set; }
    }
}
