using ETMS.Entity.Pay.Lcsw.Dto.Request;
using ETMS.Entity.Pay.Lcsw.Dto.Request.Response;
using ETMS.Pay.Lcsw.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.Pay.Lcsw
{
    public class PayLcswService : IPayLcswService
    {
        /// <summary>
        /// 检查商户名称是否存在
        /// </summary>
        /// <param name="merchantName"></param>
        /// <returns></returns>
        public BaseResult CheckName(string merchantName)
        {
            var traceno = Guid.NewGuid().ToString("N");
            var param = new Dictionary<string, object>();
            param.Add("inst_no", Config._instNo);
            param.Add("trace_no", traceno);
            param.Add("merchant_name", merchantName);
            var data = new
            {
                inst_no = Config._instNo,
                trace_no = traceno,
                merchant_name = merchantName,
                key_sign = MD5Helper.GetSign(param, "key", Config._instToken),
            };
            return Post.PostGetJson<BaseResult>(ApiAddress.Checkname, data);
        }

        /// <summary>
        /// 添加商户
        /// </summary>
        /// <param name="requesParam"></param>
        /// <param name="requerParam1"></param>
        /// <returns></returns>
        public ResponseAddOrUpdateMerchant AddMerchant(RequestAddMerchant requesParam, RequestAddMerchantIsNull requerParam1)
        {
            var traceno = Guid.NewGuid().ToString("N");
            var param = new Dictionary<string, object>();
            param = MD5Helper.GetEntityToDictionary(requesParam);
            param.Add("trace_no", traceno);
            param.Add("inst_no", Config._instNo);
            var param1 = MD5Helper.GetEntityToDictionary(requerParam1);
            APICommonFun.DictionaryUnion(param, param1);
            param.Add("key_sign", MD5Helper.GetSign(param, "key", Config._instToken));
            return Post.PostGetJson<ResponseAddOrUpdateMerchant>(ApiAddress.AddMerchant, param);
        }

        /// <summary>
        /// 更新商户信息
        /// </summary>
        /// <param name="merchantNo"></param>
        /// <param name="requesParam"></param>
        /// <returns></returns>
        public ResponseAddOrUpdateMerchant UpdateMerchant(string merchantNo, RequestUpdateMerchant requesParam)
        {
            var traceno = Guid.NewGuid().ToString("N");
            var param = MD5Helper.GetEntityToDictionary(requesParam);
            param.Add("trace_no", traceno);
            param.Add("inst_no", Config._instNo);
            param.Add("merchant_no", merchantNo);
            param.Add("key_sign", MD5Helper.GetSign(param, "key", Config._instToken));
            return Post.PostGetJson<ResponseAddOrUpdateMerchant>(ApiAddress.UpdateMerchant, param);
        }

        /// <summary>
        /// 查询商户信息
        /// </summary>
        /// <param name="merchantNo"></param>
        /// <returns></returns>
        public ResponseQuerMerchant QuerMerchant(string merchantNo)
        {
            var traceno = Guid.NewGuid().ToString("N");
            var param = new Dictionary<string, object>();
            param.Add("inst_no", Config._instNo);
            param.Add("trace_no", traceno);
            param.Add("merchant_no", merchantNo);
            var data = new
            {
                inst_no = Config._instNo,
                trace_no = traceno,
                merchant_no = merchantNo,
                key_sign = MD5Helper.GetSign(param, "key", Config._instToken),
            };
            return Post.PostGetJson<ResponseQuerMerchant>(ApiAddress.QuerMerchant, data);
        }

        /// <summary>
        /// 查询清算
        /// </summary>
        /// <param name="requesParam"></param>
        /// <returns></returns>
        public ResponseGetSettlement GetSettlement(RequestGetSettlement requesParam)
        {
            var param = MD5Helper.GetEntityToDictionary(requesParam);
            var traceno = Guid.NewGuid().ToString("N");
            param.Add("version", "200");
            param.Add("trace_no", traceno);
            param.Add("inst_no", Config._instNo);
            param.Add("key_sign", MD5Helper.GetSign(param, "key", Config._instToken));
            return Post.PostGetJson<ResponseGetSettlement>(ApiAddress.GetSettlement, param);
        }

        /// <summary>
        /// 添加终端
        /// </summary>
        /// <param name="requesParam"></param>
        /// <returns></returns>
        public ResponseAddTermina AddTermina(RequestAddTermina requesParam)
        {
            var traceno = Guid.NewGuid().ToString("N");
            var param = new Dictionary<string, object>();
            param.Add("inst_no", Config._instNo);
            param.Add("trace_no", traceno);
            param.Add("merchant_no", requesParam.merchant_no);
            var data = new
            {
                inst_no = Config._instNo,
                trace_no = traceno,
                merchant_no = requesParam.merchant_no,
                store_code = string.Empty,
                key_sign = MD5Helper.GetSign(param, "key", Config._instToken),
            };
            return Post.PostGetJson<ResponseAddTermina>(ApiAddress.AddTermina, data);
        }

        /// <summary>
        /// 查询终端
        /// </summary>
        /// <param name="trace_no"></param>
        /// <param name="terminal_id"></param>
        /// <returns></returns>
        public ResponseQueryTermina QueryTermina(string trace_no, string terminal_id)
        {
            var param = new Dictionary<string, object>();
            param.Add("inst_no", Config._instNo);
            param.Add("trace_no", trace_no);
            param.Add("terminal_id", terminal_id);
            var data = new
            {
                inst_no = Config._instNo,
                trace_no = trace_no,
                terminal_id = terminal_id,
                key_sign = MD5Helper.GetSign(param, "key", Config._instToken),
            };
            return Post.PostGetJson<ResponseQueryTermina>(ApiAddress.QueryTermina, data);
        }

        /// <summary>
        /// 付款码支付
        /// </summary>
        /// <param name="requesParam"></param>
        /// <returns></returns>
        public ResponseBarcodePay BarcodePay(RequestBarcodePay requesParam)
        {
            var param = new Dictionary<string, object>
            {
                {"pay_ver", "110"},
                {"pay_type", requesParam.payType},
                {"service_id", "010"},
                {"merchant_no", requesParam.merchant_no},
                {"terminal_id", requesParam.terminal_id},
                {"terminal_trace", requesParam.terminal_trace},
                {"terminal_time", requesParam.terminal_time},
                {"auth_no", requesParam.auth_no},
                {"total_fee", requesParam.total_fee}
            };
            param.Add("key_sign", MD5Helper.GetSign(param, requesParam.access_token));
            param.Add("order_body", requesParam.order_body);
            param.Add("attach", requesParam.attach);
            return Post.PostGetJson<ResponseBarcodePay>(ApiAddress.BarcodePay, param);
        }

        /// <summary>
        /// 公众号预支付（统一下单）
        /// </summary>
        /// <param name="requesParam"></param>
        /// <returns></returns>
        public ResponseUnifiedOrder UnifiedOrder(RequestUnifiedOrder requesParam)
        {
            if (requesParam.payType == "010" || requesParam.payType == "020")
            {
                if (string.IsNullOrEmpty(requesParam.open_id))
                {
                    throw new Exception("用户标识不能为空");
                }
            }
            var param = new Dictionary<string, object>
            {
                {"pay_ver", "100"},
                {"pay_type", requesParam.payType},
                {"service_id", "012"},
                {"merchant_no", requesParam.merchant_no},
                {"terminal_id", requesParam.terminal_id},
                {"terminal_trace", requesParam.terminal_trace},
                {"terminal_time", requesParam.terminal_time},
                {"total_fee", requesParam.total_fee}
            };
            param.Add("key_sign", MD5Helper.GetSign(param, requesParam.access_token));
            //不参与签名的字符
            param.Add("sub_appid", requesParam.sub_appid);
            param.Add("open_id", requesParam.open_id);
            param.Add("order_body", requesParam.order_body);
            param.Add("notify_url", requesParam.notify_url);
            param.Add("attach", requesParam.attach);
            return Post.PostGetJson<ResponseUnifiedOrder>(ApiAddress.UnifiedOrder, param);
        }

        /// <summary>
        /// 支付订单查询
        /// </summary>
        /// <param name="requesParam"></param>
        /// <returns></returns>
        public ResponseQuery QueryPay(RequestQuery requesParam)
        {
            var param = new Dictionary<string, object>
            {
                {"pay_ver", "100"},
                {"pay_type", requesParam.payType},
                {"service_id", "020"},
                {"merchant_no", requesParam.merchant_no},
                {"terminal_id", requesParam.terminal_id},
                {"terminal_trace", requesParam.terminal_trace},
                {"terminal_time",requesParam.terminal_time},
                {"out_trade_no", requesParam.out_trade_no}
            };
            param.Add("key_sign", MD5Helper.GetSign(param, requesParam.access_token));
            return Post.PostGetJson<ResponseQuery>(ApiAddress.Query, param);
        }

        /// <summary>
        /// 退款申请
        /// 需要商户当前账户内有大于退款金额的余额，否则会造成余额不足，退款失败；
        /// 限支付30天内退款，超过30天，不能进行退款操作。
        /// </summary>
        /// <param name="requesParam"></param>
        /// <returns></returns>
        public ResponseRefund RefundPay(RequestRefund requesParam)
        {
            var param = new Dictionary<string, object>
            {
                {"pay_ver", "100"},
                {"pay_type", requesParam.payType},
                {"service_id", "030"},
                {"merchant_no", requesParam.merchant_no},
                {"terminal_id", requesParam.terminal_id},
                {"terminal_trace", requesParam.terminal_trace},
                {"terminal_time", requesParam.terminal_time},
                {"refund_fee", requesParam.refund_fee},
                {"out_trade_no",requesParam.out_trade_no }
            };
            param.Add("key_sign", MD5Helper.GetSign(param, requesParam.access_token));
            return Post.PostGetJson<ResponseRefund>(ApiAddress.Refund, param);
        }
    }
}
