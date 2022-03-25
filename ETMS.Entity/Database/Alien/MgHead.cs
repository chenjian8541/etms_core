using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace ETMS.Entity.Database.Alien
{
    [Table("MgHead")]
    public class MgHead: EAlienEntityBase<int>
    {
        /// <summary>
        /// 代理商ID
        /// </summary>
        public int AgentId { get; set; }

        /// <summary>
        /// 总部机构ID
        /// </summary>
        public int TenantId { get; set; }

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
    }
}
