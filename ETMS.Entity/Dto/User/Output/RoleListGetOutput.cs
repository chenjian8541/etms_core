using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.Dto.User.Output
{
    public class RoleListGetOutput
    {
        public List<RoleListViewOutput> RoleLists { get; set; }
    }

    public class RoleListViewOutput
    {
        public long CId { get; set; }

        public string Name { get; set; }

        public string Remark { get; set; }

        public string DataLimitDesc { get; set; }

        public long Value { get; set; }

        public string Label { get; set; }
    }
}
