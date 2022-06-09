using ETMS.Pay.Suixing.Utility.Dto.Response;
using ETMS.Pay.Suixing.Utility.ExternalDto.Request;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.Pay.Suixing
{
    public interface IPaySuixingService
    {
        MerchantInfoQueryResponse MerchantInfoQuery(string mno);

        JsapiScanResponse JsapiScanMiniProgram(JsapiScanMiniProgramReq request);

        TradeQueryResponse TradeQuery(string mno, string ordNo);

        RefundResponse Refund(RefundReq request);

        RefundQueryResponse RefundQuery(string mno, string outOrdNo);
    }
}
