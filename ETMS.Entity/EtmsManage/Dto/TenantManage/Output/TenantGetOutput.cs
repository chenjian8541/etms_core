using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.EtmsManage.Dto.TenantManage.Output
{
    public class TenantGetOutput
    {
        public int Id { get; set; }

        /// <summary>
        /// 联系人名称
        /// </summary>
        public string LinkMan { get; set; }

        /// <summary>
        /// 身份证号码
        /// </summary>
        public string IdCard { get; set; }

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

        public int VersionId { get; set; }

        public string VersionName { get; set; }

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
    }
}
