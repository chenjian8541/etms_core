using ETMS.Business.Common;
using ETMS.Entity.Common;
using ETMS.Entity.Database.Source;
using ETMS.Entity.Dto.PaymentService.Output;
using ETMS.Entity.Dto.PaymentService.Request;
using ETMS.Entity.Enum.EtmsManage;
using ETMS.IBusiness;
using ETMS.IDataAccess;
using ETMS.IDataAccess.EtmsManage;
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

        public PaymentBLL(ITenantLcsAccountDAL tenantLcsAccountDAL, IStudentDAL studentDAL)
        {
            this._tenantLcsAccountDAL = tenantLcsAccountDAL;
            this._studentDAL = studentDAL;
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
    }
}
