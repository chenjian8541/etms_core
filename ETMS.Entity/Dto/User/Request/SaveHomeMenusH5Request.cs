using ETMS.Entity.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.Entity.Dto.User.Request
{
    public class SaveHomeMenusH5Request : RequestBase
    {
        public int[] Ids { get; set; }

        public override string Validate()
        {
            if (Ids == null || Ids.Length == 0)
            {
                return "首页至少要添加一个应用";
            }
            if (Ids.Length > 20)
            {
                return "首页最多添加20个应用";
            }
            return base.Validate();
        }
    }
}
