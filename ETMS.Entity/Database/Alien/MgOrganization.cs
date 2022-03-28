using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace ETMS.Entity.Database.Alien
{
    [Table("MgOrganization")]
    public class MgOrganization: EAlienEntity<long>
    {
        public long ParentId { get; set; }

        public string Name { get; set; }

        public string ParentsAll { get; set; }

        public int UserCount { get; set; }

        public string Remark { get; set; }
    }
}
