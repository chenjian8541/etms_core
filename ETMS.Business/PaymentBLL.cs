using Com.Fubei.OpenApi.Sdk;
using Com.Fubei.OpenApi.Sdk.Dto.Em;
using Com.Fubei.OpenApi.Sdk.Enums;
using Com.Fubei.OpenApi.Sdk.Models;
using ETMS.Business.BaseBLL;
using ETMS.Business.Common;
using ETMS.Entity.Common;
using ETMS.Entity.Config;
using ETMS.Entity.Database.Manage;
using ETMS.Entity.Database.Source;
using ETMS.Entity.Dto.CoreBusiness.Request;
using ETMS.Entity.Dto.PaymentService.Output;
using ETMS.Entity.Dto.PaymentService.Request;
using ETMS.Entity.Enum;
using ETMS.Entity.Enum.EtmsManage;
using ETMS.Entity.Pay.Lcsw.Dto.Request;
using ETMS.Entity.Temp;
using ETMS.Event.DataContract;
using ETMS.Event.DataContract.Activity;
using ETMS.Event.DataContract.Statistics;
using ETMS.IBusiness;
using ETMS.IDataAccess;
using ETMS.IDataAccess.Activity;
using ETMS.IDataAccess.EtmsManage;
using ETMS.IDataAccess.Lcs;
using ETMS.IEventProvider;
using ETMS.Pay.Lcsw;
using ETMS.Pay.Suixing.Utility.Dto;
using ETMS.Utility;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.Business
{
    public class PaymentBLL : TenantLcsAccountBLL, IPaymentBLL
    {
        private readonly IStudentDAL _studentDAL;

        private readonly IUserOperationLogDAL _userOperationLogDAL;

        private readonly ITenantLcsPayLogDAL _tenantLcsPayLogDAL;

        private readonly IEventPublisher _eventPublisher;

        private readonly IAgtPayServiceBLL _agtPayServiceBLL;

        private readonly IHttpContextAccessor _httpContextAccessor;

        public PaymentBLL(ITenantLcsAccountDAL tenantLcsAccountDAL, IStudentDAL studentDAL, ISysTenantDAL sysTenantDAL,
           IUserOperationLogDAL userOperationLogDAL, ITenantLcsPayLogDAL tenantLcsPayLogDAL,
            IEventPublisher eventPublisher, ITenantFubeiAccountDAL tenantFubeiAccountDAL, IAgtPayServiceBLL agtPayServiceBLL,
            IHttpContextAccessor httpContextAccessor)
            : base(tenantLcsAccountDAL, sysTenantDAL, tenantFubeiAccountDAL)
        {
            this._studentDAL = studentDAL;
            this._userOperationLogDAL = userOperationLogDAL;
            this._tenantLcsPayLogDAL = tenantLcsPayLogDAL;
            this._eventPublisher = eventPublisher;
            this._agtPayServiceBLL = agtPayServiceBLL;
            this._httpContextAccessor = httpContextAccessor;
        }

        public void InitTenantId(int tenantId)
        {
            this._agtPayServiceBLL.InitTenantId(tenantId);
            this.InitDataAccess(tenantId, _studentDAL, _userOperationLogDAL, _tenantLcsPayLogDAL);
        }

        public async Task<ResponseBase> TenantLcsPayLogPaging(TenantLcsPayLogPagingRequest request)
        {
            var pagingData = await _tenantLcsPayLogDAL.GetTenantLcsPayLogPaging(request);
            var output = new List<TenantLcsPayLogPagingOutput>();
            if (pagingData.Item1.Any())
            {
                var tempBoxStudent = new DataTempBox<EtStudent>();
                var limitRefundDate = DateTime.Now.Date.AddDays(-SystemConfig.ComConfig.LcsRefundOrderLimitDay);
                foreach (var p in pagingData.Item1)
                {
                    var studentName = p.StudentName;
                    var studentPhone = p.StudentPhone;
                    if (p.OrderType != EmLcsPayLogOrderType.StudentAccountRecharge && p.OrderType != EmLcsPayLogOrderType.Activity)
                    {
                        var myStudent = await ComBusiness.GetStudent(tempBoxStudent, _studentDAL, p.RelationId);
                        if (myStudent != null)
                        {
                            studentName = myStudent.Name;
                            studentPhone = myStudent.Phone;
                        }
                    }
                    var isCanRefund = false;
                    if (p.Status == EmLcsPayLogStatus.PaySuccess)
                    {
                        if (p.PayFinishOt.Value > limitRefundDate)
                        {
                            isCanRefund = true;
                        }
                    }
                    output.Add(new TenantLcsPayLogPagingOutput()
                    {
                        CId = p.Id,
                        CreateOt = p.CreateOt,
                        OutTradeNo = p.OutTradeNo,
                        OutRefundNo = p.OutRefundNo,
                        PayFinishOt = p.PayFinishOt,
                        OrderType = p.OrderType,
                        PayType = p.PayType,
                        RefundOt = p.RefundOt,
                        Status = p.Status,
                        StudentId = p.RelationId,
                        StudentName = studentName,
                        StudentPhone = studentPhone,
                        OrderNo = p.OrderNo,
                        OrderSource = p.OrderSource,
                        OrderDesc = p.OrderDesc,
                        OrderTypeDesc = EmLcsPayLogOrderType.GetPayLogOrderTypeDesc(p.OrderType),
                        StatusDesc = EmLcsPayLogStatus.GetPayLogStatus(p.Status),
                        TotalFeeDesc = p.TotalFeeDesc,
                        OrderSourceDesc = EmLcsPayLogOrderSource.GetOrderSourceDesc(p.OrderSource),
                        PayTypeDesc = EmLcsPayType.GetPayTypeDesc(p.PayType),
                        IsCanRefund = isCanRefund,
                        IsLoading = false,
                        AgtPayType = p.AgtPayType,
                        AgtPayTypeDesc = EmAgtPayType.GetAgtPayTypeDesc(p.AgtPayType)
                    });
                }
            }
            return ResponseBase.Success(new ResponsePagingDataBase<TenantLcsPayLogPagingOutput>(pagingData.Item2, output));
        }

        public async Task<ResponseBase> BarCodePay(BarCodePayRequest request)
        {
            var checkTenantLcsAccountResult = await CheckTenantAgtPayAccount(request.LoginTenantId);
            if (!string.IsNullOrEmpty(checkTenantLcsAccountResult.ErrMsg))
            {
                return ResponseBase.CommonError(checkTenantLcsAccountResult.ErrMsg);
            }
            var myTenant = checkTenantLcsAccountResult.MyTenant;
            var myTenantAgtPayAccount = checkTenantLcsAccountResult.MyAgtPayAccountInfo;

            var now = DateTime.Now;
            var orderNo = request.OrderNo;
            if (string.IsNullOrEmpty(orderNo))
            {
                switch (request.OrderType)
                {
                    case EmLcsPayLogOrderType.StudentEnrolment:
                        orderNo = OrderNumberLib.EnrolmentOrderNumber();
                        break;
                    case EmLcsPayLogOrderType.TransferCourse:
                        orderNo = OrderNumberLib.GetTransferCoursesOrderNumber();
                        break;
                    case EmLcsPayLogOrderType.StudentAccountRecharge:
                        orderNo = OrderNumberLib.StudentAccountRecharge();
                        break;
                }
            }
            var payRequest = new BarcodePayRequest()
            {
                AuthNo = request.AuthNo,
                Now = now,
                OrderDesc = request.OrderDesc,
                OrderNo = orderNo,
                PayMoney = request.PayMoney,
                PayMoneyCent = EtmsHelper3.GetCent(request.PayMoney),
                IpAddress = _httpContextAccessor.HttpContext.Connection.RemoteIpAddress.ToString()
            };
            _agtPayServiceBLL.Initialize(checkTenantLcsAccountResult);
            var resPay = await _agtPayServiceBLL.BarcodePay(payRequest);
            if (!resPay.IsSuccess)
            {
                return ResponseBase.CommonError(resPay.ErrMsg);
            }
            var status = resPay.PayResultType;
            DateTime? payFinishOt = null;
            DateTime? payFinishDate = null;
            if (status == EmLcsPayLogStatus.PaySuccess)
            {
                payFinishOt = now;
                payFinishDate = now.Date;
            }
            var lcsPayLogId = 0L;
            if (status != EmLcsPayLogStatus.PayFail)
            {
                lcsPayLogId = await _tenantLcsPayLogDAL.AddTenantLcsPayLog(new EtTenantLcsPayLog()
                {
                    AgentId = myTenant.AgentId,
                    Attach = string.Empty,
                    AuthNo = request.AuthNo,
                    CreateOt = now,
                    IsDeleted = EmIsDeleted.Normal,
                    MerchantName = myTenantAgtPayAccount.MerchantName,
                    MerchantNo = myTenantAgtPayAccount.MerchantNo,
                    MerchantType = myTenantAgtPayAccount.MerchantType,
                    OpenId = string.Empty,
                    OrderBody = request.OrderDesc,
                    OrderDesc = request.OrderDesc,
                    OrderNo = orderNo,
                    OrderSource = EmLcsPayLogOrderSource.PC,
                    OrderType = request.OrderType,
                    OutRefundNo = string.Empty,
                    OutTradeNo = resPay.OutTradeNo,
                    PayFinishOt = payFinishOt,
                    PayType = resPay.PayType,
                    RefundFee = string.Empty,
                    RefundOt = null,
                    Remark = null,
                    Status = status,
                    RelationId = request.StudentId,
                    SubAppid = string.Empty,
                    TenantId = request.LoginTenantId,
                    TerminalId = myTenantAgtPayAccount.TerminalId,
                    TerminalTrace = orderNo,
                    TotalFee = payRequest.PayMoneyCent.ToString(),
                    TotalFeeValue = request.PayMoney,
                    TotalFeeDesc = request.PayMoney.ToString(),
                    PayFinishDate = payFinishDate,
                    RefundDate = null,
                    DataType = EmTenantLcsPayLogDataType.Normal,
                    AgtPayType = myTenant.AgtPayType
                });
                if (status == EmLcsPayLogStatus.PaySuccess)
                {
                    _eventPublisher.Publish(new StatisticsLcsPayEvent(request.LoginTenantId)
                    {
                        StatisticsDate = now.Date
                    });
                }
            }
            return ResponseBase.Success(new BarCodePayOutput()
            {
                OrderNo = orderNo,
                out_trade_no = resPay.OutTradeNo,
                pay_type = resPay.PayType,
                LcsAccountId = lcsPayLogId,
                PayStatus = status
            });
        }

        public async Task<ResponseBase> LcsPayQuery(LcsPayQueryRequest request)
        {
            var checkTenantLcsAccountResult = await CheckTenantAgtPayAccount(request.LoginTenantId);
            if (!string.IsNullOrEmpty(checkTenantLcsAccountResult.ErrMsg))
            {
                return ResponseBase.CommonError(checkTenantLcsAccountResult.ErrMsg);
            }
            var now = DateTime.Now;
            _agtPayServiceBLL.Initialize(checkTenantLcsAccountResult);
            var payResult = await _agtPayServiceBLL.QueryPayLog(new QueryPayLogRequest()
            {
                Now = now,
                OrderNo = request.OrderNo,
                out_trade_no = request.out_trade_no,
                pay_type = request.pay_type,
            });

            if (!payResult.IsSuccess)
            {
                return ResponseBase.CommonError(payResult.ErrMsg);
            }
            var status = payResult.PayResultType;
            DateTime? payFinishOt = null;
            DateTime? payFinishDate = null;
            if (status == EmLcsPayLogStatus.PaySuccess)
            {
                payFinishOt = now;
                payFinishDate = now.Date;
            }
            if (status != EmLcsPayLogStatus.Unpaid)
            {
                var paylog = await _tenantLcsPayLogDAL.GetTenantLcsPayLog(request.LcsAccountId);
                paylog.PayFinishOt = payFinishOt;
                paylog.PayFinishDate = payFinishDate;
                paylog.Status = status;
                await _tenantLcsPayLogDAL.EditTenantLcsPayLog(paylog);
                if (status == EmLcsPayLogStatus.PaySuccess)
                {
                    _eventPublisher.Publish(new StatisticsLcsPayEvent(request.LoginTenantId)
                    {
                        StatisticsDate = now.Date
                    });
                }
            }

            return ResponseBase.Success(new LcsPayQueryOutput()
            {
                PayStatus = status
            });
        }

        public async Task<ResponseBase> LcsPayRefund(LcsPayRefundRequest request)
        {
            var now = DateTime.Now;
            var paylog = await _tenantLcsPayLogDAL.GetTenantLcsPayLog(request.LcsAccountId);
            if (paylog.Status != EmLcsPayLogStatus.PaySuccess)
            {
                return ResponseBase.CommonError("此订单无法执行退款");
            }
            var checkTenantLcsAccountResult = await CheckTenantAgtPayAccount2(request.LoginTenantId, paylog.AgtPayType);
            if (!string.IsNullOrEmpty(checkTenantLcsAccountResult.ErrMsg))
            {
                return ResponseBase.CommonError(checkTenantLcsAccountResult.ErrMsg);
            }

            var limitRefundDate = DateTime.Now.Date.AddDays(-SystemConfig.ComConfig.LcsRefundOrderLimitDay);
            if (paylog.PayFinishOt.Value <= limitRefundDate)
            {
                return ResponseBase.CommonError("此订单超过15天，不能进行退款操作");
            }

            _agtPayServiceBLL.Initialize(checkTenantLcsAccountResult);
            var refundResult = await _agtPayServiceBLL.RefundPay(new RefundPayRequest()
            {
                Now = now,
                Paylog = paylog
            });
            if (!refundResult.IsSuccess)
            {
                return ResponseBase.CommonError(refundResult.ErrMsg);
            }

            paylog.RefundOt = now;
            paylog.RefundDate = now.Date;
            paylog.RefundFee = refundResult.refund_fee;
            paylog.OutRefundNo = refundResult.out_refund_no;
            paylog.Status = refundResult.RefundStatus;
            await _tenantLcsPayLogDAL.EditTenantLcsPayLog(paylog);

            _eventPublisher.Publish(new StatisticsLcsPayEvent(request.LoginTenantId)
            {
                StatisticsDate = now.Date
            });
            if (paylog.PayFinishDate.Value != now.Date)
            {
                _eventPublisher.Publish(new StatisticsLcsPayEvent(request.LoginTenantId)
                {
                    StatisticsDate = paylog.PayFinishDate.Value
                });
            }

            await _userOperationLogDAL.AddUserLog(request, $"退款申请-退款金额:{paylog.TotalFeeDesc},订单号:{paylog.OrderNo}", EmUserOperationType.LcsMgr, now);
            return ResponseBase.Success();
        }

        /// <summary>
        /// 参考：http://help.lcsw.cn/xrmpic/tisnldchblgxohfl/auan62#title-node14
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<LcsPayJspayCallbackOutput> LcsPayJspayCallback(LcsPayJspayCallbackRequest request)
        {
            var strArray = request.attach.Split('_');
            var tenantId = strArray[0].ToInt();
            var lcsPayLogId = strArray[1].ToLong();
            this.InitTenantId(tenantId);
            if (request.return_code == "01")
            {
                if (request.result_code == "01") //支付成功 
                {
                    _eventPublisher.Publish(new ParentBuyMallGoodsPaySuccessEvent(tenantId)
                    {
                        LcsPayLogId = lcsPayLogId,
                        Now = DateTime.Now
                    });
                }
                else if (request.result_code == "02")  //支付失败 
                {
                    var payLog = await _tenantLcsPayLogDAL.GetTenantLcsPayLog(lcsPayLogId);
                    payLog.Status = EmLcsPayLogStatus.PayFail;
                    payLog.DataType = EmTenantLcsPayLogDataType.Normal;
                    await _tenantLcsPayLogDAL.EditTenantLcsPayLog(payLog);
                }
            }
            return new LcsPayJspayCallbackOutput()
            {
                return_code = "01",
                return_msg = "success"
            };
        }

        /// <summary>
        /// 付呗支付回调
        /// https://www.yuque.com/51fubei/openapi/callback_ordercallback
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<string> FubeiApiNotify(FubeiApiNotifyRequest request)
        {
            var result = Newtonsoft.Json.JsonConvert.DeserializeObject<FubeiApiNotifyData>(request.Data);
            var tenantId = result.attach.ToInt();
            this.InitTenantId(tenantId);
            var myTenantFubeiAccount = await _tenantFubeiAccountDAL.GetTenantFubeiAccount(tenantId);
            if (myTenantFubeiAccount == null)
            {
                return "FAIL";
            }
            bool valid = true;
            //if (myTenantFubeiAccount.AccountType == EmFubeiAccountType.Merchant)
            //{
            //    //商户级
            //    valid = FubeiOpenApiCoreSdk.VerifyFubeiNotification(new FubeiApiCommonResult<string>
            //    {
            //        Data = request.Data,
            //        ResultCode = request.ResultCode,
            //        ResultMessage = request.ResultMessage,
            //        AppId = myTenantFubeiAccount.AppId,
            //        AppSecret = myTenantFubeiAccount.AppSecret,
            //        VendorSecret = myTenantFubeiAccount.VendorSecret,
            //        VendorSn = myTenantFubeiAccount.VendorSn
            //    }, request.Sign, EApiLevel.Merchant);
            //}
            //else
            //{
            //    //服务商级
            //    valid = FubeiOpenApiCoreSdk.VerifyFubeiNotification(new FubeiApiCommonResult<string>
            //    {
            //        Data = request.Data,
            //        ResultCode = request.ResultCode,
            //        ResultMessage = request.ResultMessage,
            //        AppId = myTenantFubeiAccount.AppId,
            //        AppSecret = myTenantFubeiAccount.AppSecret,
            //        VendorSecret = myTenantFubeiAccount.VendorSecret,
            //        VendorSn = myTenantFubeiAccount.VendorSn
            //    }, request.Sign, EApiLevel.Vendor);
            //}
            if (valid)
            {
                var payLog = await _tenantLcsPayLogDAL.GetTenantLcsPayLogBuyOutTradeNo(result.order_sn);
                if (payLog == null)
                {
                    return "FAIL";
                }
                var newPayStatus = ComBusiness4.GetFubeiPayStatus(result.order_status);
                if (newPayStatus == EmLcsPayLogStatus.PaySuccess)
                {
                    _eventPublisher.Publish(new ParentBuyMallGoodsPaySuccessEvent(tenantId)
                    {
                        LcsPayLogId = payLog.Id,
                        Now = DateTime.Now
                    });
                }
                else if (newPayStatus == EmLcsPayLogStatus.PayFail)
                {
                    payLog.Status = EmLcsPayLogStatus.PayFail;
                    payLog.DataType = EmTenantLcsPayLogDataType.Normal;
                    await _tenantLcsPayLogDAL.EditTenantLcsPayLog(payLog);
                }
                return "SUCCESS";
            }
            else
            {
                return "FAIL";
            }
        }

        /// <summary>
        /// 付呗退款回调
        /// https://www.yuque.com/51fubei/openapi/callback_refundcallback
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<string> FubeiRefundApiNotify(FubeiApiNotifyRequest request)
        {
            var result = Newtonsoft.Json.JsonConvert.DeserializeObject<FubeiApiNotifyRequestData>(request.Data);
            if (string.IsNullOrEmpty(result.merchant_refund_sn))
            {
                LOG.Log.Fatal("[FubeiRefundApiNotify]付呗退款回调，订单为空", request, this.GetType());
                return "SUCCESS";
            }
            var tenantId = OrderNumberLib.FubeiRefundOrderGetTenantId(result.merchant_refund_sn);
            this.InitTenantId(tenantId);
            var myTenantFubeiAccount = await _tenantFubeiAccountDAL.GetTenantFubeiAccount(tenantId);
            if (myTenantFubeiAccount == null)
            {
                return "FAIL";
            }
            var payLog = await _tenantLcsPayLogDAL.GetTenantLcsPayLogBuyOutTradeNo(result.order_sn);
            if (payLog == null)
            {
                return "FAIL";
            }
            var now = DateTime.Now;
            switch (result.refund_status)
            {
                case EmRefundStatus.Refunding: //退款中
                    return "SUCCESS";
                case EmRefundStatus.Success: //退款成功
                    payLog.RefundOt = now;
                    payLog.RefundDate = now.Date;
                    payLog.RefundFee = EtmsHelper3.GetCent(result.buyer_refund_amount).ToString();
                    payLog.Status = EmLcsPayLogStatus.Refunded;
                    await _tenantLcsPayLogDAL.EditTenantLcsPayLog(payLog);
                    _eventPublisher.Publish(new StatisticsLcsPayEvent(tenantId)
                    {
                        StatisticsDate = now.Date
                    });
                    _eventPublisher.Publish(new StatisticsLcsPayEvent(tenantId)
                    {
                        StatisticsDate = payLog.PayFinishOt.Value.Date
                    });
                    return "SUCCESS";
                case EmRefundStatus.Fail: //退款失败
                    payLog.RefundOt = null;
                    payLog.RefundDate = null;
                    payLog.RefundFee = string.Empty;
                    payLog.Status = EmLcsPayLogStatus.PaySuccess;
                    await _tenantLcsPayLogDAL.EditTenantLcsPayLog(payLog);
                    _eventPublisher.Publish(new StatisticsLcsPayEvent(tenantId)
                    {
                        StatisticsDate = now.Date
                    });
                    _eventPublisher.Publish(new StatisticsLcsPayEvent(tenantId)
                    {
                        StatisticsDate = payLog.PayFinishOt.Value.Date
                    });
                    return "SUCCESS";
            }
            return "FAIL";
        }

        /// <summary>
        /// 随行付支付回调
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public SuixingPayCallbackOutput SuixingPayCallback(SuixingPayCallbackRequest request)
        {
            var strSp = request.extend.Split('_');
            var tenantId = strSp[0].ToInt();
            var myRouteItemId = strSp[1].ToLong();
            this.InitTenantId(tenantId);
            if (request.bizCode == EmBizCode.Success)
            {
                _eventPublisher.Publish(new SuixingPayCallbackEvent(tenantId)
                {
                    ActivityRouteItemId = myRouteItemId,
                    PayTime = DateTime.Now
                });
                return SuixingPayCallbackOutput.Success();
            }
            else
            {
                return SuixingPayCallbackOutput.Fail();
            }
        }

        public SuixingRefundCallbackOutput SuixingRefundCallback(SuixingRefundCallbackRequest request)
        {
            var strSp = request.extend.Split('_');
            var tenantId = strSp[0].ToInt();
            var myRouteItemId = strSp[1].ToLong();
            this.InitTenantId(tenantId);
            if (request.bizCode == EmBizCode.Success)
            {
                _eventPublisher.Publish(new SuixingRefundCallbackEvent(tenantId)
                {
                    ActivityRouteItemId = myRouteItemId,
                    RefundTime = DateTime.Now
                });
                return SuixingRefundCallbackOutput.Success();
            }
            else
            {
                return SuixingRefundCallbackOutput.Fail();
            }
        }
    }
}
