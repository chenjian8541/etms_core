using ETMS.Business.Common;
using ETMS.Entity.Common;
using ETMS.Entity.Database.Source;
using ETMS.Entity.Dto.HisData.Output;
using ETMS.Entity.Dto.HisData.Request;
using ETMS.Entity.Enum;
using ETMS.IBusiness;
using ETMS.IDataAccess;
using ETMS.Utility;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using ETMS.IEventProvider;
using ETMS.Event.DataContract;
using ETMS.LOG;

namespace ETMS.Business
{
    public class OrderBLL : IOrderBLL
    {
        private readonly IOrderDAL _orderDAL;

        private readonly IStudentDAL _studentDAL;

        private readonly IUserDAL _userDAL;

        private readonly IIncomeLogDAL _incomeLogDAL;

        private readonly ICouponsDAL _couponsDAL;

        private readonly ICostDAL _costDAL;

        private readonly ICourseDAL _courseDAL;

        private readonly IGoodsDAL _goodsDAL;

        private readonly IUserOperationLogDAL _userOperationLogDAL;

        private readonly IEventPublisher _eventPublisher;

        public OrderBLL(IOrderDAL orderDAL, IStudentDAL studentDAL, IUserDAL userDAL, IIncomeLogDAL incomeLogDAL, ICouponsDAL couponsDAL,
            ICostDAL costDAL, ICourseDAL courseDAL, IGoodsDAL goodsDAL, IUserOperationLogDAL userOperationLogDAL, IEventPublisher eventPublisher)
        {
            this._orderDAL = orderDAL;
            this._studentDAL = studentDAL;
            this._userDAL = userDAL;
            this._incomeLogDAL = incomeLogDAL;
            this._couponsDAL = couponsDAL;
            this._costDAL = costDAL;
            this._courseDAL = courseDAL;
            this._goodsDAL = goodsDAL;
            this._userOperationLogDAL = userOperationLogDAL;
            this._eventPublisher = eventPublisher;
        }

        public void InitTenantId(int tenantId)
        {
            this.InitDataAccess(tenantId, _orderDAL, _studentDAL, _userDAL, _incomeLogDAL,
                _couponsDAL, _costDAL, _courseDAL, _goodsDAL, _userOperationLogDAL);
        }

        public async Task<ResponseBase> OrderGetPaging(OrderGetPagingRequest request)
        {
            var pagingData = await _orderDAL.GetOrderPaging(request);
            var orderOutput = new List<OrderGetPagingOutput>();
            var tempBoxUser = new DataTempBox<EtUser>();
            foreach (var p in pagingData.Item1)
            {
                var student = await _studentDAL.GetStudent(p.StudentId);
                orderOutput.Add(new OrderGetPagingOutput()
                {
                    AptSum = p.AptSum,
                    ArrearsSum = p.ArrearsSum,
                    BuyCost = p.BuyCost,
                    BuyCourse = p.BuyCourse,
                    BuyGoods = p.BuyGoods,
                    CommissionUser = p.CommissionUser,
                    CommissionUserDesc = await GetCommissionUserDesc(tempBoxUser, p.CommissionUser),
                    No = p.No,
                    OrderType = p.OrderType,
                    OtDesc = p.Ot.EtmsToDateString(),
                    PaySum = p.PaySum,
                    Remark = p.Remark,
                    Status = p.Status,
                    StatusDesc = EmOrderStatus.GetOrderStatus(p.Status),
                    StudentId = p.StudentId,
                    StudentName = student.Student.Name,
                    StudentPhone = student.Student.Phone,
                    Sum = p.Sum,
                    TotalPoints = p.TotalPoints,
                    UserId = p.UserId,
                    UserName = await ComBusiness.GetUserName(tempBoxUser, _userDAL, p.UserId),
                    CId = p.Id
                });
            }
            return ResponseBase.Success(new ResponsePagingDataBase<OrderGetPagingOutput>(pagingData.Item2, orderOutput));
        }

        private async Task<string> GetCommissionUserDesc(DataTempBox<EtUser> tempbox, string commissionUser)
        {
            if (string.IsNullOrEmpty(commissionUser))
            {
                return string.Empty;
            }
            var userIds = commissionUser.Split(',');
            var strUserName = new StringBuilder();
            foreach (var id in userIds)
            {
                if (string.IsNullOrEmpty(id))
                {
                    continue;
                }
                var tempName = await ComBusiness.GetUserName(tempbox, _userDAL, id.ToLong());
                if (string.IsNullOrEmpty(tempName))
                {
                    continue;
                }
                strUserName.Append($"{tempName},");
            }
            return strUserName.ToString().TrimEnd(',');
        }

        public async Task<ResponseBase> OrderGetDetail(OrderGetDetailRequest request)
        {
            var order = await _orderDAL.GetOrder(request.CId);
            if (order == null)
            {
                return ResponseBase.CommonError("订单不存在");
            }
            var output = new OrderGetDetailOutput();
            var tempBoxUser = new DataTempBox<EtUser>();
            var student = (await _studentDAL.GetStudent(order.StudentId)).Student;
            var commissionUsers = await ComBusiness.GetUserMultiSelectValue(tempBoxUser, _userDAL, order.CommissionUser);
            output.BascInfo = new OrderGetDetailBascInfo()
            {
                ArrearsSum = order.ArrearsSum,
                BuyCost = order.BuyCost,
                CId = order.Id,
                AptSum = order.AptSum,
                BuyCourse = order.BuyCourse,
                BuyGoods = order.BuyGoods,
                CommissionUser = order.CommissionUser,
                CommissionUserDesc = string.Join(',', commissionUsers.Select(p => p.Label)),
                No = order.No,
                OrderType = order.OrderType,
                OtDesc = order.Ot.EtmsToDateString(),
                PaySum = order.PaySum,
                Remark = order.Remark,
                Status = order.Status,
                StatusDesc = EmOrderStatus.GetOrderStatus(order.Status),
                StudentId = order.StudentId,
                StudentName = student.Name,
                StudentPhone = student.Phone,
                Sum = order.Sum,
                TotalPoints = order.TotalPoints,
                UserId = order.UserId,
                UserName = await ComBusiness.GetUserName(tempBoxUser, _userDAL, order.UserId),
                CreateOt = order.CreateOt,
                CommissionUserIds = commissionUsers
            };
            if (!string.IsNullOrEmpty(order.CouponsIds) && !string.IsNullOrEmpty(order.CouponsStudentGetIds))
            {
                output.OrderGetDetailCoupons = new List<OrderGetDetailCoupons>();
                var couponsIds = order.CouponsIds.Split(',');
                foreach (var couponsId in couponsIds)
                {
                    if (string.IsNullOrEmpty(couponsId))
                    {
                        continue;
                    }
                    var myCoupons = await _couponsDAL.GetCoupons(couponsId.ToLong());
                    if (myCoupons == null)
                    {
                        continue;
                    }
                    output.OrderGetDetailCoupons.Add(new OrderGetDetailCoupons()
                    {
                        CId = myCoupons.Id,
                        CouponsMinLimit = myCoupons.MinLimit,
                        CouponsTitle = myCoupons.Title,
                        CouponsType = myCoupons.Type,
                        CouponsTypeDesc = EmCouponsType.GetCouponsTypeDesc(myCoupons.Type),
                        CouponsValue = myCoupons.Value,
                        CouponsValueDesc = ComBusiness.GetCouponsValueDesc2(myCoupons.Type, myCoupons.Value),
                        MinLimitDesc = myCoupons.MinLimit == null || myCoupons.MinLimit == 0 ? "无门槛" : $"消费满{myCoupons.MinLimit.Value.ToDecimalDesc()}元可用"
                    });
                }
            }
            var orderDetail = await _orderDAL.GetOrderDetail(request.CId);
            output.OrderGetDetailProducts = new List<OrderGetDetailProductInfo>();
            foreach (var myItem in orderDetail)
            {
                var productName = string.Empty;
                switch (myItem.ProductType)
                {
                    case EmOrderProductType.Cost:
                        var myCost = await _costDAL.GetCost(myItem.ProductId);
                        productName = myCost?.Name;
                        break;
                    case EmOrderProductType.Goods:
                        var myGoods = await _goodsDAL.GetGoods(myItem.ProductId);
                        productName = myGoods?.Name;
                        break;
                    case EmOrderProductType.Course:
                        var myCourse = await _courseDAL.GetCourse(myItem.ProductId);
                        productName = myCourse?.Item1.Name;
                        break;
                }
                output.OrderGetDetailProducts.Add(new OrderGetDetailProductInfo()
                {
                    BugUnit = myItem.BugUnit,
                    BuyQuantity = myItem.BuyQuantity,
                    BuyQuantityDesc = ComBusiness.GetBuyQuantityDesc(myItem.BuyQuantity, myItem.BugUnit),
                    DiscountDesc = ComBusiness.GetDiscountDesc(myItem.DiscountValue, myItem.DiscountType),
                    GiveQuantityDesc = ComBusiness.GetGiveQuantityDesc(myItem.GiveQuantity, myItem.GiveUnit),
                    ItemAptSum = myItem.ItemAptSum,
                    ItemSum = myItem.ItemSum,
                    PriceRule = myItem.PriceRule,
                    ProductTypeDesc = EmOrderProductType.GetOrderProductType(myItem.ProductType),
                    ProductName = productName,
                    CId = myItem.Id
                });
            }
            var payLog = await _incomeLogDAL.GetIncomeLogByOrderId(request.CId);
            output.OrderGetDetailIncomeLogs = new List<OrderGetDetailIncomeLog>();
            if (payLog != null && payLog.Any())
            {
                foreach (var p in payLog)
                {
                    output.OrderGetDetailIncomeLogs.Add(new OrderGetDetailIncomeLog()
                    {
                        PayOt = p.Ot.EtmsToDateString(),
                        PayType = p.PayType,
                        PayTypeDesc = EmPayType.GetPayType(p.PayType),
                        ProjectType = p.ProjectType,
                        ProjectTypeName = EmIncomeLogProjectType.GetIncomeLogProjectType(p.ProjectType),
                        Sum = p.Sum,
                        UserName = await ComBusiness.GetUserName(tempBoxUser, _userDAL, p.UserId),
                        CId = p.Id
                    });
                }
            }
            return ResponseBase.Success(output);
        }

        public async Task<ResponseBase> OrderGetSimpleDetail(OrderGetSimpleDetailRequest request)
        {
            var order = await _orderDAL.GetOrder(request.CId);
            if (order == null)
            {
                return ResponseBase.CommonError("订单不存在");
            }
            var output = new OrderGetSimpleDetailOutput();
            var tempBoxUser = new DataTempBox<EtUser>();
            var student = (await _studentDAL.GetStudent(order.StudentId)).Student;
            output.BascInfo = new OrderGetDetailBascInfo()
            {
                ArrearsSum = order.ArrearsSum,
                BuyCost = order.BuyCost,
                CId = order.Id,
                AptSum = order.AptSum,
                BuyCourse = order.BuyCourse,
                BuyGoods = order.BuyGoods,
                CommissionUser = order.CommissionUser,
                CommissionUserDesc = await ComBusiness.GetUserNames(tempBoxUser, _userDAL, order.CommissionUser),
                No = order.No,
                OrderType = order.OrderType,
                OtDesc = order.Ot.EtmsToDateString(),
                PaySum = order.PaySum,
                Remark = order.Remark,
                Status = order.Status,
                StatusDesc = EmOrderStatus.GetOrderStatus(order.Status),
                StudentId = order.StudentId,
                StudentName = student.Name,
                StudentPhone = student.Phone,
                Sum = order.Sum,
                TotalPoints = order.TotalPoints,
                UserId = order.UserId,
                UserName = await ComBusiness.GetUserName(tempBoxUser, _userDAL, order.UserId),
                CreateOt = order.CreateOt
            };
            var payLog = await _incomeLogDAL.GetIncomeLogByOrderId(request.CId);
            output.OrderGetDetailIncomeLogs = new List<OrderGetDetailIncomeLog>();
            if (payLog != null && payLog.Any())
            {
                foreach (var p in payLog)
                {
                    output.OrderGetDetailIncomeLogs.Add(new OrderGetDetailIncomeLog()
                    {
                        PayOt = p.Ot.EtmsToDateString(),
                        PayType = p.PayType,
                        PayTypeDesc = EmPayType.GetPayType(p.PayType),
                        ProjectType = p.ProjectType,
                        ProjectTypeName = EmIncomeLogProjectType.GetIncomeLogProjectType(p.ProjectType),
                        Sum = p.Sum,
                        UserName = await ComBusiness.GetUserName(tempBoxUser, _userDAL, p.UserId)
                    });
                }
            }
            return ResponseBase.Success(output);
        }

        public async Task<ResponseBase> OrderPayment(OrderPaymentRequest request)
        {
            var order = await _orderDAL.GetOrder(request.OrderId);
            if (order == null)
            {
                return ResponseBase.CommonError("订单不存在");
            }
            if (order.Status != EmOrderStatus.MakeUpMoney && order.Status != EmOrderStatus.Unpaid)
            {
                return ResponseBase.CommonError("此订单无须补交费用");
            }
            var payTotal = request.PayWechat + request.PayAlipay + request.PayCash + request.PayBank + request.PayPos;
            if (order.ArrearsSum < payTotal)
            {
                return ResponseBase.CommonError("支付金额不能大于欠款金额");
            }
            var now = DateTime.Now;
            var incomeLogs = new List<EtIncomeLog>();
            if (request.PayWechat > 0)
            {
                incomeLogs.Add(GetEtIncomeLog(EmPayType.WeChat, request.PayWechat, now, request.PayOt, order.No, order.Id, request));
            }
            if (request.PayAlipay > 0)
            {
                incomeLogs.Add(GetEtIncomeLog(EmPayType.Alipay, request.PayAlipay, now, request.PayOt, order.No, order.Id, request));
            }
            if (request.PayCash > 0)
            {
                incomeLogs.Add(GetEtIncomeLog(EmPayType.Cash, request.PayCash, now, request.PayOt, order.No, order.Id, request));
            }
            if (request.PayBank > 0)
            {
                incomeLogs.Add(GetEtIncomeLog(EmPayType.Bank, request.PayBank, now, request.PayOt, order.No, order.Id, request));
            }
            if (request.PayPos > 0)
            {
                incomeLogs.Add(GetEtIncomeLog(EmPayType.Pos, request.PayPos, now, request.PayOt, order.No, order.Id, request));
            }
            _incomeLogDAL.AddIncomeLog(incomeLogs);
            var newArrearsSum = order.ArrearsSum - payTotal;
            var newStatus = EmOrderStatus.MakeUpMoney;
            if (newArrearsSum == 0)
            {
                newStatus = EmOrderStatus.Normal;
            }
            var newPaySum = order.PaySum + payTotal;
            order.ArrearsSum = newArrearsSum;
            order.Status = newStatus;
            order.PaySum = newPaySum;
            await _orderDAL.UpdateOrder(order);
            _eventPublisher.Publish(new StatisticsFinanceIncomeEvent(request.LoginTenantId)
            {
                StatisticsDate = request.PayOt
            });
            var opLog = new EtOrderOperationLog()
            {
                IsDeleted = EmIsDeleted.Normal,
                OpContent = $"补交费用-补交{payTotal}元",
                OpType = EmOrderOperationLogType.CollectMoney,
                OrderId = order.Id,
                OrderNo = order.No,
                Ot = now,
                Remark = string.Empty,
                TenantId = request.LoginTenantId,
                UserId = request.LoginUserId
            };
            await _orderDAL.AddOrderOperationLog(opLog);
            await _userOperationLogDAL.AddUserLog(request, $"补交报名费用，订单号:{order.No},补交金额:{payTotal}", EmUserOperationType.StudentEnrolmentAddPay, now);
            return ResponseBase.Success();
        }

        private EtIncomeLog GetEtIncomeLog(byte payType, decimal payValue, DateTime createTime, DateTime ot, string no, long orderId, OrderPaymentRequest request)
        {
            return new EtIncomeLog()
            {
                AccountNo = string.Empty,
                IsDeleted = EmIsDeleted.Normal,
                No = no,
                Ot = ot,
                PayType = payType,
                ProjectType = EmIncomeLogProjectType.StudentEnrolmentAddPay,
                Remark = request.Remark,
                RepealOt = null,
                OrderId = orderId,
                RepealUserId = null,
                Status = EmIncomeLogStatus.Normal,
                Sum = payValue,
                TenantId = request.LoginTenantId,
                Type = EmIncomeLogType.AccountIn,
                UserId = request.LoginUserId,
                CreateOt = createTime
            };
        }

        public async Task<ResponseBase> OrderEditRemark(OrderEditRemarkRequest request)
        {
            var order = await _orderDAL.GetOrder(request.CId);
            if (order == null)
            {
                return ResponseBase.CommonError("订单不存在");
            }
            if (order.Status == EmOrderStatus.Repeal)
            {
                return ResponseBase.CommonError("此订单已作废，无法编辑");
            }
            var now = DateTime.Now;
            order.Remark = request.NewRemark;
            await _orderDAL.UpdateOrder(order);
            var opLog = new EtOrderOperationLog()
            {
                IsDeleted = EmIsDeleted.Normal,
                OpContent = $"编辑备注-{request.NewRemark}",
                OpType = EmOrderOperationLogType.ModifyRemark,
                OrderId = order.Id,
                OrderNo = order.No,
                Ot = now,
                Remark = string.Empty,
                TenantId = request.LoginTenantId,
                UserId = request.LoginUserId
            };
            await _orderDAL.AddOrderOperationLog(opLog);
            await _userOperationLogDAL.AddUserLog(request, $"编辑订单备注,订单号:{order.No},新备注:{request.NewRemark}", EmUserOperationType.OrderMgr, now);
            return ResponseBase.Success();
        }

        public async Task<ResponseBase> OrderEditCommission(OrderEditCommissionRequest request)
        {
            var order = await _orderDAL.GetOrder(request.CId);
            if (order == null)
            {
                return ResponseBase.CommonError("订单不存在");
            }
            if (order.Status == EmOrderStatus.Repeal)
            {
                return ResponseBase.CommonError("此订单已作废，无法编辑");
            }
            var newNames = string.Empty;
            var newIds = string.Empty;
            if (request.NewCommissionUsers != null && request.NewCommissionUsers.Any())
            {
                var tempIds = new StringBuilder();
                var tempNames = new StringBuilder();
                foreach (var p in request.NewCommissionUsers)
                {
                    tempIds.Append($"{p.Value},");
                    tempNames.Append($"{p.Label},");
                }
                newIds = $",{tempIds.ToString()}";
                newNames = tempNames.ToString().TrimEnd(',');
            }
            order.CommissionUser = newIds;
            await _orderDAL.UpdateOrder(order);
            var now = DateTime.Now;
            var opLog = new EtOrderOperationLog()
            {
                IsDeleted = EmIsDeleted.Normal,
                OpContent = $"修改业绩归属人-{newNames}",
                OpType = EmOrderOperationLogType.ModifyCommissionUser,
                OrderId = order.Id,
                OrderNo = order.No,
                Ot = now,
                Remark = string.Empty,
                TenantId = request.LoginTenantId,
                UserId = request.LoginUserId
            };
            await _orderDAL.AddOrderOperationLog(opLog);
            await _userOperationLogDAL.AddUserLog(request, $"修改业绩归属人,订单号:{order.No},业绩归属人:{newNames}", EmUserOperationType.OrderMgr, now);
            return ResponseBase.Success();
        }

        public async Task<ResponseBase> OrderOperationLogGetPaging(OrderOperationLogGetPagingRequest request)
        {
            var pagingData = await _orderDAL.GetOrderOperationLogPaging(request);
            var output = new List<OrderOperationLogGetPagingOutput>();
            if (pagingData.Item1.Any())
            {
                var tempBoxUser = new DataTempBox<EtUser>();
                foreach (var p in pagingData.Item1)
                {
                    output.Add(new OrderOperationLogGetPagingOutput()
                    {
                        CId = p.Id,
                        OpContent = p.OpContent,
                        OpType = p.OpType,
                        OpTypeDesc = EmOrderOperationLogType.GetOrderOperationLogTypeDesc(p.OpType),
                        Ot = p.Ot,
                        UserId = p.UserId,
                        UserDesc = await ComBusiness.GetUserName(tempBoxUser, _userDAL, p.UserId)
                    });
                }
            }
            return ResponseBase.Success(new ResponsePagingDataBase<OrderOperationLogGetPagingOutput>(pagingData.Item2, output));
        }

        public async Task<ResponseBase> OrderRepeal(OrderRepealRequest request)
        {
            var order = await _orderDAL.GetOrder(request.OrderId);
            if (order == null)
            {
                return ResponseBase.CommonError("订单不存在");
            }
            if (order.Status == EmOrderStatus.Repeal)
            {
                return ResponseBase.CommonError("此订单已作废");
            }
            switch (order.OrderType)
            {
                case EmOrderType.StudentEnrolment:
                    return await OrderStudentEnrolmentRepeal(request);
                case EmOrderType.ReturnCourse:
                    return await OrderReturnCourseRepeal(request, order);
                case EmOrderType.ReturnGoods:
                    return await OrderReturnGoodsRepeal(request, order);
                case EmOrderType.ReturnCost:
                    return await OrderReturnCostRepeal(request, order);
            }
            Log.Error($"异常订单无法作废:{EtmsHelper.EtmsSerializeObject(request)}", this.GetType());
            return ResponseBase.CommonError("异常订单，无法作废");
        }

        private async Task<ResponseBase> OrderStudentEnrolmentRepeal(OrderRepealRequest request)
        {
            await _orderDAL.OrderStudentEnrolmentRepeal(request.OrderId);
            _eventPublisher.Publish(new OrderStudentEnrolmentRepealEvent(request.LoginTenantId)
            {
                OrderId = request.OrderId,
                UserId = request.LoginUserId,
                Remark = request.Remark,
                LoginClientType = request.LoginClientType
            });
            return ResponseBase.Success();
        }

        private async Task<ResponseBase> OrderReturnCourseRepeal(OrderRepealRequest request, EtOrder order)
        { return ResponseBase.Success(); }

        private async Task<ResponseBase> OrderReturnGoodsRepeal(OrderRepealRequest request, EtOrder order)
        { return ResponseBase.Success(); }

        private async Task<ResponseBase> OrderReturnCostRepeal(OrderRepealRequest request, EtOrder order)
        {
            return ResponseBase.Success();
        }

    }
}
