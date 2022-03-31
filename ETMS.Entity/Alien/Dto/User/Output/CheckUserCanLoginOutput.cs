using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.Entity.Alien.Dto.User.Output
{
    public class CheckUserCanLoginOutput
    {
        /// <summary>
        /// 机构ID集合
        /// </summary>
        public List<int> AllTenants { get; set; }
    }
}
