using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.Entity.Dto.OpenApi99.Output
{
    public class TenantLcsAccountGetOutput
    {
        /// <summary>
        /// <see cref="ETMS.Entity.Enum.EtmsManage.EmLcsMerchanttype"/>
        /// </summary>
        public int MerchantType { get; set; }

        public string MerchantName { get; set; }

        public string MerchantCompany { get; set; }

        public string MerchantStatus { get; set; }

        public string TerminalName { get; set; }

        public string MerchantInfoData { get; set; }

        public string MerchantRquestData { get; set; }

        /// <summary>
        /// 利楚扫呗申请状态
        /// <see cref="ETMS.Entity.Enum.EtmsManage.EmLcswApplyStatus"/>
        /// </summary>
        public int LcswApplyStatus { get; set; }

        public string ReturnCode { get; set; }

        public string ReturnMsg { get; set; }

        public string TraceNo { get; set; }

        public string ResultCode { get; set; }

        public string InstNo { get; set; }

        public string MerchantNo { get; set; }

        public string StoreCode { get; set; }

        public string TerminalId { get; set; }

        public string AccessToken { get; set; }

        public DateTime CreationTime { get; set; }

        public DateTime? ReviewTime { get; set; }

        public DateTime ChangeTime { get; set; }
    }
}
