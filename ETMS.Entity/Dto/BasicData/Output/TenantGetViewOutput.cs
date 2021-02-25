using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.Dto.BasicData.Output
{
    public class TenantGetViewOutput
    {
        /// <summary>
        /// 机构ID
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// etms系统版本
        /// </summary>
        public int VersionId { get; set; }

        public string VersionName { get; set; }

        public string ExDateDesc { get; set; }

        public bool IsLimitExDate { get; set; }

        /// <summary>
        /// 短信数量
        /// </summary>
        public int SmsCount { get; set; }

        /// <summary>
        /// 限制用户数
        /// (0:不限制)
        /// </summary>
        public int MaxUserCount { get; set; }

        /// <summary>
        /// 状态 <see cref="ETMS.Entity.Enum.EtmsManage.EmSysTenantStatus"/>
        /// </summary>
        public byte Status { get; set; }

        public string StatusDesc { get; set; }

        /// <summary>
        /// 购买状态  <see cref="ETMS.Entity.Enum.EtmsManage.EmSysTenantBuyStatus"/>
        /// </summary>
        public byte BuyStatus { get; set; }

        public string BuyStatusDesc { get; set; }

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

        public string OtDesc { get; set; }

        /// <summary>
        /// 联系人名称
        /// </summary>
        public string LinkMan { get; set; }

        /// <summary>
        /// 地址
        /// </summary>
        public string Address { get; set; }

        /// <summary>
        /// 短信签名
        /// </summary>
        public string SmsSignature { get; set; }
    }
}
