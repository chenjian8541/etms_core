using ETMS.Business.Common;
using ETMS.Entity.Common;
using ETMS.Entity.Database.Manage;
using ETMS.Entity.Database.Source;
using ETMS.Entity.Dto.PaymentService.Output;
using ETMS.Entity.Dto.PaymentService.Request;
using ETMS.Entity.Enum;
using ETMS.Entity.Enum.EtmsManage;
using ETMS.Entity.Pay.Lcsw.Dto.Request;
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

        public PaymentBLL(ITenantLcsAccountDAL tenantLcsAccountDAL, IStudentDAL studentDAL, ISysTenantDAL sysTenantDAL,
            IPayLcswService payLcswService)
        {
            this._tenantLcsAccountDAL = tenantLcsAccountDAL;
            this._studentDAL = studentDAL;
            this._sysTenantDAL = sysTenantDAL;
            this._payLcswService = payLcswService;
        }

        public void InitTenantId(int tenantId)
        {
            this.InitDataAccess(tenantId, _studentDAL);
        }

        public async Task<ResponseBase> TenantLcsPayLogPaging(TenantLcsPayLogPagingRequest request)
        {
            var pagingData = await _tenantLcsAccountDAL.GetTenantLcsPayLogPaging(request);
            var output = new List<TenantLcsPayLogPagingOutput>();
            if (pagingData.Item1.Any())
            {
                var tempBoxStudent = new DataTempBox<EtStudent>();
                var now = DateTime.Now.Date.AddDays(-15); //限支付15天内退款，超过15天，不能进行退款操作
                foreach (var p in pagingData.Item1)
                {
                    var myStudent = await ComBusiness.GetStudent(tempBoxStudent, _studentDAL, p.StudentId);
                    var isCanRefund = false;
                    if (p.Status == EmLcsPayLogStatus.PaySuccess)
                    {
                        if (p.PayFinishOt > now)
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

        public async Task<ResponseBase> BarCodePay(BarCodePayRequest request)
        {
            var myTenant = await _sysTenantDAL.GetTenant(request.LoginTenantId);
            var isOpenLcsPay = ComBusiness4.GetIsOpenLcsPay(myTenant.LcswApplyStatus, myTenant.LcswOpenStatus);
            if (!isOpenLcsPay)
            {
                return ResponseBase.CommonError("机构未开通扫呗支付");
            }
            var myLcsAccount = await _tenantLcsAccountDAL.GetTenantLcsAccount(myTenant.Id);
            if (myLcsAccount == null)
            {
                LOG.Log.Error("[BarCodePay]扫呗账户异常", request, this.GetType());
                return ResponseBase.CommonError("扫呗账户异常，无法支付");
            }
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
                accessToken = myLcsAccount.AccessToken,
                terminal_id = myLcsAccount.TerminalId,
                terminal_time = ComBusiness4.GetLcsTerminalTime(now),
                payType = EmLcsPayType.AutoTypePay,
                auth_no = request.AuthNo,
                terminal_trace = orderNo,
                attach = request.LoginTenantId.ToString(),
                merchant_no = myLcsAccount.MerchantNo,
                order_body = request.OrderDesc,
                total_fee = EtmsHelper3.GetCent(request.PayMoney).ToString(),
                goods_detail = string.Empty
            };
            var resPay = _payLcswService.BarcodePay(payRequest);
            if (!resPay.IsSuccess())
            {
                return ResponseBase.CommonError($"扫码支付失败:{resPay.return_msg}");
            }
            await _tenantLcsAccountDAL.AddTenantLcsPayLog(new SysTenantLcsPayLog()
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
                TotalFeeDesc = request.PayMoney.ToString(),
            });
            return ResponseBase.Success(new BarCodePayOutput()
            {
                OrderNo = orderNo,
                out_trade_no = resPay.out_trade_no,
                pay_type = resPay.pay_type
            });
        }
    }
}
