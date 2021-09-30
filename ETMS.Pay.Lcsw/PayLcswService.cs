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
            var key_sign = new Dictionary<string, string>();
            key_sign.Add("inst_no", requesParam.inst_no);
            key_sign.Add("version", "200");
            key_sign.Add("trace_no", requesParam.trace_no);
            key_sign.Add("merchant_no", requesParam.merchant_no);
            key_sign.Add("begin_date", requesParam.begin_date);
            key_sign.Add("end_date", requesParam.end_date);
            var param = from obj in key_sign
                        orderby obj.Key ascending
                        select obj;
            var str = new StringBuilder();
            foreach (var kv in param)
            {
                str.Append(kv.Key + "=" + kv.Value + "&");
            }
            string sign = string.Format("{0}key={1}", str.ToString(), Config._instToken);
            var data = new
            {
                inst_no = requesParam.inst_no,
                version = "200",
                trace_no = requesParam.trace_no,
                merchant_no = requesParam.merchant_no,
                begin_date = requesParam.begin_date,
                end_date = requesParam.end_date,
                key_sign = MD5Helper.GetSign(sign),
            };
            return Post.PostGetJson<ResponseGetSettlement>(ApiAddress.GetSettlement, data);
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
            var param = new StringBuilder();
            param.Append("pay_ver=100");
            param.AppendFormat("&pay_type={0}", requesParam.payType);
            param.Append("&service_id=010");
            param.AppendFormat("&merchant_no={0}", requesParam.merchant_no);
            param.AppendFormat("&terminal_id={0}", requesParam.terminal_id);
            param.AppendFormat("&terminal_trace={0}", requesParam.terminal_trace);
            param.AppendFormat("&terminal_time={0}", requesParam.terminal_time);
            param.AppendFormat("&auth_no={0}", requesParam.auth_no);
            param.AppendFormat("&total_fee={0}", requesParam.total_fee);
            //拼接签名
            var key_sign = string.Format("{0}&access_token={1}", param.ToString(), requesParam.accessToken);

            var data = new
            {
                pay_ver = "100",
                pay_type = requesParam.payType,
                service_id = "010",
                merchant_no = requesParam.merchant_no,
                terminal_id = requesParam.terminal_id,
                terminal_trace = requesParam.terminal_trace,
                terminal_time = requesParam.terminal_time,
                auth_no = requesParam.auth_no,
                total_fee = requesParam.total_fee,
                order_body = requesParam.order_body,
                attach = requesParam.attach,
                goods_detail = requesParam.goods_detail,
                key_sign = MD5Helper.GetSign(key_sign),
            };
            return Post.PostGetJson<ResponseBarcodePay>(ApiAddress.BarcodePay, data);
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
            var param = new Dictionary<string, object>();
            param.Add("pay_ver", "100");
            param.Add("pay_type", requesParam.payType);
            param.Add("service_id", "012");
            param.Add("merchant_no", requesParam.merchant_no);
            param.Add("terminal_id", requesParam.terminal_id);
            param.Add("terminal_trace", requesParam.terminal_trace);
            param.Add("terminal_time", requesParam.terminal_time);
            param.Add("total_fee", requesParam.total_fee);
            //拼接签名
            param.Add("key_sign", MD5Helper.GetSignNoSort(param, "access_token", requesParam.accessToken));
            //不参与签名的字符
            param.Add("open_id", requesParam.open_id);
            param.Add("order_body", requesParam.order_body);
            param.Add("notify_url", requesParam.notify_url);
            param.Add("attach", requesParam.attach);
            param.Add("goods_detail", requesParam.goods_detail);
            return Post.PostGetJson<ResponseUnifiedOrder>(ApiAddress.UnifiedOrder, param);
        }

        /// <summary>
        /// 支付订单查询
        /// </summary>
        /// <param name="requesParam"></param>
        /// <returns></returns>
        public ResponseQuery Query(RequestQuery requesParam)
        {
            var param = new StringBuilder();
            param.Append("pay_ver=100");
            param.AppendFormat("&pay_type={0}", requesParam.payType);
            param.Append("&service_id=020");
            param.AppendFormat("&merchant_no={0}", requesParam.merchant_no);
            param.AppendFormat("&terminal_id={0}", requesParam.terminal_id);
            param.AppendFormat("&terminal_trace={0}", requesParam.terminal_trace);
            param.AppendFormat("&terminal_time={0}", requesParam.terminal_time);
            param.AppendFormat("&out_trade_no={0}", requesParam.out_trade_no);
            var key_sign = string.Format("{0}&access_token={1}", param.ToString(), requesParam.accessToken);
            var data = new
            {
                pay_ver = "100",
                pay_type = requesParam.payType,
                service_id = "020",
                merchant_no = requesParam.merchant_no,
                terminal_id = requesParam.terminal_id,
                terminal_trace = requesParam.terminal_trace,
                terminal_time = requesParam.terminal_time,
                out_trade_no = requesParam.out_trade_no,
                key_sign = MD5Helper.GetSign(key_sign),
            };

            return Post.PostGetJson<ResponseQuery>(ApiAddress.Query, data);
        }

        /// <summary>
        /// 退款申请
        /// 需要商户当前账户内有大于退款金额的余额，否则会造成余额不足，退款失败；
        /// 限支付30天内退款，超过30天，不能进行退款操作。
        /// </summary>
        /// <param name="requesParam"></param>
        /// <returns></returns>
        public ResponseRefund Refund(RequestRefund requesParam)
        {
            var param = new StringBuilder();
            param.Append("pay_ver=100");
            param.AppendFormat("&pay_type={0}", requesParam.payType);
            param.Append("&service_id=030");
            param.AppendFormat("&merchant_no={0}", requesParam.merchant_no);
            param.AppendFormat("&terminal_id={0}", requesParam.terminal_id);
            param.AppendFormat("&terminal_trace={0}", requesParam.terminal_trace);
            param.AppendFormat("&terminal_time={0}", requesParam.terminal_time);
            param.AppendFormat("&refund_fee={0}", requesParam.refund_fee);
            param.AppendFormat("&out_trade_no={0}", requesParam.out_trade_no);
            string key_sign = string.Format("{0}&access_token={1}", param.ToString(), requesParam.accessToken);

            var data = new
            {
                pay_ver = "100",
                pay_type = requesParam.payType,
                service_id = "030",
                merchant_no = requesParam.merchant_no,
                terminal_id = requesParam.terminal_id,
                terminal_trace = requesParam.terminal_trace,
                terminal_time = requesParam.terminal_time,
                refund_fee = requesParam.refund_fee,
                out_trade_no = requesParam.out_trade_no,
                pay_trace = requesParam.pay_trace,
                pay_time = requesParam.pay_time,
                auth_code = requesParam.auth_code,
                key_sign = MD5Helper.GetSign(key_sign),
            };
            return Post.PostGetJson<ResponseRefund>(ApiAddress.Refund, data);
        }
    }
}
