using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.Entity.Dto.User.Output
{
    public class GetAllMenusH5Output
    {
        public List<GetAllMenusH5Category> AllCategorys { get; set; }
    }

    public class GetAllMenusH5Category
    {
        public int CategoryId { get; set; }

        public string CategoryName { get; set; }

        public IEnumerable<MenuH5Output> MyMenus { get; set; }
    }
}
