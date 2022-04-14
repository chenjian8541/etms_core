using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.Entity.EtmsManage.Dto.Head.Output
{
    public class HeadGetOutput
    {
        public HeadBascInfoOutput HeadBascInfo { get; set; }

        public List<HeadTenantOutput> HeadTenants { get; set; }
    }

    public class HeadBascInfoOutput {
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
    }

    public class HeadTenantOutput {

        public int Id { get; set; }

        /// <summary>
        /// 联系人名称
        /// </summary>
        public string LinkMan { get; set; }

        /// <summary>
        /// 地址
        /// </summary>
        public string Address { get; set; }

        /// <summary>
        /// 机构名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 机构代码
        /// </summary>
        public string TenantCode { get; set; }

        /// <summary>
        /// 机构电话
        /// </summary>
        public string Phone { get; set; }

     
        public string ExDateDesc { get; set; }

        public int SmsCount { get; set; }

        /// <summary>
        /// <see cref="ETMS.Entity.Enum.EtmsManage.EmSysTenantStatus"/>
        /// </summary>
        public byte Status { get; set; }

        /// <summary>
        /// 购买状态  <see cref="ETMS.Entity.Enum.EtmsManage.EmSysTenantBuyStatus"/>
        /// </summary>
        public byte BuyStatus { get; set; }

        public string Remark { get; set; }

        public string SmsSignature { get; set; }

        public string StatusDesc { get; set; }

        public string BuyStatusDesc { get; set; }

    }
}
