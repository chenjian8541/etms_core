using ETMS.Entity.Dto.CoreBusiness.Output;
using ETMS.Entity.Dto.CoreBusiness.Request;
using ETMS.Entity.Temp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.IBusiness
{
    /// <summary>
    /// 聚合支付服务
    /// </summary>
    public interface IAgtPayServiceBLL : IBaseBLL
    {
        void Initialize(CheckTenantLcsAccountView checkResult);

        Task<WxConfigOutput> WxConfig(WxConfigRequest request);

        Task<CallbackConfigOutput> CallbackConfig(CallbackConfigRequest request);

        Task<BarcodePayOutput> BarcodePay(BarcodePayRequest request);

        Task<UnifiedOrderOutput> UnifiedOrder(UnifiedOrderRequest request);

        Task<QueryPayLogOutput> QueryPayLog(QueryPayLogRequest request);

        Task<RefundPayOutput> RefundPay(RefundPayRequest request);
    }
}
