using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.Entity.Dto.User.Output
{
    public class CheckUserCanLoginOutput
    {
        /// <summary>
        /// 是否启用数据限制
        /// </summary>
        public bool IsDataLimit { get; set; }

        /// <summary>
        /// 隐私类型 <see cref="ETMS.Entity.Enum.EmRoleSecrecyType"/>
        /// </summary>
        public int SecrecyType { get; set; }
    }
}
