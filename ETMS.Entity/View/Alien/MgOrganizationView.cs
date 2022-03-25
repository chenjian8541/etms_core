using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.Entity.View.Alien
{
    public class MgOrganizationView
    {
        public long Id { get; set; }

        public long ParentId { get; set; }

        public string Name { get; set; }

        public string ParentsAll { get; set; }

        public string UserCount { get; set; }

        public string Remark { get; set; }

        public List<MgOrganizationView> Child { get; set; }
    }
}
