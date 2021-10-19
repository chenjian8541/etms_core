using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.Entity.View.Database
{
    public class MallGoodsTag
    {
        public List<MallGoodsTagItem> Items { get; set; }
    }

    public class MallGoodsTagItem
    {
        public string Name { get; set; }

        public string StyleColor { get; set; }
    }
}
