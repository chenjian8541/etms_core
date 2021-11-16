using ETMS.Business.BaseBLL;
using ETMS.Business.Common;
using ETMS.Entity.Common;
using ETMS.Entity.Config;
using ETMS.Entity.Database.Manage;
using ETMS.Entity.Database.Source;
using ETMS.Entity.Dto.PaymentService.Output;
using ETMS.Entity.Dto.PaymentService.Request;
using ETMS.Entity.Enum;
using ETMS.Entity.Enum.EtmsManage;
using ETMS.Entity.Pay.Lcsw.Dto.Request;
using ETMS.Entity.Temp;
using ETMS.Event.DataContract;
using ETMS.Event.DataContract.Statistics;
using ETMS.IBusiness;
using ETMS.IDataAccess;
using ETMS.IDataAccess.EtmsManage;
using ETMS.IDataAccess.Lcs;
using ETMS.IEventProvider;
using ETMS.Pay.Lcsw;
using ETMS.Utility;
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

        private readonly IPayLcswService _payLcswService;

        private readonly IUserOperationLogDAL _userOperationLogDAL;

        private readonly ITenantLcsPayLogDAL _tenantLcsPayLogDAL;

        private readonly IEventPublisher _eventPublisher;

        public PaymentBLL(ITenantLcsAccountDAL tenantLcsAccountDAL, IStudentDAL studentDAL, ISysTenantDAL sysTenantDAL,
            IPayLcswService payLcswService, IUserOperationLogDAL userOperationLogDAL, ITenantLcsPayLogDAL tenantLcsPayLogDAL,
            IEventPublisher eventPublisher) : base(tenantLcsAccountDAL, sysTenantDAL)
        {
            this._studentDAL = studentDAL;
            this._payLcswService = payLcswService;
            this._userOperationLogDAL = userOperationLogDAL;
            this._tenantLcsPayLogDAL = tenantLcsPayLogDAL;
            this._eventPublisher = eventPublisher;
        }

        public void InitTenantId(int tenantId)
        {
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
                    EtStudent myStudent = null;
                    if (p.OrderType != EmLcsPayLogOrderType.StudentAccountRecharge)
                    {
                        myStudent = await ComBusiness.GetStudent(tempBoxStudent, _studentDAL, p.RelationId);
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
                        StudentName = myStudent?.Name,
                        StudentPhone = myStudent?.Phone,
                        OrderNo = p.OrderNo,
                        OrderSource = p.OrderSource,
                        OrderDesc = p.OrderDesc,
                        OrderTypeDesc = EmLcsPayLogOrderType.GetPayLogOrderTypeDesc(p.OrderType),
                        StatusDesc = EmLcsPayLogStatus.GetPayLogStatus(p.Status),
                        TotalFeeDesc = p.TotalFeeDesc,
                        OrderSourceDesc = EmLcsPayLogOrderSource.GetOrderSourceDesc(p.OrderSource),
                        PayTypeDesc = EmLcsPayType.GetPayTypeDesc(p.PayType),
                        IsCanRefund = isCanRefund,
                        IsLoading = false
                    }); ;
                }
            }
            return ResponseBase.Success(new ResponsePagingDataBase<TenantLcsPayLogPagingOutput>(pagingData.Item2, output));
        }

        public async Task<ResponseBase> BarCodePay(BarCodePayRequest request)
        {
            var checkTenantLcsAccountResult = await CheckTenantLcsAccount(request.LoginTenantId);
            if (!string.IsNullOrEmpty(checkTenantLcsAccountResult.ErrMsg))
            {
                return ResponseBase.CommonError(checkTenantLcsAccountResult.ErrMsg);
            }
            var myTenant = checkTenantLcsAccountResult.MyTenant;
            var myLcsAccount = checkTenantLcsAccountResult.MyLcsAccount;

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
            var payRequest = new RequestBarcodePay()
            {
                access_token = myLcsAccount.AccessToken,
                terminal_id = myLcsAccount.TerminalId,
                terminal_time = ComBusiness4.GetLcsTerminalTime(now),
                payType = EmLcsPayType.AutoTypePay,
                auth_no = request.AuthNo,
                terminal_trace = orderNo,
                attach = request.LoginTenantId.ToString(),
                merchant_no = myLcsAccount.MerchantNo,
                order_body = request.OrderDesc,
                total_fee = EtmsHelper3.GetCent(request.PayMoney).ToString()
            };
            var resPay = _payLcswService.BarcodePay(payRequest);
            if (!resPay.IsRequestSuccess())
            {
                return ResponseBase.CommonError($"扫码支付失败:{resPay.return_msg}");
            }
            var status = EmLcsPayLogStatus.Unpaid;
            DateTime? payFinishOt = null;
            DateTime? payFinishDate = null;
            switch (resPay.result_code)
            {
                case "01": //成功
                    status = EmLcsPayLogStatus.PaySuccess;
                    payFinishOt = now;
                    payFinishDate = now.Date;
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
            var lcsPayLogId = 0L;
            if (status != EmLcsPayLogStatus.PayFail)
            {
                lcsPayLogId = await _tenantLcsPayLogDAL.AddTenantLcsPayLog(new EtTenantLcsPayLog()
                {
                    AgentId = myTenant.AgentId,
                    Attach = payRequest.attach,
                    AuthNo = payRequest.auth_no,
                    CreateOt = now,
                    IsDeleted = EmIsDeleted.Normal,
                    MerchantName = myLcsAccount.MerchantName,
                    MerchantNo = myLcsAccount.MerchantNo,
                    MerchantType = myLcsAccount.MerchantType,
                    OpenId = string.Empty,
                    OrderBody = payRequest.order_body,
                    OrderDesc = request.OrderDesc,
                    OrderNo = orderNo,
                    OrderSource = EmLcsPayLogOrderSource.PC,
                    OrderType = request.OrderType,
                    OutRefundNo = string.Empty,
                    OutTradeNo = resPay.out_trade_no,
                    PayFinishOt = payFinishOt,
                    PayType = resPay.pay_type,
                    RefundFee = string.Empty,
                    RefundOt = null,
                    Remark = null,
                    Status = status,
                    RelationId = request.StudentId,
                    SubAppid = string.Empty,
                    TenantId = request.LoginTenantId,
                    TerminalId = myLcsAccount.TerminalId,
                    TerminalTrace = orderNo,
                    TotalFee = payRequest.total_fee,
                    TotalFeeValue = request.PayMoney,
                    TotalFeeDesc = request.PayMoney.ToString(),
                    PayFinishDate = payFinishDate,
                    RefundDate = null,
                    DataType = EmTenantLcsPayLogDataType.Normal
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
                out_trade_no = resPay.out_trade_no,
                pay_type = resPay.pay_type,
                LcsAccountId = lcsPayLogId,
                PayStatus = status
            });
        }

        public async Task<ResponseBase> LcsPayQuery(LcsPayQueryRequest request)
        {
            var checkTenantLcsAccountResult = await CheckTenantLcsAccount(request.LoginTenantId);
            if (!string.IsNullOrEmpty(checkTenantLcsAccountResult.ErrMsg))
            {
                return ResponseBase.CommonError(checkTenantLcsAccountResult.ErrMsg);
            }
            var myLcsAccount = checkTenantLcsAccountResult.MyLcsAccount;

            var now = DateTime.Now;
            var payResult = _payLcswService.QueryPay(new RequestQuery()
            {
                access_token = myLcsAccount.AccessToken,
                merchant_no = myLcsAccount.MerchantNo,
                out_trade_no = request.out_trade_no,
                payType = request.pay_type,
                terminal_id = myLcsAccount.TerminalId,
                terminal_time = ComBusiness4.GetLcsTerminalTime(now),
                terminal_trace = request.OrderNo
            });
            if (!payResult.IsRequestSuccess())
            {
                return ResponseBase.CommonError($"扫码支付失败:{payResult.return_msg}");
            }
            var status = EmLcsPayLogStatus.Unpaid;
            DateTime? payFinishOt = null;
            DateTime? payFinishDate = null;
            switch (payResult.result_code)
            {
                case "01": //成功
                    status = EmLcsPayLogStatus.PaySuccess;
                    payFinishOt = now;
                    payFinishDate = now.Date;
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
            var checkTenantLcsAccountResult = await CheckTenantLcsAccount(request.LoginTenantId);
            if (!string.IsNullOrEmpty(checkTenantLcsAccountResult.ErrMsg))
            {
                return ResponseBase.CommonError(checkTenantLcsAccountResult.ErrMsg);
            }
            var myLcsAccount = checkTenantLcsAccountResult.MyLcsAccount;

            var now = DateTime.Now;
            var paylog = await _tenantLcsPayLogDAL.GetTenantLcsPayLog(request.LcsAccountId);
            if (paylog.Status != EmLcsPayLogStatus.PaySuccess)
            {
                return ResponseBase.CommonError("此订单无法执行退款");
            }
            var limitRefundDate = DateTime.Now.Date.AddDays(-SystemConfig.ComConfig.LcsRefundOrderLimitDay);
            if (paylog.PayFinishOt.Value <= limitRefundDate)
            {
                return ResponseBase.CommonError("此订单超过15天，不能进行退款操作");
            }

            var refundResult = _payLcswService.RefundPay(new RequestRefund()
            {
                access_token = myLcsAccount.AccessToken,
                merchant_no = myLcsAccount.MerchantNo,
                out_trade_no = paylog.OutTradeNo,
                payType = paylog.PayType,
                refund_fee = paylog.TotalFee,
                terminal_id = myLcsAccount.TerminalId,
                terminal_time = ComBusiness4.GetLcsTerminalTime(now),
                terminal_trace = paylog.OrderNo
            });
            if (!refundResult.IsSuccess(true))
            {
                return ResponseBase.CommonError(refundResult.return_msg);
            }

            paylog.RefundOt = now;
            paylog.RefundDate = now.Date;
            paylog.RefundFee = refundResult.refund_fee;
            paylog.OutRefundNo = refundResult.out_refund_no;
            paylog.Status = EmLcsPayLogStatus.Refunded;
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
    }
}
