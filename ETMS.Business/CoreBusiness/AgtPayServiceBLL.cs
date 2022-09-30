using Com.Fubei.OpenApi.Sdk;
using Com.Fubei.OpenApi.Sdk.Dto.Em;
using Com.Fubei.OpenApi.Sdk.Enums;
using Com.Fubei.OpenApi.Sdk.Models;
using ETMS.Business.Common;
using ETMS.Entity.Config;
using ETMS.Entity.Dto.CoreBusiness.Output;
using ETMS.Entity.Dto.CoreBusiness.Request;
using ETMS.Entity.Enum.EtmsManage;
using ETMS.Entity.Pay.Lcsw.Dto.Request;
using ETMS.Entity.Temp;
using ETMS.Event.DataContract.Activity;
using ETMS.IBusiness;
using ETMS.IEventProvider;
using ETMS.Pay.Lcsw;
using ETMS.Pay.Suixing;
using ETMS.Pay.Suixing.Utility.Dto;
using ETMS.Pay.Suixing.Utility.ExternalDto.Request;
using ETMS.Utility;
using FubeiOpenApi.CoreSdk.Models.Parameter.Agent;
using FubeiOpenApi.CoreSdk.Models.Response.Agent;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.Business
{
    public class AgtPayServiceBLL : IAgtPayServiceBLL
    {
        private readonly IPayLcswService _payLcswService;

        private CheckTenantLcsAccountView _checkTenantAgtPayAccountResult;

        private readonly IPaySuixingService _paySuixingService;

        private readonly IEventPublisher _eventPublisher;

        public AgtPayServiceBLL(IPayLcswService payLcswService, IPaySuixingService paySuixingService, IEventPublisher eventPublisher)
        {
            this._payLcswService = payLcswService;
            this._paySuixingService = paySuixingService;
            this._eventPublisher = eventPublisher;
        }

        public void InitTenantId(int tenantId)
        {
        }

        public void Initialize(CheckTenantLcsAccountView checkResult)
        {
            this._checkTenantAgtPayAccountResult = checkResult;
        }

        private T GetErrResult<T>(string msg) where T : AgtPayServiceOutputBase, new()
        {
            var res = new T();
            res.IsSuccess = false;
            res.ErrMsg = msg;
            return res;
        }

        private string GetFubeiPayTypeDesc(string payType)
        {
            switch (payType)
            {
                case "wxpay":
                    return "微信";
                case "alipay":
                    return "支付宝";
                case "unionpay":
                    return " 银联";
            }
            return payType;
        }

        public async Task<WxConfigOutput> WxConfig(WxConfigRequest request)
        {
            var apiLevel = EApiLevel.Merchant;
            if (request.AccountType == EmFubeiAccountType.Vendor)
            {
                apiLevel = EApiLevel.Vendor;
            }
            var result = await FubeiOpenApiCoreSdk.PostVendorApiRequestAsync<AOrderWxConfigResultEntity>("fbpay.order.wxconfig", new AOrderWxConfig()
            {
                MerchantId = request.MerchantId,
                StoreId = request.StoreId ?? 0,
                SubAppId = request.WxSubAppid,
                JsapiPath = request.JsapiPath,
                AppId = request.AppId,
                AppSecret = request.AppSecret,
                VendorSn = request.VendorSn,
                VendorSecret = request.VendorSecret
            }, apiLevel);
            if (result == null || !result.IsSuccess() || result.Data == null)
            {
                return GetErrResult<WxConfigOutput>("微信配置失败");
            }
            var myResult = result.Data;
            if (myResult.SubAppidCode != 1 || myResult.JsapiCode != 1)
            {
                return GetErrResult<WxConfigOutput>("微信配置失败");
            }
            return new WxConfigOutput()
            {
                IsSuccess = true,
                ErrMsg = string.Empty
            };
        }

        public async Task<CallbackConfigOutput> CallbackConfig(CallbackConfigRequest request)
        {
            var apiLevel = EApiLevel.Merchant;
            if (request.AccountType == EmFubeiAccountType.Vendor)
            {
                apiLevel = EApiLevel.Vendor;
            }
            var result = await FubeiOpenApiCoreSdk.PostVendorApiRequestAsync<ACallbackConfigResultEntity>("fbpay.pay.callback.config", new ACallbackConfig()
            {
                AppId = request.AppId,
                AppSecret = request.AppSecret,
                VendorSn = request.VendorSn,
                VendorSecret = request.VendorSecret,
                merchant_id = request.MerchantId,
                refund_callback_url = request.refund_callback_url,
                remit_callback_url = null,
                second_callback_url = null
            }, apiLevel);
            if (result == null || !result.IsSuccess() || result.Data == null)
            {
                return GetErrResult<CallbackConfigOutput>("回调服务配置失败");
            }
            var myResult = result.Data;
            if (myResult.bind_status != 1)
            {
                return GetErrResult<CallbackConfigOutput>(myResult.resp_message);
            }
            return new CallbackConfigOutput()
            {
                IsSuccess = true,
                ErrMsg = string.Empty
            };
        }

        #region 扫码支付

        public async Task<BarcodePayOutput> BarcodePay(BarcodePayRequest request)
        {
            switch (_checkTenantAgtPayAccountResult.MyTenant.AgtPayType)
            {
                case EmAgtPayType.Lcsw:
                    return BarcodePay_Lcsw(request);
                case EmAgtPayType.Fubei:
                    return await BarcodePay_Fubei(request);
                case EmAgtPayType.Suixing:
                    return BarcodePay_SuixingPay(request);
            }
            return GetErrResult<BarcodePayOutput>("账户异常");
        }

        private BarcodePayOutput BarcodePay_Lcsw(BarcodePayRequest request)
        {
            var myTenant = _checkTenantAgtPayAccountResult.MyTenant;
            var myLcsAccount = _checkTenantAgtPayAccountResult.MyLcsAccount;
            var payRequest = new RequestBarcodePay()
            {
                access_token = myLcsAccount.AccessToken,
                terminal_id = myLcsAccount.TerminalId,
                terminal_time = ComBusiness4.GetLcsTerminalTime(request.Now),
                payType = EmLcsPayType.AutoTypePay,
                auth_no = request.AuthNo,
                terminal_trace = request.OrderNo,
                attach = myTenant.Id.ToString(),
                merchant_no = myLcsAccount.MerchantNo,
                order_body = request.OrderDesc,
                total_fee = request.PayMoneyCent.ToString(),
                terminal_ip = request.IpAddress
            };
            var resPay = _payLcswService.BarcodePay(payRequest);
            if (!resPay.IsRequestSuccess())
            {
                return GetErrResult<BarcodePayOutput>($"扫码支付失败:{resPay.return_msg}");
            }
            var status = EmLcsPayLogStatus.Unpaid;
            switch (resPay.result_code)
            {
                case "01": //成功
                    status = EmLcsPayLogStatus.PaySuccess;
                    break;
                case "02": //失败 
                    status = EmLcsPayLogStatus.PayFail;
                    break;
                case "03": //支付中
                    status = EmLcsPayLogStatus.Unpaid;
                    break;
                case "99": //该条码暂不支持支付类型自动匹配  支付失败
                    status = EmLcsPayLogStatus.PayFail;
                    break;
            }
            if (status == EmLcsPayLogStatus.PayFail)
            {
                return new BarcodePayOutput()
                {
                    ErrMsg = string.Empty,
                    IsSuccess = true,
                    PayResultType = status
                };
            }
            return new BarcodePayOutput()
            {
                ErrMsg = string.Empty,
                IsSuccess = true,
                PayResultType = status,
                OutTradeNo = resPay.out_trade_no,
                PayType = resPay.pay_type
            };
        }

        private async Task<BarcodePayOutput> BarcodePay_Fubei(BarcodePayRequest request)
        {
            var myTenant = _checkTenantAgtPayAccountResult.MyTenant;
            var myFubeiAccount = _checkTenantAgtPayAccountResult.MyFubeiAccount;
            FubeiApiCommonResult<AOrderDetailResultEntity> result = null;
            if (myFubeiAccount.AccountType == EmFubeiAccountType.Merchant)
            {
                //商户级
                result = await FubeiOpenApiCoreSdk.PostVendorApiRequestAsync<AOrderDetailResultEntity>("fbpay.order.pay", new AOrderPayParam()
                {
                    MerchantOrderSn = request.OrderNo,
                    AuthCode = request.AuthNo,
                    TotalAmount = request.PayMoney,
                    StoreId = myFubeiAccount.StoreId ?? 0,
                    CashierId = myFubeiAccount.CashierId,
                    AppId = myFubeiAccount.AppId,
                    AppSecret = myFubeiAccount.AppSecret,
                    VendorSn = myFubeiAccount.VendorSn,
                    VendorSecret = myFubeiAccount.VendorSecret,
                    Attach = myTenant.Id.ToString(),
                    Body = request.OrderDesc,
                    NotifyUrl = SysWebApiAddressConfig.FubeiApiNotifyUrl
                }, EApiLevel.Merchant);
            }
            else
            {
                //服务商级
                result = await FubeiOpenApiCoreSdk.PostVendorApiRequestAsync<AOrderDetailResultEntity>("fbpay.order.pay", new AOrderPayParam()
                {
                    MerchantOrderSn = request.OrderNo,
                    AuthCode = request.AuthNo,
                    TotalAmount = request.PayMoney,
                    StoreId = myFubeiAccount.StoreId ?? 0,
                    CashierId = myFubeiAccount.CashierId,
                    AppId = myFubeiAccount.AppId,
                    AppSecret = myFubeiAccount.AppSecret,
                    VendorSn = myFubeiAccount.VendorSn,
                    VendorSecret = myFubeiAccount.VendorSecret,
                    Attach = myTenant.Id.ToString(),
                    Body = request.OrderDesc,
                    NotifyUrl = SysWebApiAddressConfig.FubeiApiNotifyUrl,
                    MerchantId = myFubeiAccount.MerchantId
                }, EApiLevel.Vendor);
            }
            if (result == null || !result.IsSuccess() || result.Data == null)
            {
                return new BarcodePayOutput()
                {
                    ErrMsg = string.Empty,
                    IsSuccess = true,
                    PayResultType = EmLcsPayLogStatus.PayFail
                };
            }
            var myResultData = result.Data;
            var status = ComBusiness4.GetFubeiPayStatus(myResultData.OrderStatus);
            return new BarcodePayOutput()
            {
                ErrMsg = string.Empty,
                IsSuccess = true,
                PayResultType = status,
                OutTradeNo = myResultData.OrderSn,
                PayType = GetFubeiPayTypeDesc(myResultData.PayType)
            };
        }

        private BarcodePayOutput BarcodePay_SuixingPay(BarcodePayRequest request)
        {
            var myTenant = _checkTenantAgtPayAccountResult.MyTenant;
            var mySuixingAccount = _checkTenantAgtPayAccountResult.MySysTenantSuixingAccount;
            var payRequest = new ReverseScanReq()
            {
                ordNo = request.OrderNo,
                authCode = request.AuthNo,
                trmIp = request.IpAddress,
                subject = request.OrderDesc,
                mno = mySuixingAccount.Mno,
                amt = request.PayMoney.ToString("F2"),
                extend = myTenant.Id.ToString(),
                notifyUrl = SysWebApiAddressConfig.SuixingPayCallbackUrl2
            };
            var resPay = _paySuixingService.ReverseScan(payRequest);
            if (resPay == null)
            {
                return GetErrResult<BarcodePayOutput>("扫码支付失败");
            }
            var status = EmLcsPayLogStatus.Unpaid;
            var payTag = resPay.tranSts.ToUpper().Trim();
            switch (payTag)
            {
                case "SUCCESS":
                    status = EmLcsPayLogStatus.PaySuccess;
                    break;
                case "FAIL":
                    status = EmLcsPayLogStatus.PayFail;
                    break;
                case "PAYING":
                    status = EmLcsPayLogStatus.Unpaid;
                    break;
            }
            return new BarcodePayOutput()
            {
                IsSuccess = true,
                PayResultType = status,
                OutTradeNo = resPay.uuid ?? string.Empty,
                ErrMsg = resPay.bizMsg,
                PayType = resPay.payType ?? string.Empty
            };
        }

        #endregion

        #region  预支付订单

        public async Task<UnifiedOrderOutput> UnifiedOrder(UnifiedOrderRequest request)
        {
            switch (_checkTenantAgtPayAccountResult.MyTenant.AgtPayType)
            {
                case EmAgtPayType.Lcsw:
                    return UnifiedOrder_Lcsw(request);
                case EmAgtPayType.Fubei:
                    return await UnifiedOrder_Fubei(request);
                case EmAgtPayType.Suixing:
                    return UnifiedOrder_Suixing(request);
            }
            return GetErrResult<UnifiedOrderOutput>("账户异常");
        }

        private UnifiedOrderOutput UnifiedOrder_Lcsw(UnifiedOrderRequest request)
        {
            var myTenant = _checkTenantAgtPayAccountResult.MyTenant;
            var myLcsAccount = _checkTenantAgtPayAccountResult.MyLcsAccount;
            var unifiedOrderRequest = new RequestUnifiedOrder()
            {
                access_token = myLcsAccount.AccessToken,
                terminal_id = myLcsAccount.TerminalId,
                terminal_time = ComBusiness4.GetLcsTerminalTime(request.Now),
                merchant_no = myLcsAccount.MerchantNo,
                open_id = request.OpenId,
                order_body = request.OrderBody,
                payType = "010",
                terminal_trace = request.OrderNo,
                notify_url = SysWebApiAddressConfig.LcsPayJspayCallbackUrl,
                total_fee = request.PayMoneyCent.ToString(),
                sub_appid = request.SubAppid,
                attach = $"{myTenant.Id}_{request.PayLogId}",
                terminal_ip = request.IpAddress
            };
            var unifiedOrderResult = _payLcswService.UnifiedOrder(unifiedOrderRequest);
            if (unifiedOrderResult.IsSuccess())
            {
                return new UnifiedOrderOutput()
                {
                    IsSuccess = true,
                    ErrMsg = string.Empty,
                    AppId = unifiedOrderResult.appId,
                    NonceStr = unifiedOrderResult.nonceStr,
                    OutTradeNo = unifiedOrderResult.out_trade_no,
                    Package_str = unifiedOrderResult.package_str,
                    PaySign = unifiedOrderResult.paySign,
                    PayType = unifiedOrderResult.pay_type,
                    SignType = unifiedOrderResult.signType,
                    TimeStamp = unifiedOrderResult.timeStamp,
                    ali_trade_no = unifiedOrderResult.ali_trade_no
                };
            }
            return GetErrResult<UnifiedOrderOutput>($"生成预支付订单失败:{unifiedOrderResult.return_msg}");
        }

        private async Task<UnifiedOrderOutput> UnifiedOrder_Fubei(UnifiedOrderRequest request)
        {
            var myTenant = _checkTenantAgtPayAccountResult.MyTenant;
            var myFubeiAccount = _checkTenantAgtPayAccountResult.MyFubeiAccount;
            FubeiApiCommonResult<AOrderCreateResultEntity> result = null;
            if (myFubeiAccount.AccountType == EmFubeiAccountType.Merchant)
            {
                //商户级
                result = await FubeiOpenApiCoreSdk.PostVendorApiRequestAsync<AOrderCreateResultEntity>("fbpay.order.create", new AOrderCreateParam()
                {
                    MerchantOrderSn = request.OrderNo,
                    PayType = "wxpay",
                    TotalAmount = request.PayMoney,
                    StoreId = myFubeiAccount.StoreId,
                    CashierId = myFubeiAccount.CashierId,
                    SubAppId = request.SubAppid,
                    UserId = request.OpenId,
                    AppId = myFubeiAccount.AppId,
                    AppSecret = myFubeiAccount.AppSecret,
                    VendorSn = myFubeiAccount.VendorSn,
                    VendorSecret = myFubeiAccount.VendorSecret,
                    Attach = myTenant.Id.ToString(),
                    Body = request.OrderBody,
                    NotifyUrl = SysWebApiAddressConfig.FubeiApiNotifyUrl
                }, EApiLevel.Merchant);
            }
            else
            {
                //服务商级
                result = await FubeiOpenApiCoreSdk.PostVendorApiRequestAsync<AOrderCreateResultEntity>("fbpay.order.create", new AOrderCreateParam()
                {
                    MerchantOrderSn = request.OrderNo,
                    PayType = "wxpay",
                    TotalAmount = request.PayMoney,
                    StoreId = myFubeiAccount.StoreId,
                    CashierId = myFubeiAccount.CashierId,
                    SubAppId = request.SubAppid,
                    UserId = request.OpenId,
                    AppId = myFubeiAccount.AppId,
                    AppSecret = myFubeiAccount.AppSecret,
                    VendorSn = myFubeiAccount.VendorSn,
                    VendorSecret = myFubeiAccount.VendorSecret,
                    Attach = myTenant.Id.ToString(),
                    Body = request.OrderBody,
                    MerchantId = myFubeiAccount.MerchantId,
                    NotifyUrl = SysWebApiAddressConfig.FubeiApiNotifyUrl
                }, EApiLevel.Vendor);
            }
            if (result == null || !result.IsSuccess() || result.Data == null)
            {
                return GetErrResult<UnifiedOrderOutput>($"生成预支付订单失败:{result.ResultMessage}");
            }
            var myResultData = result.Data;
            var signPackage = myResultData.SignPackage;
            return new UnifiedOrderOutput()
            {
                IsSuccess = true,
                ErrMsg = string.Empty,
                AppId = signPackage.AppId,
                NonceStr = signPackage.NonceStr,
                OutTradeNo = myResultData.OrderSn,
                Package_str = signPackage.Package,
                PaySign = signPackage.PaySign,
                PayType = GetFubeiPayTypeDesc(myResultData.PayType),
                SignType = signPackage.SignType,
                TimeStamp = signPackage.TimeStamp,
                ali_trade_no = string.Empty
            };
        }

        private UnifiedOrderOutput UnifiedOrder_Suixing(UnifiedOrderRequest request)
        {
            var myTenant = _checkTenantAgtPayAccountResult.MyTenant;
            var mySuixingAccount = _checkTenantAgtPayAccountResult.MySysTenantSuixingAccount;
            var req = new JsapiScanMiniProgramReq()
            {
                mno = mySuixingAccount.Mno,
                notifyUrl = SysWebApiAddressConfig.SuixingPayCallbackUrl2,
                trmIp = request.IpAddress,
                extend = myTenant.Id.ToString(),
                ordNo = request.OrderNo,
                subject = request.OrderBody,
                openid = request.OpenId,
                amt = request.PayMoney
            };
            var res = _paySuixingService.JsapiScan(req);
            if (res == null)
            {
                return GetErrResult<UnifiedOrderOutput>("生成预支付订单失败");
            }
            return new UnifiedOrderOutput()
            {
                IsSuccess = true,
                ErrMsg = res.bizMsg,
                OutTradeNo = res.uuid,
                ali_trade_no = string.Empty,
                AppId = res.payAppId,
                NonceStr = res.paynonceStr,
                Package_str = res.payPackage,
                PaySign = res.paySign,
                SignType = res.paySignType,
                TimeStamp = res.payTimeStamp,
                PayType = "微信"
            };
        }

        #endregion

        #region 查询订单支付状态

        public async Task<QueryPayLogOutput> QueryPayLog(QueryPayLogRequest request)
        {
            switch (_checkTenantAgtPayAccountResult.MyTenant.AgtPayType)
            {
                case EmAgtPayType.Lcsw:
                    return QueryPayLog_Lcsw(request);
                case EmAgtPayType.Fubei:
                    return await QueryPayLog_Fubei(request);
                case EmAgtPayType.Suixing:
                    return QueryPayLog_Suixing(request);
            }
            return GetErrResult<QueryPayLogOutput>("账户异常");
        }

        private QueryPayLogOutput QueryPayLog_Lcsw(QueryPayLogRequest request)
        {
            var myLcsAccount = _checkTenantAgtPayAccountResult.MyLcsAccount;
            var payResult = _payLcswService.QueryPay(new RequestQuery()
            {
                access_token = myLcsAccount.AccessToken,
                merchant_no = myLcsAccount.MerchantNo,
                out_trade_no = request.out_trade_no,
                payType = request.pay_type,
                terminal_id = myLcsAccount.TerminalId,
                terminal_time = ComBusiness4.GetLcsTerminalTime(request.Now),
                terminal_trace = request.OrderNo
            });
            if (!payResult.IsRequestSuccess())
            {
                return GetErrResult<QueryPayLogOutput>($"扫码支付失败:{payResult.return_msg}");
            }
            var status = EmLcsPayLogStatus.Unpaid;
            switch (payResult.result_code)
            {
                case "01": //成功
                    status = EmLcsPayLogStatus.PaySuccess;
                    break;
                case "02": //失败 
                    status = EmLcsPayLogStatus.PayFail;
                    break;
                case "03": //支付中
                    break;
                case "99": //该条码暂不支持支付类型自动匹配  支付失败
                    status = EmLcsPayLogStatus.PayFail;
                    break;
            }
            return new QueryPayLogOutput()
            {
                IsSuccess = true,
                ErrMsg = string.Empty,
                PayResultType = status
            };
        }

        private async Task<QueryPayLogOutput> QueryPayLog_Fubei(QueryPayLogRequest request)
        {
            var myFubeiAccount = _checkTenantAgtPayAccountResult.MyFubeiAccount;
            FubeiApiCommonResult<AOrderDetailResultEntity> result = null;
            if (myFubeiAccount.AccountType == EmFubeiAccountType.Merchant)
            {
                //商户级
                result = await FubeiOpenApiCoreSdk.PostVendorApiRequestAsync<AOrderDetailResultEntity>("fbpay.order.query", new AOrderQueryParam()
                {
                    MerchantOrderSn = request.OrderNo,
                    OrderSn = request.out_trade_no,
                    InsOrderSn = string.Empty,
                    AppId = myFubeiAccount.AppId,
                    AppSecret = myFubeiAccount.AppSecret,
                    VendorSn = myFubeiAccount.VendorSn,
                    VendorSecret = myFubeiAccount.VendorSecret
                }, EApiLevel.Merchant);
            }
            else
            {
                //服务商级
                result = await FubeiOpenApiCoreSdk.PostVendorApiRequestAsync<AOrderDetailResultEntity>("fbpay.order.query", new AOrderQueryParam()
                {
                    MerchantOrderSn = request.OrderNo,
                    OrderSn = request.out_trade_no,
                    InsOrderSn = string.Empty,
                    AppId = myFubeiAccount.AppId,
                    AppSecret = myFubeiAccount.AppSecret,
                    VendorSn = myFubeiAccount.VendorSn,
                    VendorSecret = myFubeiAccount.VendorSecret,
                    MerchantId = myFubeiAccount.MerchantId
                }, EApiLevel.Vendor);
            }
            if (result == null || !result.IsSuccess() || result.Data == null)
            {
                return GetErrResult<QueryPayLogOutput>(result.ResultMessage);
            }
            var myResult = result.Data;
            return new QueryPayLogOutput()
            {
                IsSuccess = true,
                ErrMsg = string.Empty,
                PayResultType = ComBusiness4.GetFubeiPayStatus(myResult.OrderStatus)
            };
        }

        private QueryPayLogOutput QueryPayLog_Suixing(QueryPayLogRequest request)
        {
            var myTenant = _checkTenantAgtPayAccountResult.MyTenant;
            var mySuixingAccount = _checkTenantAgtPayAccountResult.MySysTenantSuixingAccount;
            var resPay = _paySuixingService.TradeQuery(mySuixingAccount.Mno, request.OrderNo);
            if (resPay == null)
            {
                return GetErrResult<QueryPayLogOutput>("支付错误");
            }
            var status = EmLcsPayLogStatus.Unpaid;
            var payTag = resPay.tranSts.ToUpper().Trim();
            switch (payTag)
            {
                case "SUCCESS":
                    status = EmLcsPayLogStatus.PaySuccess;
                    break;
                case "FAIL":
                    status = EmLcsPayLogStatus.PayFail;
                    break;
                case "PAYING":
                    status = EmLcsPayLogStatus.Unpaid;
                    break;
            }
            return new QueryPayLogOutput()
            {
                IsSuccess = true,
                ErrMsg = string.Empty,
                PayResultType = status,
                OutTradeNo = resPay.uuid
            };
        }

        #endregion

        #region 退款

        public async Task<RefundPayOutput> RefundPay(RefundPayRequest request)
        {
            //if (request.Paylog.AgtPayType != _checkTenantAgtPayAccountResult.MyTenant.AgtPayType)
            //{
            //    return GetErrResult<RefundPayOutput>("支付订单与所绑定的聚合支付服务商不一致，无法退款");
            //}
            switch (request.Paylog.AgtPayType)
            {
                case EmAgtPayType.Lcsw:
                    return RefundPay_Lcsw(request);
                case EmAgtPayType.Fubei:
                    return await RefundPay_Fubei(request);
                case EmAgtPayType.Suixing:
                    return RefundPay_Suixing(request);
            }
            return GetErrResult<RefundPayOutput>("账户异常");
        }

        public RefundPayOutput RefundPay_Lcsw(RefundPayRequest request)
        {
            var myLcsAccount = _checkTenantAgtPayAccountResult.MyLcsAccount;
            var paylog = request.Paylog;
            var refundResult = _payLcswService.RefundPay(new RequestRefund()
            {
                access_token = myLcsAccount.AccessToken,
                merchant_no = myLcsAccount.MerchantNo,
                out_trade_no = paylog.OutTradeNo,
                payType = paylog.PayType,
                refund_fee = paylog.TotalFee,
                terminal_id = myLcsAccount.TerminalId,
                terminal_time = ComBusiness4.GetLcsTerminalTime(request.Now),
                terminal_trace = paylog.OrderNo
            });
            if (!refundResult.IsSuccess(true))
            {
                return GetErrResult<RefundPayOutput>(refundResult.return_msg);
            }
            return new RefundPayOutput()
            {
                IsSuccess = true,
                ErrMsg = string.Empty,
                out_refund_no = refundResult.out_refund_no,
                refund_fee = refundResult.refund_fee,
                RefundStatus = EmLcsPayLogStatus.Refunded
            };
        }

        public async Task<RefundPayOutput> RefundPay_Fubei(RefundPayRequest request)
        {
            var paylog = request.Paylog;
            var myFubeiAccount = _checkTenantAgtPayAccountResult.MyFubeiAccount;
            FubeiApiCommonResult<AOrderRefundResultEntity> result = null;
            var refundSn = OrderNumberLib.FubeiRefundOrder(paylog.TenantId);
            if (myFubeiAccount.AccountType == EmFubeiAccountType.Merchant)
            {
                //商户级
                result = await FubeiOpenApiCoreSdk.PostVendorApiRequestAsync<AOrderRefundResultEntity>("fbpay.order.refund", new AOrderRefundParam()
                {
                    AppId = myFubeiAccount.AppId,
                    AppSecret = myFubeiAccount.AppSecret,
                    VendorSn = myFubeiAccount.VendorSn,
                    VendorSecret = myFubeiAccount.VendorSecret,
                    merchant_id = myFubeiAccount.MerchantId,
                    order_sn = paylog.OutTradeNo,
                    merchant_order_sn = paylog.OrderNo,
                    refund_amount = paylog.TotalFeeValue,
                    merchant_refund_sn = refundSn
                }, EApiLevel.Merchant);
            }
            else
            {
                //服务商级
                result = await FubeiOpenApiCoreSdk.PostVendorApiRequestAsync<AOrderRefundResultEntity>("fbpay.order.refund", new AOrderRefundParam()
                {
                    AppId = myFubeiAccount.AppId,
                    AppSecret = myFubeiAccount.AppSecret,
                    VendorSn = myFubeiAccount.VendorSn,
                    VendorSecret = myFubeiAccount.VendorSecret,
                    merchant_id = myFubeiAccount.MerchantId,
                    order_sn = paylog.OutTradeNo,
                    merchant_order_sn = paylog.OrderNo,
                    refund_amount = paylog.TotalFeeValue,
                    merchant_refund_sn = refundSn
                }, EApiLevel.Vendor);
            }
            if (result == null || !result.IsSuccess() || result.Data == null)
            {
                return GetErrResult<RefundPayOutput>($"退款失败:{result.ResultMessage}");
            }
            var myResultData = result.Data;
            switch (myResultData.refund_status)
            {
                case EmRefundStatus.Refunding:
                    return new RefundPayOutput()
                    {
                        IsSuccess = true,
                        ErrMsg = string.Empty,
                        RefundStatus = EmLcsPayLogStatus.Refunding,
                        refund_fee = paylog.TotalFee,
                        out_refund_no = myResultData.merchant_refund_sn
                    };
                case EmRefundStatus.Success:
                    return new RefundPayOutput()
                    {
                        IsSuccess = true,
                        ErrMsg = string.Empty,
                        RefundStatus = EmLcsPayLogStatus.Refunded,
                        refund_fee = paylog.TotalFee,
                        out_refund_no = myResultData.merchant_refund_sn
                    };
                default:
                    return GetErrResult<RefundPayOutput>("退款失败");
            }
        }

        public RefundPayOutput RefundPay_Suixing(RefundPayRequest request)
        {
            var paylog = request.Paylog;
            var refundResult = _paySuixingService.Refund(new RefundReq()
            {
                ordNo = OrderNumberLib.SuixingRefundOrder(),
                mno = paylog.MerchantNo,
                origUuid = paylog.OutTradeNo,
                amt = paylog.TotalFeeValue,
                notifyUrl = SysWebApiAddressConfig.SuixingRefundCallbackUrl,
                refundReason = "退款",
                extend = $"{paylog.TenantId}_{paylog.RelationId}"
            });
            if (refundResult != null && refundResult.bizCode == EmBizCode.Success)
            {
                _eventPublisher.Publish(new SuixingRefundCallbackEvent(paylog.TenantId)
                {
                    ActivityRouteItemId = paylog.RelationId,
                    RefundTime = DateTime.Now
                });
                return new RefundPayOutput()
                {
                    IsSuccess = true,
                    ErrMsg = string.Empty,
                    out_refund_no = refundResult.sxfUuid,
                    refund_fee = paylog.TotalFee,
                    RefundStatus = EmLcsPayLogStatus.Refunded
                };
            }
            if (refundResult != null && !string.IsNullOrEmpty(refundResult.bizMsg))
            {
                return GetErrResult<RefundPayOutput>(refundResult.bizMsg);

            }
            return GetErrResult<RefundPayOutput>("退款失败");
        }

        #endregion 
    }
}
