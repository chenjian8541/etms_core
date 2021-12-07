using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.Entity.Dto.CoreBusiness.Request
{
    public class WxConfigRequest
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

        /// <summary>
        /// 服务商AppSecret
        /// </summary>
        public string VendorSecret { get; set; }

        public int? MerchantId { get; set; }

        public int? StoreId { get; set; }

        public int? CashierId { get; set; }

        public string WxSubAppid { get; set; }

        public string JsapiPath { get; set; }
    }
}
