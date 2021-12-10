using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace ETMS.Entity.Database.Manage
{
    [Table("SysTenantFubeiAccount")]
    public class SysTenantFubeiAccount : EManageEntity<long>
    {
        public int TenantId { get; set; }

        public int AgentId { get; set; }

        /// <summary>
        /// <see cref="ETMS.Entity.Enum.EtmsManage.EmFubeiAccountType"/>
        /// </summary>
        public byte AccountType { get; set; }

        /// <summary>
        /// 商户Api AppId
        /// </summary>
        public string AppId { get; set; }

        /// <summary>
        /// 商户Api AppSecret
        /// </summary>
        public string AppSecret { get; set; }

        /// <summary>
        /// 服务商SN
        /// </summary>
        public string VendorSn { get; set; }

        /// <summary>
        /// 服务商AppSecret
        /// </summary>
        public string VendorSecret { get; set; }

        public string MerchantName { get; set; }

        public string MerchantCode { get; set; }

        public int? MerchantId { get; set; }

        public int? StoreId { get; set; }

        public int? CashierId { get; set; }

        public string WxSubAppid { get; set; }

        public string WxJsapiPath { get; set; }

        /// <summary>
        /// <see cref="ETMS.Entity.Enum.EtmsManage.EmTenantFubeiAccountApplyStatus"/>
        /// </summary>
        public int ApplyStatus { get; set; }

        public DateTime CreationTime { get; set; }

        public DateTime ChangeTime { get; set; }
    }
}
