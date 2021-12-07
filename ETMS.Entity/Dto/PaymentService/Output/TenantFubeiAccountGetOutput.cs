using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.Entity.Dto.PaymentService.Output
{
    public class TenantFubeiAccountGetOutput
    {
        /// <summary>
        /// 聚合支付状态类型 <see cref="ETMS.Entity.Enum.EtmsManage.EmAgtPayType"/>
        /// </summary>
        public int AgtPayType { get; set; }

        public TenantFubeiAccountInfo AccountInfo { get; set; }
    }

    public class TenantFubeiAccountInfo
    {
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

        public string MerchantName { get; set; }

        public string MerchantCode { get; set; }

        public int? MerchantId { get; set; }

        public int? StoreId { get; set; }

        public int? CashierId { get; set; }

        public string WxSubAppid { get; set; }

        /// <summary>
        /// <see cref="ETMS.Entity.Enum.EtmsManage.EmTenantFubeiAccountApplyStatus"/>
        /// </summary>
        public byte ApplyStatus { get; set; }
    }
}
