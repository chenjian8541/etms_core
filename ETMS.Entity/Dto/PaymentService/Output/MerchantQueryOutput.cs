using ETMS.Entity.Pay.Lcsw.Dto.Request.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.Entity.Dto.PaymentService.Output
{
    public class MerchantQueryOutput
    {
        /// <summary>
        /// 利楚扫呗申请状态
        /// <see cref="ETMS.Entity.Enum.EtmsManage.EmLcswApplyStatus"/>
        /// </summary>
        public int LcswApplyStatus { get; set; }

        public string TenantNo { get; set; }

        public string UserNo { get; set; }

        /// <summary>
        /// 申请信息
        /// </summary>
        public MerchantInfoOutput MerchantInfo { get; set; }
    }
}
