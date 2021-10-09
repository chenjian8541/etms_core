using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.Entity.Dto.User.Output
{
    public class GetEditMenusH5Output
    {
        public List<MenuH5Output> HomeMenus { get; set; }

        public List<AllMenuH5Output> AllMenus { get; set; }
    }

    public class AllMenuH5Output : MenuH5Output
    {
        public bool IsHome { get; set; }
    }
}
