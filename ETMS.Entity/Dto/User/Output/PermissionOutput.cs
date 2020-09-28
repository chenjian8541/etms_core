using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.Dto.User.Output
{
   public class PermissionOutput
    {
        public List<int> Page { get; set; }

        public List<int> Action { get; set; }
    }
}
