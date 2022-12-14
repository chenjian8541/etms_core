using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.Entity.Dto.PaymentService.Output
{
    public class TenantSuixingAccountGet2Output
    {
        /// <summary>
        /// 聚合支付状态类型 <see cref="ETMS.Entity.Enum.EtmsManage.EmAgtPayType"/>
        /// </summary>
        public int AgtPayType { get; set; }

        public TenantSuixingAccountGet2OutputInfo AccountInfo { get; set; }
    }

    public class TenantSuixingAccountGet2OutputInfo
    {
        public string Mno { get; set; }

        public string MerName { get; set; }

        public string MecDisNm { get; set; }

        public string MblNo { get; set; }

        public string OperationalType { get; set; }

        public string HaveLicenseNo { get; set; }

        public string MecTypeFlag { get; set; }

        public string ParentMno { get; set; }

        public string IndependentModel { get; set; }

        public string MerchantStatus { get; set; }

        public string Email { get; set; }

        public string OnlineName { get; set; }

        public string CprRegAddr { get; set; }

        public string IdentityName { get; set; }

        public string ActNm { get; set; }

        public string LbnkNm { get; set; }

        /// <summary>
        /// <see cref="ETMS.Entity.Enum.EtmsManage.EmTenantSuixingAccountStatus"/>
        /// </summary>
        public int Status { get; set; }
    }
}
