using ETMS.Entity.Common;
using ETMS.Entity.Enum.EtmsManage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.Entity.Dto.PaymentService.Request
{
    public class TenantFubeiAccountBindRequest : RequestBase
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

        public string MerchantName { get; set; }

        public string MerchantCode { get; set; }

        public int? MerchantId { get; set; }

        public int? StoreId { get; set; }

        public int? CashierId { get; set; }

        public string WxSubAppid { get; set; }

        public override string Validate()
        {
            if (AccountType == EmFubeiAccountType.Merchant)
            {
                //商户级
                if (string.IsNullOrEmpty(AppId))
                {
                    return "请输入商户AppId";
                }
                if (string.IsNullOrEmpty(AppSecret))
                {
                    return "请输入商户AppSecret";
                }
            }
            else
            {
                //服务商级
                if (MerchantId == null || MerchantId <= 0)
                {
                    return "请输入MerchantId";
                }
            }
            if (string.IsNullOrEmpty(WxSubAppid))
            {
                return "请输入SubAppid";
            }
            if (StoreId == null || StoreId <= 0)
            {
                return "请输入StoreId";
            }
            return base.Validate();
        }
    }
}
