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
using ETMS.IBusiness;
using ETMS.IDataAccess;
using ETMS.IDataAccess.EtmsManage;
using ETMS.Pay.Lcsw;
using ETMS.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.Business
{
    public class PaymentBLL : IPaymentBLL
    {
        private readonly ITenantLcsAccountDAL _tenantLcsAccountDAL;

        private readonly IStudentDAL _studentDAL;

        private readonly ISysTenantDAL _sysTenantDAL;

        private readonly IPayLcswService _payLcswService;

        private readonly IUserOperationLogDAL _userOperationLogDAL;

        public PaymentBLL(ITenantLcsAccountDAL tenantLcsAccountDAL, IStudentDAL studentDAL, ISysTenantDAL sysTenantDAL,
            IPayLcswService payLcswService, IUserOperationLogDAL userOperationLogDAL)
        {
            this._tenantLcsAccountDAL = tenantLcsAccountDAL;
            this._studentDAL = studentDAL;
            this._sysTenantDAL = sysTenantDAL;
            this._payLcswService = payLcswService;
            this._userOperationLogDAL = userOperationLogDAL;
        }

        public void InitTenantId(int tenantId)
        {
            this.InitDataAccess(tenantId, _studentDAL, _userOperationLogDAL);
        }

        public async Task<ResponseBase> TenantLcsPayLogPaging(TenantLcsPayLogPagingRequest request)
        {
            var pagingData = await _tenantLcsAccountDAL.GetTenantLcsPayLogPaging(request);
            var output = new List<TenantLcsPayLogPagingOutput>();
            if (pagingData.Item1.Any())
            {
                var tempBoxStudent = new DataTempBox<EtStudent>();
                var limitRefundDate = DateTime.Now.Date.AddDays(-SystemConfig.ComConfig.LcsRefundOrderLimitDay);
                foreach (var p in pagingData.Item1)
                {
                    var myStudent = await ComBusiness.GetStudent(tempBoxStudent, _studentDAL, p.StudentId);
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
                        CreateOt = p.CreateOt,
                        OutTradeNo = p.OutTradeNo,
                        OutRefundNo = p.OutRefundNo,
                        PayFinishOt = p.PayFinishOt,
                        OrderType = p.OrderType,
                        PayType = p.PayType,
                        RefundOt = p.RefundOt,
                        Status = p.Status,
                        StudentId = p.StudentId,
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
                        IsCanRefund = isCanRefund
                    }); ;
                }
            }
            return ResponseBase.Success(new ResponsePagingDataBase<TenantLcsPayLogPagingOutput>(pagingData.Item2, output));
        }

        private async Task<CheckTenantLcsAccountView> CheckTenantLcsAccount(int tenantId)
        {
            var myTenant = await _sysTenantDAL.GetTenant(tenantId);
            var isOpenLcsPay = ComBusiness4.GetIsOpenLcsPay(myTenant.LcswApplyStatus, myTenant.LcswOpenStatus);
            if (!isOpenLcsPay)
            {
                return new CheckTenantLcsAccountView("机构未开通扫呗支付");
            }
            var myLcsAccount = await _tenantLcsAccountDAL.GetTenantLcsAccount(myTenant.Id);
            if (myLcsAccount == null)
            {
                LOG.Log.Error($"[CheckTenantLcsAccount]扫呗账户异常tenantId:{tenantId}", this.GetType());
                return new CheckTenantLcsAccountView("扫呗账户异常，无法支付");
            }
            return new CheckTenantLcsAccountView()
            {
                MyTenant = myTenant,
                MyLcsAccount = myLcsAccount,
                ErrMsg = null
            };
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
            var orderNo = string.Empty;
            switch (request.OrderType)
            {
                case EmLcsPayLogOrderType.StudentEnrolment:
                    orderNo = OrderNumberLib.EnrolmentOrderNumber();
                    break;
                case EmLcsPayLogOrderType.StudentAccountRecharge:
                    orderNo = OrderNumberLib.StudentAccountRecharge();
                    break;
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
            if (!resPay.IsSuccess())
            {
                return ResponseBase.CommonError($"扫码支付失败:{resPay.return_msg}");
            }
            var lcsPayLogId = await _tenantLcsAccountDAL.AddTenantLcsPayLog(new SysTenantLcsPayLog()
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
                PayFinishOt = null,
                PayType = resPay.pay_type,
                RefundFee = string.Empty,
                RefundOt = null,
                Remark = null,
                Status = EmLcsPayLogStatus.Unpaid,
                StudentId = request.StudentId,
                SubAppid = string.Empty,
                TenantId = request.LoginTenantId,
                TerminalId = myLcsAccount.TerminalId,
                TerminalTrace = orderNo,
                TotalFee = payRequest.total_fee,
                TotalFeeDesc = request.PayMoney.ToString()
            });
            return ResponseBase.Success(new BarCodePayOutput()
            {
                OrderNo = orderNo,
                out_trade_no = resPay.out_trade_no,
                pay_type = resPay.pay_type,
                LcsAccountId = lcsPayLogId
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
            if (!payResult.IsSuccess(true))
            {
                if (payResult.result_code == "02")
                {
                    return ResponseBase.Success(new LcsPayQueryOutput()
                    {
                        PayStatus = EmLcsPayQueryPayStatus.Fail
                    });
                }
                return ResponseBase.Success(new LcsPayQueryOutput()
                {
                    PayStatus = EmLcsPayQueryPayStatus.Paying
                });
            }
            var paylog = await _tenantLcsAccountDAL.GetTenantLcsPayLog(request.LcsAccountId);
            paylog.PayFinishOt = now;
            paylog.Status = EmLcsPayLogStatus.PaySuccess;
            await _tenantLcsAccountDAL.EditTenantLcsPayLog(paylog);

            return ResponseBase.Success(new LcsPayQueryOutput()
            {
                PayStatus = EmLcsPayQueryPayStatus.Success
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
            var paylog = await _tenantLcsAccountDAL.GetTenantLcsPayLog(request.LcsAccountId);
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
            paylog.RefundFee = refundResult.refund_fee;
            paylog.OutRefundNo = refundResult.out_refund_no;
            paylog.Status = EmLcsPayLogStatus.Refunded;
            await _tenantLcsAccountDAL.EditTenantLcsPayLog(paylog);

            await _userOperationLogDAL.AddUserLog(request, $"退款申请-退款金额:{paylog.TotalFeeDesc},订单号:{paylog.OrderNo}", EmUserOperationType.LcsMgr, now);
            return ResponseBase.Success();
        }
    }
}
