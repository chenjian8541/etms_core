using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.Entity.Alien.Dto.User.Output
{
    public class RoleListGetOutput
    {
        public List<RoleListView> RoleLists { get; set; }
    }

    public class RoleListView
    {
        public long CId { get; set; }

        public string Name { get; set; }

        public string Remark { get; set; }

        public string DataLimitDesc { get; set; }

        public long Value { get; set; }

        public string Label { get; set; }
    }
}
