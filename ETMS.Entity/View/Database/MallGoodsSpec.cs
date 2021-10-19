using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.Entity.View.Database
{
    public class MallGoodsSpec
    {
        public List<MallGoodsSpecItem> SpecItems { get; set; }
    }

    public class MallGoodsSpecItem
    {
        public string Name { get; set; }

        public List<string> Values { get; set; }
    }
}
