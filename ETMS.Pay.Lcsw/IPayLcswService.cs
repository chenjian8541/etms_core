using ETMS.Entity.Pay.Lcsw.Dto.Request;
using ETMS.Entity.Pay.Lcsw.Dto.Request.Response;

namespace ETMS.Pay.Lcsw
{
    public interface IPayLcswService
    {
        /// <summary>
        /// 检查商户名称是否存在
        /// </summary>
        /// <param name="merchantName"></param>
        /// <returns></returns>
        BaseResult CheckName(string merchantName);

        /// <summary>
        /// 添加商户
        /// </summary>
        /// <param name="requesParam"></param>
        /// <param name="requerParam1"></param>
        /// <returns></returns>
        ResponseAddOrUpdateMerchant AddMerchant(RequestAddMerchant requesParam, RequestAddMerchantIsNull requerParam1);

        /// <summary>
        /// 更新商户信息
        /// </summary>
        /// <param name="merchantNo"></param>
        /// <param name="requesParam"></param>
        /// <returns></returns>
        ResponseAddOrUpdateMerchant UpdateMerchant(string merchantNo, RequestUpdateMerchant requesParam);

        /// <summary>
        /// 查询商户信息
        /// </summary>
        /// <param name="merchantNo"></param>
        /// <returns></returns>
        ResponseQuerMerchant QuerMerchant(string merchantNo);

        /// <summary>
        /// 查询清算
        /// </summary>
        /// <param name="requesParam"></param>
        /// <returns></returns>
        ResponseGetSettlement GetSettlement(RequestGetSettlement requesParam);

        /// <summary>
        /// 添加终端
        /// </summary>
        /// <param name="requesParam"></param>
        /// <returns></returns>
        ResponseAddTermina AddTermina(RequestAddTermina requesParam);

        /// <summary>
        /// 查询终端
        /// </summary>
        /// <param name="trace_no"></param>
        /// <param name="terminal_id"></param>
        /// <returns></returns>
        ResponseQueryTermina QueryTermina(string trace_no, string terminal_id);

        /// <summary>
        /// 付款码支付
        /// </summary>
        /// <param name="requesParam"></param>
        /// <returns></returns>
        ResponseBarcodePay BarcodePay(RequestBarcodePay requesParam);

        /// <summary>
        /// 公众号预支付（统一下单）
        /// </summary>
        /// <param name="requesParam"></param>
        /// <returns></returns>
        ResponseUnifiedOrder UnifiedOrder(RequestUnifiedOrder requesParam);

        /// <summary>
        /// 支付订单查询
        /// </summary>
        /// <param name="requesParam"></param>
        /// <returns></returns>
        ResponseQuery Query(RequestQuery requesParam);

        /// <summary>
        /// 退款申请
        /// 需要商户当前账户内有大于退款金额的余额，否则会造成余额不足，退款失败；
        /// 限支付30天内退款，超过30天，不能进行退款操作。
        /// </summary>
        /// <param name="requesParam"></param>
        /// <returns></returns>
        ResponseRefund Refund(RequestRefund requesParam);
    }
}
