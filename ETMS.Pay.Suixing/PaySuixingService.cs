using ETMS.Pay.Suixing.Utility;
using ETMS.Pay.Suixing.Utility.Dto;
using ETMS.Pay.Suixing.Utility.Dto.Request;
using ETMS.Pay.Suixing.Utility.Dto.Response;
using ETMS.Pay.Suixing.Utility.ExternalDto.Request;

namespace ETMS.Pay.Suixing
{
    public class PaySuixingService : IPaySuixingService
    {
        public MerchantInfoQueryResponse MerchantInfoQuery(string mno)
        {
            var req = new RequestBase<MerchantInfoQueryRequest>()
            {
                orgId = Config._orgId,
                reqId = Com.GetReqId(),
                timestamp = Com.GetTimestamp(),
                reqData = new MerchantInfoQueryRequest()
                {
                    mno = mno
                }
            };
            string strSignTemp = PackReflectionEntity<RequestBase<MerchantInfoQueryRequest>>.GetEntityToString(req);
            var strSignResult = RSAUtil.RSASign(strSignTemp, Config._privateKeyPem);
            req.sign = strSignResult;
            var result = Post.PostGetJson<ResponseBase<MerchantInfoQueryResponse>>(Config._merchantInfoQuery, req);
            if (result.code == EmResponseCode.Success)
            {
                return result.respData;
            }
            return null;
        }

        public JsapiScanResponse JsapiScanMiniProgram(JsapiScanMiniProgramReq request)
        {
            var req = new RequestBase<JsapiScanRequest>()
            {
                orgId = Config._orgId,
                reqId = Com.GetReqId(),
                timestamp = Com.GetTimestamp(),
                reqData = new JsapiScanRequest()
                {
                    ordNo = request.ordNo,
                    amt = request.amt.ToString("F2"),
                    mno = request.mno,
                    notifyUrl = request.notifyUrl,
                    payType = EmPayType.WeChat,
                    payWay = "03",
                    subAppid = Config._subAppidMiniProgram,
                    subject = request.subject,
                    trmIp = request.trmIp,
                    userId = request.openid,
                    extend = request.extend
                }
            };
            string strSignTemp = PackReflectionEntity<RequestBase<JsapiScanRequest>>.GetEntityToString(req);
            var strSignResult = RSAUtil.RSASign(strSignTemp, Config._privateKeyPem);
            req.sign = strSignResult;
            var result = Post.PostGetJson<ResponseBase<JsapiScanResponse>>(Config._jsapiScan, req);
            if (result.code == EmResponseCode.Success)
            {
                return result.respData;
            }
            return null;
        }

        public TradeQueryResponse TradeQuery(string mno, string ordNo)
        {
            var req = new RequestBase<TradeQueryRequest>()
            {
                orgId = Config._orgId,
                reqId = Com.GetReqId(),
                timestamp = Com.GetTimestamp(),
                reqData = new TradeQueryRequest()
                {
                    mno = mno,
                    ordNo = ordNo
                }
            };
            string strSignTemp = PackReflectionEntity<RequestBase<TradeQueryRequest>>.GetEntityToString(req);
            var strSignResult = RSAUtil.RSASign(strSignTemp, Config._privateKeyPem);
            req.sign = strSignResult;
            var result = Post.PostGetJson<ResponseBase<TradeQueryResponse>>(Config._tradeQuery, req);
            if (result.code == EmResponseCode.Success)
            {
                return result.respData;
            }
            return null;
        }

        public RefundResponse Refund(RefundReq request)
        {
            var req = new RequestBase<RefundRequest>()
            {
                orgId = Config._orgId,
                reqId = Com.GetReqId(),
                timestamp = Com.GetTimestamp(),
                reqData = new RefundRequest()
                {
                    amt = request.amt.ToString("F2"),
                    extend = request.extend,
                    mno = request.mno,
                    notifyUrl = request.notifyUrl,
                    ordNo = request.ordNo,
                    origOrderNo = request.origOrderNo,
                    refundReason = request.refundReason
                }
            };
            string strSignTemp = PackReflectionEntity<RequestBase<RefundRequest>>.GetEntityToString(req);
            var strSignResult = RSAUtil.RSASign(strSignTemp, Config._privateKeyPem);
            req.sign = strSignResult;
            var result = Post.PostGetJson<ResponseBase<RefundResponse>>(Config._refund, req);
            if (result.code == EmResponseCode.Success)
            {
                return result.respData;
            }
            return null;
        }

        public RefundQueryResponse RefundQuery(string mno, string outOrdNo)
        {
            var req = new RequestBase<RefundQueryRequest>()
            {
                orgId = Config._orgId,
                reqId = Com.GetReqId(),
                timestamp = Com.GetTimestamp(),
                reqData = new RefundQueryRequest()
                {
                    mno = mno,
                    ordNo = outOrdNo
                }
            };
            string strSignTemp = PackReflectionEntity<RequestBase<RefundQueryRequest>>.GetEntityToString(req);
            var strSignResult = RSAUtil.RSASign(strSignTemp, Config._privateKeyPem);
            req.sign = strSignResult;
            var result = Post.PostGetJson<ResponseBase<RefundQueryResponse>>(Config._merchantInfoQuery, req);
            if (result.code == EmResponseCode.Success)
            {
                return result.respData;
            }
            return null;
        }
    }
}