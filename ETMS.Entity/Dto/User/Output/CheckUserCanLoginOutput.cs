using ETMS.Entity.View.Role;
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

        /// <summary>
        /// 聚合支付状态类型 <see cref="ETMS.Entity.Enum.EtmsManage.EmAgtPayType"/>
        /// </summary>
        public int AgtPayType { get; set; }

        public AuthorityValueDataDetailView AuthorityValueDataBag { get; set; }

        public SecrecyDataView SecrecyDataBag { get; set; }
    }
}
