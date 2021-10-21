using ETMS.Business.BaseBLL;
using ETMS.Business.Common;
using ETMS.Entity.Common;
using ETMS.Entity.Config;
using ETMS.Entity.Database.Source;
using ETMS.Entity.Dto.Parent3.Request;
using ETMS.Entity.Dto.Parent4.Output;
using ETMS.Entity.Enum;
using ETMS.Entity.Enum.EtmsManage;
using ETMS.Entity.Pay.Lcsw.Dto.Request;
using ETMS.Entity.Temp;
using ETMS.IBusiness.Parent;
using ETMS.IBusiness.Wechart;
using ETMS.IDataAccess.EtmsManage;
using ETMS.IDataAccess.Lcs;
using ETMS.IDataAccess.MallGoodsDAL;
using ETMS.LOG;
using ETMS.Pay.Lcsw;
using ETMS.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.Business.Parent
{
    public class ParentData4BLL : TenantLcsAccountBLL, IParentData4BLL
    {
        private readonly IPayLcswService _payLcswService;

        private readonly IMallGoodsDAL _mallGoodsDAL;

        private readonly ITenantLcsPayLogDAL _tenantLcsPayLogDAL;

        private readonly IComponentAccessBLL _componentAccessBLL;

        public ParentData4BLL(ISysTenantDAL sysTenantDAL, ITenantLcsAccountDAL tenantLcsAccountDAL,
            IPayLcswService payLcswService, IMallGoodsDAL mallGoodsDAL, ITenantLcsPayLogDAL tenantLcsPayLogDAL,
            IComponentAccessBLL componentAccessBLL) : base(tenantLcsAccountDAL, sysTenantDAL)
        {
            this._payLcswService = payLcswService;
            this._mallGoodsDAL = mallGoodsDAL;
            this._tenantLcsPayLogDAL = tenantLcsPayLogDAL;
            this._componentAccessBLL = componentAccessBLL;
        }

        public void InitTenantId(int tenantId)
        {
            this.InitDataAccess(tenantId, _mallGoodsDAL, _tenantLcsPayLogDAL);
        }

        private CheckBuyMallGoodsResult CheckBuyMallGoods(EtMallGoods mallGoods, List<EtMallCoursePriceRule> mallCoursePriceRules, int buyCount,
            long? coursePriceRuleId)
        {
            if (mallGoods.ProductType == EmProductType.Course)
            {
                if (coursePriceRuleId == null || coursePriceRuleId.Value <= 0)
                {
                    return new CheckBuyMallGoodsResult("请选择课程收费方式");
                }
                if (mallCoursePriceRules == null || mallCoursePriceRules.Count == 0)
                {
                    return new CheckBuyMallGoodsResult("课程未设置收费方式");
                }
                var priceRule = mallCoursePriceRules.FirstOrDefault(p => p.Id == coursePriceRuleId);
                if (priceRule == null)
                {
                    return new CheckBuyMallGoodsResult("课程收费方式错误");
                }
                var buyQuantity = priceRule.Quantity > 1 ? priceRule.Quantity : buyCount;
                var itemSum = priceRule.Quantity > 1 ? priceRule.TotalPrice : (buyQuantity * priceRule.Price).EtmsToRound();
                return new CheckBuyMallGoodsResult()
                {
                    TotalMoney = itemSum,
                    CoursePriceRule = priceRule,
                    ErrMsg = string.Empty
                };
            }
            return new CheckBuyMallGoodsResult()
            {
                TotalMoney = mallGoods.Price,
                ErrMsg = string.Empty
            };
        }

        public async Task<ResponseBase> ParentBuyMallGoodsPrepay(ParentBuyMallGoodsPrepayRequest request)
        {
            var checkTenantLcsAccountResult = await CheckTenantLcsAccount(request.LoginTenantId);
            if (!string.IsNullOrEmpty(checkTenantLcsAccountResult.ErrMsg))
            {
                return ResponseBase.CommonError(checkTenantLcsAccountResult.ErrMsg);
            }
            var mallGoodsBucket = await _mallGoodsDAL.GetMallGoods(request.GId);
            if (mallGoodsBucket == null || mallGoodsBucket.MallGoods == null)
            {
                return ResponseBase.CommonError("商品不存在");
            }
            var tenantWechartAuth = await _componentAccessBLL.GetTenantWechartAuth(request.LoginTenantId);
            if (tenantWechartAuth == null)
            {
                Log.Error($"[ParentBuyMallGoodsPrepay]未找到机构授权信息,tenantId:{request.LoginTenantId}", this.GetType());
                return ResponseBase.CommonError("机构绑定的微信公众号无权限");
            }
            var myMallGoods = mallGoodsBucket.MallGoods;
            var myMallCoursePriceRules = mallGoodsBucket.MallCoursePriceRules;
            var checkBuyMallGoodsResult = CheckBuyMallGoods(myMallGoods, myMallCoursePriceRules, request.BuyCount, request.CoursePriceRuleId);
            if (!string.IsNullOrEmpty(checkBuyMallGoodsResult.ErrMsg))
            {
                return ResponseBase.CommonError(checkBuyMallGoodsResult.ErrMsg);
            }
            var totalMoney = checkBuyMallGoodsResult.TotalMoney;
            //var myCoursePriceRule = checkBuyMallGoodsResult.CoursePriceRule;

            var myTenant = checkTenantLcsAccountResult.MyTenant;
            var myLcsAccount = checkTenantLcsAccountResult.MyLcsAccount;
            var now = DateTime.Now;
            var orderNo = OrderNumberLib.EnrolmentOrderNumber2();
            var unifiedOrderRequest = new RequestUnifiedOrder()
            {
                access_token = myLcsAccount.AccessToken,
                terminal_id = myLcsAccount.TerminalId,
                terminal_time = ComBusiness4.GetLcsTerminalTime(now),
                attach = orderNo,
                merchant_no = myLcsAccount.MerchantNo,
                open_id = request.OpenId,
                order_body = "在线商城_商品购买",
                payType = "010",
                terminal_trace = orderNo,
                notify_url = SysWebApiAddressConfig.LcsPayJspayCallbackUrl,
                total_fee = EtmsHelper3.GetCent(totalMoney).ToString(),
                sub_appid = tenantWechartAuth.AuthorizerAppid
            };
            var unifiedOrderResult = _payLcswService.UnifiedOrder(unifiedOrderRequest);
            if (unifiedOrderResult.IsSuccess())
            {
                var payLogId = await _tenantLcsPayLogDAL.AddTenantLcsPayLog(new EtTenantLcsPayLog()
                {
                    AgentId = myTenant.AgentId,
                    Attach = orderNo,
                    AuthNo = string.Empty,
                    CreateOt = now,
                    DataType = EmTenantLcsPayLogDataType.Prepaid,
                    IsDeleted = EmIsDeleted.Normal,
                    MerchantName = myLcsAccount.MerchantName,
                    MerchantNo = myLcsAccount.MerchantNo,
                    MerchantType = myLcsAccount.MerchantType,
                    OpenId = request.OpenId,
                    OrderBody = unifiedOrderRequest.order_body,
                    OrderDesc = "在线购课",
                    OrderNo = orderNo,
                    OrderSource = EmLcsPayLogOrderSource.WeChat,
                    OrderType = EmLcsPayLogOrderType.OnlineBuyCourse,
                    OutRefundNo = string.Empty,
                    OutTradeNo = unifiedOrderResult.out_trade_no,
                    PayFinishDate = null,
                    PayFinishOt = null,
                    PayType = unifiedOrderResult.pay_type,
                    RefundDate = null,
                    RefundFee = null,
                    RefundOt = null,
                    RelationId = request.ParentStudentIds[0],
                    Remark = string.Empty,
                    Status = EmLcsPayLogStatus.Unpaid,
                    SubAppid = unifiedOrderResult.appId,
                    TenantId = myTenant.Id,
                    TerminalId = myLcsAccount.TerminalId,
                    TerminalTrace = orderNo,
                    TotalFee = unifiedOrderResult.total_fee,
                    TotalFeeDesc = totalMoney.ToString(),
                    TotalFeeValue = totalMoney
                });
                return ResponseBase.Success(new ParentBuyMallGoodsPrepayOutput()
                {
                    ali_trade_no = unifiedOrderResult.ali_trade_no,
                    appId = unifiedOrderResult.appId,
                    nonceStr = unifiedOrderResult.nonceStr,
                    orderNo = orderNo,
                    package_str = unifiedOrderResult.package_str,
                    paySign = unifiedOrderResult.paySign,
                    signType = unifiedOrderResult.signType,
                    TenantLcsPayLogId = payLogId,
                    timeStamp = unifiedOrderResult.timeStamp,
                    token_id = unifiedOrderResult.token_id
                });
            }
            return ResponseBase.CommonError($"生成预支付订单失败:{unifiedOrderResult.return_msg}");
        }

        public async Task<ResponseBase> ParentBuyMallGoodsSubmit(ParentBuyMallGoodsSubmitRequest request)
        {
            return ResponseBase.Success();
        }
    }
}
