using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.Entity.EtmsManage.Dto.Head.Output
{
    public class HeadGetPagingOutput
    {
        public long CId { get; set; }

        /// <summary>
        /// 机构数量
        /// </summary>
        public int TenantCount { get; set; }

        /// <summary>
        /// 企业编码
        /// </summary>
        public string HeadCode { get; set; }

        /// <summary>
        /// 企业名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 企业地址
        /// </summary>
        public string Address { get; set; }

        /// <summary>
        /// 企业地址
        /// </summary>
        public string LinkMan { get; set; }

        /// <summary>
        /// 企业联系人手机号码
        /// </summary>
        public string Phone { get; set; }

        /// <summary>
        /// 注册时间
        /// </summary>
        public DateTime Ot { get; set; }

        /// <summary>
        /// 状态  <see cref="ETMS.Entity.Enum.Alien.EmMgHeadStatus"/>
        /// </summary>
        public byte Status { get; set; }

        public string StatusDesc { get; set; }

        public string AgentName { get; set; }
    }
}
