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
using ETMS.Entity.Config;
using ETMS.Entity.Temp;

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

        private readonly IStudentCourseDAL _studentCourseDAL;

        private readonly IStudentAccountRechargeDAL _studentAccountRechargeDAL;

        private readonly IStudentAccountRechargeLogDAL _studentAccountRechargeLogDAL;

        private readonly IParentStudentDAL _parentStudentDAL;

        private readonly IStudentAccountRechargeCoreBLL _studentAccountRechargeCoreBLL;

        public OrderBLL(IOrderDAL orderDAL, IStudentDAL studentDAL, IUserDAL userDAL, IIncomeLogDAL incomeLogDAL, ICouponsDAL couponsDAL,
            ICostDAL costDAL, ICourseDAL courseDAL, IGoodsDAL goodsDAL, IUserOperationLogDAL userOperationLogDAL, IEventPublisher eventPublisher,
            IStudentCourseDAL studentCourseDAL, IStudentAccountRechargeDAL studentAccountRechargeDAL,
            IStudentAccountRechargeLogDAL studentAccountRechargeLogDAL, IParentStudentDAL parentStudentDAL,
            IStudentAccountRechargeCoreBLL studentAccountRechargeCoreBLL)
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
            this._studentCourseDAL = studentCourseDAL;
            this._studentAccountRechargeDAL = studentAccountRechargeDAL;
            this._studentAccountRechargeLogDAL = studentAccountRechargeLogDAL;
            this._parentStudentDAL = parentStudentDAL;
            this._studentAccountRechargeCoreBLL = studentAccountRechargeCoreBLL;
        }

        public void InitTenantId(int tenantId)
        {
            this._studentAccountRechargeCoreBLL.InitTenantId(tenantId);
            this.InitDataAccess(tenantId, _orderDAL, _studentDAL, _userDAL, _incomeLogDAL,
                _couponsDAL, _costDAL, _courseDAL, _goodsDAL, _userOperationLogDAL, _studentCourseDAL,
                _studentAccountRechargeDAL, _studentAccountRechargeLogDAL, _parentStudentDAL);
        }

        private async Task<OrderStudentView> OrderStudentGet(EtOrder order)
        {
            var orderStudentView = new OrderStudentView()
            {
                StudentId = order.StudentId
            };
            if (order.StudentId > 0)
            {
                var studentBucket = await _studentDAL.GetStudent(order.StudentId);
                if (studentBucket != null && studentBucket.Student != null)
                {
                    orderStudentView.StudentPhone = studentBucket.Student.Phone;
                    orderStudentView.StudentName = studentBucket.Student.Name;
                    orderStudentView.StudentCardNo = studentBucket.Student.CardNo;
                    return orderStudentView;
                }
            }
            else
            {
                if (order.StudentAccountRechargeId != null)
                {
                    var accountLog = await _studentAccountRechargeDAL.GetStudentAccountRecharge(order.StudentAccountRechargeId.Value);
                    if (accountLog != null)
                    {
                        orderStudentView.StudentPhone = accountLog.Phone;
                        return orderStudentView;
                    }
                }
            }
            return orderStudentView;
        }

        public async Task<ResponseBase> OrderGetPaging(OrderGetPagingRequest request)
        {
            var pagingData = await _orderDAL.GetOrderPaging(request);
            var orderOutput = new List<OrderGetPagingOutput>();
            var tempBoxUser = new DataTempBox<EtUser>();
            foreach (var p in pagingData.Item1)
            {
                var studentInfo = await OrderStudentGet(p);
                orderOutput.Add(new OrderGetPagingOutput()
                {
                    AptSum = p.AptSum,
                    ArrearsSum = p.ArrearsSum,
                    BuyCost = p.BuyCost,
                    BuyCourse = p.BuyCourse,
                    BuyGoods = p.BuyGoods,
                    BuyOther = p.BuyOther,
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
                    StudentName = studentInfo.StudentName,
                    StudentPhone = studentInfo.StudentPhone,
                    Sum = p.Sum,
                    TotalPoints = p.TotalPoints,
                    TotalPointsDesc = EmOrderInOutType.GetTotalPointsDesc(p.TotalPoints, p.InOutType),
                    UserId = p.UserId,
                    UserName = await ComBusiness.GetUserName(tempBoxUser, _userDAL, p.UserId),
                    CId = p.Id,
                    InOutType = p.InOutType,
                    OrderTypeDesc = EmOrderType.GetOrderTypeDesc(p.OrderType)
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

        private void ProcessOrderAccountRechargePay(List<OrderGetDetailIncomeLog> incomeLogs, EtOrder order, string userName)
        {
            if (order.PayAccountRechargeId == null || (order.PayAccountRechargeReal == 0 && order.PayAccountRechargeGive == 0))
            {
                return;
            }
            incomeLogs.Insert(0, new OrderGetDetailIncomeLog()
            {
                PayOt = order.Ot.EtmsToDateString(),
                PayType = EmPayType.PayAccountRecharge,
                PayTypeDesc = EmPayType.GetPayType(EmPayType.PayAccountRecharge),
                ProjectType = 0,
                ProjectTypeName = EmOrderType.GetOrderTypeDesc(order.OrderType),
                Sum = order.PayAccountRechargeReal + order.PayAccountRechargeGive,
                UserName = userName
            });
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
            var studentInfo = await OrderStudentGet(order);
            var commissionUsers = await ComBusiness.GetUserMultiSelectValue(tempBoxUser, _userDAL, order.CommissionUser);
            output.BascInfo = new OrderGetDetailBascInfo()
            {
                ArrearsSum = order.ArrearsSum,
                BuyCost = order.BuyCost,
                CId = order.Id,
                AptSum = order.AptSum,
                BuyCourse = order.BuyCourse,
                BuyGoods = order.BuyGoods,
                BuyOther = order.BuyOther,
                CommissionUser = order.CommissionUser,
                CommissionUserDesc = string.Join(',', commissionUsers.Select(p => p.Label)),
                No = order.No,
                OrderType = order.OrderType,
                OrderTypeDesc = EmOrderType.GetOrderTypeDesc(order.OrderType),
                OtDesc = order.Ot.EtmsToDateString(),
                PaySum = order.PaySum,
                Remark = order.Remark,
                Status = order.Status,
                StatusDesc = EmOrderStatus.GetOrderStatus(order.Status),
                StudentId = order.StudentId,
                StudentName = studentInfo.StudentName,
                StudentPhone = studentInfo.StudentPhone,
                Sum = order.Sum,
                TotalPoints = order.TotalPoints,
                UserId = order.UserId,
                UserName = await ComBusiness.GetUserName(tempBoxUser, _userDAL, order.UserId),
                CreateOt = order.CreateOt,
                CommissionUserIds = commissionUsers,
                InOutType = order.InOutType,
                TotalPointsDesc = EmOrderInOutType.GetTotalPointsDesc(order.TotalPoints, order.InOutType),
                UnionOrderId = order.UnionOrderId.ToString(),
                UnionOrderNo = order.UnionOrderNo,
                IsReturn = order.IsReturn,
                IsTransferCourse = order.IsTransferCourse,
                StudentCardNo = studentInfo.StudentCardNo,
                GiveSum = order.AptSum - order.Sum
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
            var isHasCourse = false;
            var isOnlyOneToOneCourse = true;
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
                        isHasCourse = true;
                        if (myCourse != null && myCourse.Item1 != null)
                        {
                            productName = myCourse.Item1.Name;
                            if (myCourse.Item1.Type == EmCourseType.OneToMany)
                            {
                                isOnlyOneToOneCourse = false;
                            }
                        }
                        break;
                }
                output.OrderGetDetailProducts.Add(new OrderGetDetailProductInfo()
                {
                    BugUnit = myItem.BugUnit,
                    BuyQuantity = myItem.BuyQuantity,
                    BuyQuantityDesc = ComBusiness.GetBuyQuantityDesc(myItem.BuyQuantity, myItem.BugUnit, myItem.ProductType),
                    DiscountDesc = ComBusiness.GetDiscountDesc(myItem.DiscountValue, myItem.DiscountType),
                    GiveQuantityDesc = ComBusiness.GetGiveQuantityDesc(myItem.GiveQuantity, myItem.GiveUnit),
                    ItemAptSum = Math.Abs(myItem.ItemAptSum),
                    ItemSum = Math.Abs(myItem.ItemSum),
                    PriceRule = myItem.PriceRule,
                    ProductTypeDesc = EmOrderProductType.GetOrderProductType(myItem.ProductType),
                    ProductName = productName,
                    CId = myItem.Id,
                    OutQuantity = myItem.OutQuantity,
                    OutQuantityDesc = ComBusiness.GetOutQuantityDesc(myItem.OutQuantity, myItem.BugUnit, myItem.ProductType)
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
            ProcessOrderAccountRechargePay(output.OrderGetDetailIncomeLogs, order, output.BascInfo.UserName);

            output.BascInfo.IsHasCourse = isHasCourse;
            output.BascInfo.IsOnlyOneToOneCourse = isOnlyOneToOneCourse;
            return ResponseBase.Success(output);
        }

        public async Task<ResponseBase> OrderGetDetailAccountRecharge(OrderGetDetailRequest request)
        {
            var order = await _orderDAL.GetOrder(request.CId);
            if (order == null)
            {
                return ResponseBase.CommonError("订单不存在");
            }
            var output = new OrderGetDetailAccountRechargeOutput();
            var tempBoxUser = new DataTempBox<EtUser>();
            var commissionUsers = await ComBusiness.GetUserMultiSelectValue(tempBoxUser, _userDAL, order.CommissionUser);
            output.BascInfo = new OrderGetDetailAccountRechargeOutputBasc()
            {
                ArrearsSum = order.ArrearsSum,
                BuyCost = order.BuyCost,
                CId = order.Id,
                AptSum = order.AptSum,
                BuyCourse = order.BuyCourse,
                BuyGoods = order.BuyGoods,
                BuyOther = order.BuyOther,
                CommissionUser = order.CommissionUser,
                CommissionUserDesc = string.Join(',', commissionUsers.Select(p => p.Label)),
                No = order.No,
                OrderType = order.OrderType,
                OrderTypeDesc = EmOrderType.GetOrderTypeDesc(order.OrderType),
                OtDesc = order.Ot.EtmsToDateString(),
                PaySum = order.PaySum,
                Remark = order.Remark,
                Status = order.Status,
                StatusDesc = EmOrderStatus.GetOrderStatus(order.Status),
                StudentId = order.StudentId,
                Sum = order.Sum,
                TotalPoints = order.TotalPoints,
                UserId = order.UserId,
                UserName = await ComBusiness.GetUserName(tempBoxUser, _userDAL, order.UserId),
                CreateOt = order.CreateOt,
                CommissionUserIds = commissionUsers,
                InOutType = order.InOutType,
                TotalPointsDesc = EmOrderInOutType.GetTotalPointsDesc(order.TotalPoints, order.InOutType),
                GiveSum = order.AptSum - order.Sum
            };
            var accountRechargeLog = await _studentAccountRechargeLogDAL.GetAccountRechargeLogByOrderId(order.Id);
            if (accountRechargeLog != null)
            {
                var studentAccountRechargeView = await _studentAccountRechargeCoreBLL.GetStudentAccountRechargeByPhone(accountRechargeLog.Phone);
                output.RechargeLog = new OrderGetDetailAccountRechargeOutputRecharge()
                {
                    CgBalanceGive = accountRechargeLog.CgBalanceGive,
                    CgBalanceReal = accountRechargeLog.CgBalanceReal,
                    CgNo = accountRechargeLog.CgNo,
                    CgServiceCharge = accountRechargeLog.CgServiceCharge,
                    Phone = accountRechargeLog.Phone,
                    RelatedOrderId = accountRechargeLog.RelatedOrderId,
                    RelationStudent = ComBusiness2.GetStudentsDesc2(studentAccountRechargeView.Binders),
                    StudentAccountRechargeId = accountRechargeLog.StudentAccountRechargeId,
                    Type = accountRechargeLog.Type,
                    UserId = accountRechargeLog.UserId,
                    CgBalanceRealDesc = EmStudentAccountRechargeLogType.GetValueDesc(accountRechargeLog.CgBalanceReal, accountRechargeLog.Type),
                    CgBalanceGiveDesc = EmStudentAccountRechargeLogType.GetValueDesc(accountRechargeLog.CgBalanceGive, accountRechargeLog.Type),
                    CgServiceChargeDesc = accountRechargeLog.CgServiceCharge > 0 ? $"￥{accountRechargeLog.CgServiceCharge.ToString("F2")}" : "-"
                };
            }

            var payLog = await _incomeLogDAL.GetIncomeLogByOrderId(request.CId);
            output.IncomeLogs = new List<OrderGetDetailAccountRechargeOutputIncomeLog>();
            if (payLog != null && payLog.Any())
            {
                foreach (var p in payLog)
                {
                    output.IncomeLogs.Add(new OrderGetDetailAccountRechargeOutputIncomeLog()
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

        public async Task<ResponseBase> OrderTransferCoursesGetDetail(OrderTransferCoursesGetDetailRequest request)
        {
            var order = await _orderDAL.GetOrder(request.CId);
            if (order == null)
            {
                return ResponseBase.CommonError("订单不存在");
            }
            var output = new OrderTransferCoursesGetDetailOutput()
            {
                InList = new List<OrderTransferCoursesGetDetailIn>(),
                OutList = new List<OrderTransferCoursesGetDetailOut>()
            };
            var tempBoxUser = new DataTempBox<EtUser>();
            var studentInfo = await OrderStudentGet(order);
            var commissionUsers = await ComBusiness.GetUserMultiSelectValue(tempBoxUser, _userDAL, order.CommissionUser);
            output.BascInfo = new OrderTransferCoursesGetDetailBascInfo()
            {
                ArrearsSum = order.ArrearsSum,
                BuyCost = order.BuyCost,
                CId = order.Id,
                AptSum = order.AptSum,
                BuyCourse = order.BuyCourse,
                BuyGoods = order.BuyGoods,
                BuyOther = order.BuyOther,
                CommissionUser = order.CommissionUser,
                CommissionUserDesc = string.Join(',', commissionUsers.Select(p => p.Label)),
                No = order.No,
                OrderType = order.OrderType,
                OrderTypeDesc = EmOrderType.GetOrderTypeDesc(order.OrderType),
                OtDesc = order.Ot.EtmsToDateString(),
                PaySum = order.PaySum,
                Remark = order.Remark,
                Status = order.Status,
                StatusDesc = EmOrderStatus.GetOrderStatus(order.Status),
                StudentId = order.StudentId,
                StudentName = studentInfo.StudentName,
                StudentPhone = studentInfo.StudentPhone,
                Sum = order.Sum,
                TotalPoints = order.TotalPoints,
                UserId = order.UserId,
                UserName = await ComBusiness.GetUserName(tempBoxUser, _userDAL, order.UserId),
                CreateOt = order.CreateOt,
                CommissionUserIds = commissionUsers,
                InOutType = order.InOutType,
                TotalPointsDesc = EmOrderInOutType.GetTotalPointsDesc(order.TotalPoints, order.InOutType),
                UnionOrderId = order.UnionOrderId.ToString(),
                UnionOrderNo = order.UnionOrderNo,
                IsReturn = order.IsReturn,
                IsTransferCourse = order.IsTransferCourse
            };
            var orderDetail = await _orderDAL.GetOrderDetail(request.CId);
            var intDetail = orderDetail.Where(p => p.InOutType == EmOrderInOutType.In);
            var tempBoxCourse = new DataTempBox<EtCourse>();
            foreach (var myItem in intDetail)
            {
                output.InList.Add(new OrderTransferCoursesGetDetailIn()
                {
                    BugUnit = myItem.BugUnit,
                    BuyQuantity = myItem.BuyQuantity,
                    BuyQuantityDesc = ComBusiness.GetBuyQuantityDesc(myItem.BuyQuantity, myItem.BugUnit, myItem.ProductType),
                    DiscountDesc = ComBusiness.GetDiscountDesc(myItem.DiscountValue, myItem.DiscountType),
                    GiveQuantityDesc = ComBusiness.GetGiveQuantityDesc(myItem.GiveQuantity, myItem.GiveUnit),
                    ItemAptSum = Math.Abs(myItem.ItemAptSum),
                    ItemSum = Math.Abs(myItem.ItemSum),
                    PriceRule = myItem.PriceRule,
                    ProductTypeDesc = EmOrderProductType.GetOrderProductType(myItem.ProductType),
                    ProductName = await ComBusiness.GetCourseName(tempBoxCourse, _courseDAL, myItem.ProductId),
                    CId = myItem.Id,
                    OutQuantity = myItem.OutQuantity,
                    OutQuantityDesc = ComBusiness.GetOutQuantityDesc(myItem.OutQuantity, myItem.BugUnit, myItem.ProductType)
                });
            }
            var outDetail = orderDetail.Where(p => p.InOutType == EmOrderInOutType.Out);
            foreach (var myItem in outDetail)
            {
                output.OutList.Add(new OrderTransferCoursesGetDetailOut()
                {
                    CId = myItem.Id,
                    UnionOrderId = myItem.OutOrderId.Value,
                    UnionOrderNo = myItem.OutOrderNo,
                    ItemAptSum = Math.Abs(myItem.ItemAptSum),
                    OutQuantity = myItem.OutQuantity.EtmsToString(),
                    OutQuantityDesc = ComBusiness.GetOutQuantityDesc(myItem.OutQuantity, myItem.BugUnit, myItem.ProductType),
                    ProductName = await ComBusiness.GetCourseName(tempBoxCourse, _courseDAL, myItem.ProductId)
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
            ProcessOrderAccountRechargePay(output.OrderGetDetailIncomeLogs, order, output.BascInfo.UserName);

            output.InSum = output.InList.Sum(j => j.ItemAptSum);
            output.OutSum = output.OutList.Sum(j => j.ItemAptSum);
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
            var studentInfo = await OrderStudentGet(order);
            output.BascInfo = new OrderGetDetailBascInfo()
            {
                ArrearsSum = order.ArrearsSum,
                BuyCost = order.BuyCost,
                CId = order.Id,
                AptSum = order.AptSum,
                BuyCourse = order.BuyCourse,
                BuyGoods = order.BuyGoods,
                BuyOther = order.BuyOther,
                CommissionUser = order.CommissionUser,
                CommissionUserDesc = await ComBusiness.GetUserNames(tempBoxUser, _userDAL, order.CommissionUser),
                No = order.No,
                OrderType = order.OrderType,
                OrderTypeDesc = EmOrderType.GetOrderTypeDesc(order.OrderType),
                OtDesc = order.Ot.EtmsToDateString(),
                PaySum = order.PaySum,
                Remark = order.Remark,
                Status = order.Status,
                StatusDesc = EmOrderStatus.GetOrderStatus(order.Status),
                StudentId = order.StudentId,
                StudentName = studentInfo.StudentName,
                StudentPhone = studentInfo.StudentPhone,
                Sum = order.Sum,
                TotalPoints = order.TotalPoints,
                UserId = order.UserId,
                UserName = await ComBusiness.GetUserName(tempBoxUser, _userDAL, order.UserId),
                CreateOt = order.CreateOt,
                InOutType = order.InOutType,
                TotalPointsDesc = EmOrderInOutType.GetTotalPointsDesc(order.TotalPoints, order.InOutType),
                UnionOrderId = order.UnionOrderId.ToString(),
                UnionOrderNo = order.UnionOrderNo,
                IsReturn = order.IsReturn,
                IsTransferCourse = order.IsTransferCourse,
                StudentCardNo = studentInfo.StudentCardNo,
                GiveSum = order.AptSum - order.Sum
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
            ProcessOrderAccountRechargePay(output.OrderGetDetailIncomeLogs, order, output.BascInfo.UserName);

            return ResponseBase.Success(output);
        }

        public async Task<ResponseBase> OrderGetBascDetail(OrderGetBascDetailRequest request)
        {
            var order = await _orderDAL.GetOrder(request.CId);
            if (order == null)
            {
                return ResponseBase.CommonError("订单不存在");
            }
            var tempBoxUser = new DataTempBox<EtUser>();
            var studentInfo = await OrderStudentGet(order);
            var output = new OrderGetDetailBascInfo()
            {
                ArrearsSum = order.ArrearsSum,
                BuyCost = order.BuyCost,
                CId = order.Id,
                AptSum = order.AptSum,
                BuyCourse = order.BuyCourse,
                BuyGoods = order.BuyGoods,
                BuyOther = order.BuyOther,
                CommissionUser = order.CommissionUser,
                CommissionUserDesc = await ComBusiness.GetUserNames(tempBoxUser, _userDAL, order.CommissionUser),
                No = order.No,
                OrderType = order.OrderType,
                OrderTypeDesc = EmOrderType.GetOrderTypeDesc(order.OrderType),
                OtDesc = order.Ot.EtmsToDateString(),
                PaySum = order.PaySum,
                Remark = order.Remark,
                Status = order.Status,
                StatusDesc = EmOrderStatus.GetOrderStatus(order.Status),
                StudentId = order.StudentId,
                StudentName = studentInfo.StudentName,
                StudentPhone = studentInfo.StudentPhone,
                Sum = order.Sum,
                TotalPoints = order.TotalPoints,
                UserId = order.UserId,
                UserName = await ComBusiness.GetUserName(tempBoxUser, _userDAL, order.UserId),
                CreateOt = order.CreateOt,
                InOutType = order.InOutType,
                TotalPointsDesc = EmOrderInOutType.GetTotalPointsDesc(order.TotalPoints, order.InOutType),
                UnionOrderId = order.UnionOrderId.ToString(),
                UnionOrderNo = order.UnionOrderNo,
                IsReturn = order.IsReturn,
                IsTransferCourse = order.IsTransferCourse,
                StudentCardNo = studentInfo.StudentCardNo,
                GiveSum = order.AptSum - order.Sum
            };
            return ResponseBase.Success(output);
        }

        public async Task<ResponseBase> OrderGetProductInfo(OrderGetProductInfoRequest request)
        {
            var order = await _orderDAL.GetOrder(request.CId);
            if (order == null)
            {
                return ResponseBase.CommonError("订单不存在");
            }
            var orderDetail = await _orderDAL.GetOrderDetail(request.CId);
            var studentCourseDetail = await _studentCourseDAL.GetStudentCourseDetailByOrderId(request.CId);
            var output = new OrderGetProductInfoOutput()
            {
                OrderCourses = new List<OrderGetProductInfoCourseItem>(),
                OrderGoods = new List<OrderGetProductInfoGoodsItem>(),
                OrderCosts = new List<OrderGetProductInfoCostItem>()
            };
            foreach (var p in orderDetail)
            {
                switch (p.ProductType)
                {
                    case EmOrderProductType.Course:
                        var tempCourse = await _courseDAL.GetCourse(p.ProductId);
                        var tempCourseNmae = tempCourse.Item1.Name;
                        var myStudentCourseDetail = studentCourseDetail.FirstOrDefault(j => j.CourseId == p.ProductId);
                        if (myStudentCourseDetail == null)
                        {
                            Log.Error($"[OrderGetProductInfo]获取订单课程详情失败,orderId:{request.CId}", this.GetType());
                            continue;
                        }
                        var courseCanReturnInfo = ComBusiness2.GetCourseCanReturnInfo(p, myStudentCourseDetail);
                        output.OrderCourses.Add(new OrderGetProductInfoCourseItem()
                        {
                            OrderDetailId = p.Id,
                            PriceRule = p.PriceRule,
                            ProductName = tempCourseNmae,
                            DiscountDesc = ComBusiness.GetDiscountDesc(p.DiscountValue, p.DiscountType),
                            BugUnit = p.BugUnit,
                            BuyQuantity = p.BuyQuantity,
                            BuyQuantityDesc = ComBusiness.GetBuyQuantityDesc(p.BuyQuantity, p.BugUnit, p.ProductType),
                            GiveQuantityDesc = ComBusiness.GetGiveQuantityDesc(p.GiveQuantity, p.GiveUnit),
                            GiveQuantity = p.GiveQuantity,
                            ItemAptSum = p.ItemAptSum,
                            Price = courseCanReturnInfo.Price,
                            PriceDesc = p.PriceRule,
                            SurplusQuantity = courseCanReturnInfo.SurplusQuantity.EtmsToString(),
                            SurplusQuantityDesc = courseCanReturnInfo.SurplusQuantityDesc,
                            Status = courseCanReturnInfo.IsHas ? OrderGetProductInfoItemsStatus.Normal : OrderGetProductInfoItemsStatus.Disable,
                            ItemSum = p.ItemSum,
                            ProductTypeDesc = EmOrderProductType.GetOrderProductType(p.ProductType),
                            ProductId = p.ProductId,
                            BuyValidSmallQuantity = courseCanReturnInfo.BuyValidSmallQuantity
                        });
                        break;
                    case EmOrderProductType.Goods:
                        var tempGoods = await _goodsDAL.GetGoods(p.ProductId);
                        var tempGoodsName = tempGoods?.Name;
                        var tempGoodsTotalQuantity = p.BuyQuantity + p.GiveQuantity;
                        var tempGoodsSurplusQuantity = (int)(tempGoodsTotalQuantity - p.OutQuantity);
                        var tempGoodsPrice = Math.Round(p.ItemAptSum / p.BuyQuantity, 2);
                        output.OrderGoods.Add(new OrderGetProductInfoGoodsItem()
                        {
                            BuyQuantity = p.BuyQuantity,
                            BuyQuantityDesc = ComBusiness.GetBuyQuantityDesc(p.BuyQuantity, p.BugUnit, p.ProductType),
                            DiscountDesc = ComBusiness.GetDiscountDesc(p.DiscountValue, p.DiscountType),
                            OrderDetailId = p.Id,
                            Price = tempGoodsPrice,
                            PriceDesc = ComBusiness.GetProductPriceDesc(tempGoodsPrice, p.ProductType),
                            ProductName = tempGoodsName,
                            SurplusQuantity = tempGoodsSurplusQuantity,
                            SurplusQuantityDesc = ComBusiness.GetProductSurplusQuantityDesc(tempGoodsSurplusQuantity, p.BugUnit, p.ProductType),
                            PriceRule = p.PriceRule,
                            Status = tempGoodsSurplusQuantity > 0 ? OrderGetProductInfoItemsStatus.Normal : OrderGetProductInfoItemsStatus.Disable,
                            ItemSum = p.ItemSum,
                            ItemAptSum = p.ItemAptSum,
                            ProductTypeDesc = EmOrderProductType.GetOrderProductType(p.ProductType),
                            ProductId = p.ProductId,
                            BuyValidSmallQuantity = tempGoodsTotalQuantity
                        });
                        break;
                    case EmOrderProductType.Cost:
                        var tempCost = await _costDAL.GetCost(p.ProductId);
                        var tempCostName = tempCost?.Name;
                        var tempCostTotalQuantity = p.BuyQuantity + p.GiveQuantity;
                        var tempCostSurplusQuantity = (int)(tempCostTotalQuantity - p.OutQuantity);
                        var tempCostPrice = Math.Round(p.ItemAptSum / p.BuyQuantity, 2);
                        output.OrderCosts.Add(new OrderGetProductInfoCostItem()
                        {
                            BuyQuantity = p.BuyQuantity,
                            BuyQuantityDesc = ComBusiness.GetBuyQuantityDesc(p.BuyQuantity, p.BugUnit, p.ProductType),
                            DiscountDesc = ComBusiness.GetDiscountDesc(p.DiscountValue, p.DiscountType),
                            OrderDetailId = p.Id,
                            PriceRule = p.PriceRule,
                            ProductName = tempCostName,
                            SurplusQuantity = tempCostSurplusQuantity,
                            SurplusQuantityDesc = ComBusiness.GetProductSurplusQuantityDesc(tempCostSurplusQuantity, p.BugUnit, p.ProductType),
                            Price = tempCostPrice,
                            PriceDesc = ComBusiness.GetProductPriceDesc(tempCostPrice, p.ProductType),
                            Status = tempCostSurplusQuantity > 0 ? OrderGetProductInfoItemsStatus.Normal : OrderGetProductInfoItemsStatus.Disable,
                            ItemSum = p.ItemSum,
                            ItemAptSum = p.ItemAptSum,
                            ProductTypeDesc = EmOrderProductType.GetOrderProductType(p.ProductType),
                            ProductId = p.ProductId,
                            BuyValidSmallQuantity = tempCostTotalQuantity
                        });
                        break;
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

            _eventPublisher.Publish(new ResetTenantToDoThingEvent(request.LoginTenantId));
            await _userOperationLogDAL.AddUserLog(request, $"补交报名费用-订单号:{order.No},补交金额:{payTotal}", EmUserOperationType.StudentEnrolmentAddPay, now);
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

        public async Task<ResponseBase> OrderReturnLogGet(OrderReturnLogGetRequest request)
        {
            var returnOrder = await _orderDAL.GetUnionOrderSource(request.CId);
            var output = new List<OrderReturnLogGetOutput>();
            if (returnOrder.Count > 0)
            {
                var tempBoxUser = new DataTempBox<EtUser>();
                var returnOrderDetail = await _orderDAL.GetOrderDetail(returnOrder.Select(p => p.Id).ToList());
                foreach (var order in returnOrder)
                {
                    var myOutputItem = new OrderReturnLogGetOutput()
                    {
                        AptSum = order.AptSum,
                        CId = order.Id,
                        CreateOt = order.CreateOt,
                        InOutType = order.InOutType,
                        No = order.No,
                        OrderType = order.OrderType,
                        OtDesc = order.Ot.EtmsToDateString(),
                        Status = order.Status,
                        StatusDesc = EmOrderStatus.GetOrderStatus(order.Status),
                        Remark = order.Remark,
                        StudentId = order.StudentId,
                        Sum = order.Sum,
                        TotalPoints = order.TotalPoints,
                        TotalPointsDesc = EmOrderInOutType.GetTotalPointsDesc(order.TotalPoints, order.InOutType),
                        UserId = order.UserId,
                        UserName = await ComBusiness.GetUserName(tempBoxUser, _userDAL, order.UserId),
                        LogDetails = new List<OrderReturnLogDetail>()
                    };
                    var myOrderDetail = returnOrderDetail.Where(p => p.OrderId == order.Id);
                    foreach (var orderDetail in myOrderDetail)
                    {
                        var productName = string.Empty;
                        switch (orderDetail.ProductType)
                        {
                            case EmOrderProductType.Cost:
                                var myCost = await _costDAL.GetCost(orderDetail.ProductId);
                                productName = myCost?.Name;
                                break;
                            case EmOrderProductType.Goods:
                                var myGoods = await _goodsDAL.GetGoods(orderDetail.ProductId);
                                productName = myGoods?.Name;
                                break;
                            case EmOrderProductType.Course:
                                var myCourse = await _courseDAL.GetCourse(orderDetail.ProductId);
                                productName = myCourse?.Item1.Name;
                                break;
                        }
                        myOutputItem.LogDetails.Add(new OrderReturnLogDetail()
                        {
                            CId = orderDetail.Id,
                            ItemAptSum = Math.Abs(orderDetail.ItemAptSum),
                            ItemSum = Math.Abs(orderDetail.ItemSum),
                            OutQuantity = orderDetail.OutQuantity.EtmsToString(),
                            ProductTypeDesc = EmOrderProductType.GetOrderProductType(orderDetail.ProductType),
                            ProductName = productName,
                            OutQuantityDesc = ComBusiness.GetOutQuantityDesc(orderDetail.OutQuantity, orderDetail.BugUnit, orderDetail.ProductType)
                        });
                    }
                    output.Add(myOutputItem);
                }
            }
            return ResponseBase.Success(output);
        }

        public async Task<ResponseBase> OrderTransferCoursesLogGet(OrderTransferCoursesLogGetRequest request)
        {
            var unionTransferOrder = await _orderDAL.GetUnionTransferOrder(request.CId);
            var output = new List<OrderTransferCoursesLogGetOutput>();
            if (unionTransferOrder.Any())
            {
                var tempBoxUser = new DataTempBox<EtUser>();
                var returnOrderDetail = await _orderDAL.GetOrderDetail(unionTransferOrder.Select(p => p.Id).ToList());
                var myTransferOrderDetail = returnOrderDetail.Where(p => p.OutOrderId == request.CId);
                foreach (var p in myTransferOrderDetail)
                {
                    var myCourse = await _courseDAL.GetCourse(p.ProductId);
                    if (myCourse == null || myCourse.Item1 == null)
                    {
                        LOG.Log.Error("[OrderTransferCoursesLogGet]课程不存在", request, this.GetType());
                        continue;
                    }
                    output.Add(new OrderTransferCoursesLogGetOutput
                    {
                        ItemAptSum = Math.Abs(p.ItemAptSum),
                        UnionOrderId = p.OrderId,
                        UnionOrderNo = p.OrderNo,
                        OutQuantity = p.OutQuantity.EtmsToString(),
                        OutQuantityDesc = ComBusiness.GetOutQuantityDesc(p.OutQuantity, p.BugUnit, p.ProductType),
                        ProductName = myCourse.Item1.Name,
                        OtDesc = p.Ot.EtmsToDateString()
                    });
                }
            }
            return ResponseBase.Success(output);
        }

        public async Task<ResponseBase> OrderEditRemark(OrderEditRemarkRequest request)
        {
            var order = await _orderDAL.GetOrder(request.CId);
            if (order == null)
            {
                return ResponseBase.CommonError("订单不存在");
            }
            //if (order.Status == EmOrderStatus.Repeal)
            //{
            //    return ResponseBase.CommonError("此订单已作废，无法编辑");
            //}
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
            await _userOperationLogDAL.AddUserLog(request, $"编辑订单备注-订单号:{order.No},新备注:{request.NewRemark}", EmUserOperationType.OrderMgr, now);
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
            var oldCommissionUser = order.CommissionUser;
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

            if (EmOrderType.IsBuyOrder(order.OrderType))
            {
                _eventPublisher.Publish(new StatisticsSalesOrderEvent(request.LoginTenantId)
                {
                    Order1 = order,
                    OpType = StatisticsSalesOrderOpType.ChangeCommissionUser,
                    OldCommissionUser = oldCommissionUser
                });
            }
            await _userOperationLogDAL.AddUserLog(request, $"修改业绩归属人-订单号:{order.No},新业绩归属人:{newNames}", EmUserOperationType.OrderMgr, now);
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
                    //case EmOrderType.ReturnOrder:
                    //    return await ReturnOrderRepeal(request, order);
                    //case EmOrderType.TransferCourse:
                    //    return await TransferCourseRepeal(request, order);
            }
            return ResponseBase.CommonError("此类型订单无法作废");
        }

        private async Task<ResponseBase> OrderStudentEnrolmentRepeal(OrderRepealRequest request)
        {
            //var unionOrderSource = await _orderDAL.GetUnionOrderSource(request.OrderId);
            //if (unionOrderSource != null && unionOrderSource.Count > 0)
            //{
            //    return ResponseBase.CommonError("此订单有退单操作，无法作废");
            //}
            if (await _orderDAL.ExistOutOrder(request.OrderId))
            {
                return ResponseBase.CommonError("此订单已有转课/退单操作，无法作废");
            }
            var studentCourseDetail = await _studentCourseDAL.GetStudentCourseDetailByOrderId(request.OrderId);
            if (studentCourseDetail.Count > 0)
            {
                //判断购买的课时是否已使用，扣减
                var clasTimesHasUseLog = studentCourseDetail.FirstOrDefault(p => p.DeType == EmDeClassTimesType.ClassTimes && p.UseQuantity > 0);
                if (clasTimesHasUseLog != null)
                {
                    return ResponseBase.CommonError("此订单已记上课，无法作废");
                }
            }

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

        private async Task<ResponseBase> ReturnOrderRepeal(OrderRepealRequest request, EtOrder order)
        { return ResponseBase.Success(); }

        private async Task<ResponseBase> TransferCourseRepeal(OrderRepealRequest request, EtOrder order)
        { return ResponseBase.Success(); }

        private EtOrderDetail GetReturnOrderDetail(EtOrderDetail sourceOrderDetail, OrderReturnProductItem productItem,
            string newNo, DateTime now, long sourceOrderId, string sourceOrderNo)
        {
            var buyUnit = sourceOrderDetail.BugUnit;
            if (buyUnit == EmCourseUnit.Month)
            {
                buyUnit = EmCourseUnit.Day;
            }
            var buyQuantity = Convert.ToInt32(productItem.ReturnCount);
            if (sourceOrderDetail.ProductType == EmOrderProductType.Course && sourceOrderDetail.BugUnit != EmCourseUnit.ClassTimes)
            {
                buyQuantity = buyQuantity / 30;
            }
            var price = Math.Round(productItem.ReturnSum / productItem.ReturnCount, 2);
            return new EtOrderDetail()
            {
                BugUnit = buyUnit,
                BuyQuantity = -buyQuantity,
                OutQuantity = productItem.ReturnCount,
                DiscountType = EmOrderDiscountType.Nothing,
                DiscountValue = 0,
                GiveQuantity = 0,
                GiveUnit = sourceOrderDetail.GiveUnit,
                InOutType = EmOrderInOutType.Out,
                IsDeleted = EmIsDeleted.Normal,
                ItemAptSum = -productItem.ReturnSum,
                ItemSum = -productItem.ReturnSum,
                OrderId = 0,
                OrderNo = newNo,
                Ot = now,
                Price = -price,
                PriceRule = string.Empty,
                ProductId = sourceOrderDetail.ProductId,
                ProductType = sourceOrderDetail.ProductType,
                Remark = string.Empty,
                Status = EmOrderStatus.Normal,
                TenantId = sourceOrderDetail.TenantId,
                UserId = sourceOrderDetail.UserId,
                OutOrderId = sourceOrderId,
                OutOrderNo = sourceOrderNo
            };
        }

        public async Task<ResponseBase> OrderReturnProduct(OrderReturnProductRequest request)
        {
            var sourceOrder = await _orderDAL.GetOrder(request.ReturnOrderId);
            if (sourceOrder == null)
            {
                return ResponseBase.CommonError("原订单不存在");
            }
            if (sourceOrder.Status == EmOrderStatus.Repeal)
            {
                return ResponseBase.CommonError("原订单已作废，无法退单");
            }
            if (sourceOrder.OrderType != EmOrderType.StudentEnrolment)
            {
                return ResponseBase.CommonError("此类型订单无法退单");
            }
            if (sourceOrder.Status == EmOrderStatus.Unpaid || sourceOrder.Status == EmOrderStatus.MakeUpMoney)
            {
                return ResponseBase.CommonError("原订单未支付完成，无法退单");
            }
            if (request.OrderReturnOrderInfo.Ot.Date < sourceOrder.Ot.Date)
            {
                return ResponseBase.CommonError("退单经办日期不能小于原订单经办日期");
            }
            var isReturnAccountRecharge = false;
            EtStudentAccountRecharge accountLog = null;
            if (request.OrderReturnOrderInfo.PaySum > 0)
            {
                if (request.OrderReturnOrderInfo.PayType == EmPayType.PayAccountRecharge &&
                    request.OrderReturnOrderInfo.PayStudentAccountRechargeId != null)
                {
                    accountLog = await _studentAccountRechargeDAL.GetStudentAccountRecharge(request.OrderReturnOrderInfo.PayStudentAccountRechargeId.Value);
                    if (accountLog == null)
                    {
                        return ResponseBase.CommonError("充值账户不存在");
                    }
                    isReturnAccountRecharge = true;
                }
            }

            var sourceOrderDetail = await _orderDAL.GetOrderDetail(request.ReturnOrderId);
            var sourceStudentCourseDetail = await _studentCourseDAL.GetStudentCourseDetailByOrderId(request.ReturnOrderId);
            var sourceOrderDetailUpdateEntitys = new List<EtOrderDetail>();
            var sourceStudentCourseDetailUpdateEntitys = new List<EtStudentCourseDetail>();
            var newOrderDetailList = new List<EtOrderDetail>();
            var newOrderNo = OrderNumberLib.GetReturnOrderNumber();
            var now = DateTime.Now;
            var monthToDay = SystemConfig.ComConfig.MonthToDay;
            StringBuilder buyCourse = new StringBuilder(), buyGoods = new StringBuilder(), buyCost = new StringBuilder();
            foreach (var changeOrderDetail in request.OrderReturnProductItems)
            {
                //处理原订单和学员剩余课程,创建订单详情
                var mySourceOrderDetail = sourceOrderDetail.FirstOrDefault(p => p.Id == changeOrderDetail.OrderDetailId && p.ProductId == changeOrderDetail.ProductId);
                if (mySourceOrderDetail == null)
                {
                    return ResponseBase.CommonError("请求数据错误，请重新再试");
                }
                newOrderDetailList.Add(GetReturnOrderDetail(mySourceOrderDetail, changeOrderDetail, newOrderNo, now, sourceOrder.Id, sourceOrder.No));
                switch (mySourceOrderDetail.ProductType)
                {
                    case EmOrderProductType.Course:
                        var mySourceStudentCourseDetail = sourceStudentCourseDetail.FirstOrDefault(p => p.CourseId == changeOrderDetail.ProductId);
                        if (mySourceStudentCourseDetail == null)
                        {
                            LOG.Log.Warn("[OrderReturnProduct]课程数据错误", request, this.GetType());
                            return ResponseBase.CommonError("请求数据错误，请重新再试");
                        }
                        var desc = ComBusiness2.GetReturnCourseDesc(changeOrderDetail.ProductName, mySourceStudentCourseDetail.UseUnit, changeOrderDetail.ReturnCount);
                        buyCourse.Append($"{desc}；");
                        if (changeOrderDetail.IsAllReturn)
                        {
                            mySourceStudentCourseDetail.UseQuantity += changeOrderDetail.ReturnCount;
                            mySourceStudentCourseDetail.SurplusQuantity = 0;
                            mySourceStudentCourseDetail.SurplusSmallQuantity = 0;
                            mySourceStudentCourseDetail.Status = EmStudentCourseStatus.EndOfClass;
                            mySourceStudentCourseDetail.EndCourseRemark = "退单结课";
                            mySourceStudentCourseDetail.EndCourseTime = now;
                            mySourceStudentCourseDetail.EndCourseUser = request.LoginUserId;
                        }
                        else
                        {
                            if (mySourceStudentCourseDetail.DeType == EmDeClassTimesType.ClassTimes)
                            {
                                if (mySourceStudentCourseDetail.SurplusQuantity < changeOrderDetail.ReturnCount)
                                {
                                    return ResponseBase.CommonError($"[{changeOrderDetail.ProductName}]剩余课时不足");
                                }
                                mySourceStudentCourseDetail.UseQuantity += changeOrderDetail.ReturnCount;
                                mySourceStudentCourseDetail.SurplusQuantity -= changeOrderDetail.ReturnCount;
                            }
                            else
                            {
                                //按天
                                var deDay = (int)changeOrderDetail.ReturnCount;
                                if (mySourceStudentCourseDetail.StartTime != null && mySourceStudentCourseDetail.EndTime != null)
                                {
                                    mySourceStudentCourseDetail.EndTime = mySourceStudentCourseDetail.EndTime.Value.AddDays(-deDay);
                                    DateTime firstDate;
                                    if (mySourceStudentCourseDetail.StartTime.Value <= now.Date)
                                    {
                                        firstDate = now.Date;
                                    }
                                    else
                                    {
                                        firstDate = mySourceStudentCourseDetail.StartTime.Value;
                                    }

                                    var dffTime = EtmsHelper.GetDffTime(firstDate, mySourceStudentCourseDetail.EndTime.Value);
                                    mySourceStudentCourseDetail.SurplusQuantity = dffTime.Item1;
                                    mySourceStudentCourseDetail.SurplusSmallQuantity = dffTime.Item2;
                                    mySourceStudentCourseDetail.UseQuantity += deDay;
                                }
                                else
                                {
                                    var tatalDay = mySourceStudentCourseDetail.SurplusQuantity * monthToDay + mySourceStudentCourseDetail.SurplusSmallQuantity; //剩余总天数
                                    tatalDay = tatalDay - deDay;
                                    if (tatalDay < 0)
                                    {
                                        mySourceStudentCourseDetail.UseQuantity += changeOrderDetail.ReturnCount;
                                        mySourceStudentCourseDetail.SurplusQuantity = 0;
                                        mySourceStudentCourseDetail.SurplusSmallQuantity = 0;
                                        mySourceStudentCourseDetail.Status = EmStudentCourseStatus.EndOfClass;
                                        mySourceStudentCourseDetail.EndCourseRemark = "退单结课";
                                        mySourceStudentCourseDetail.EndCourseTime = now;
                                        mySourceStudentCourseDetail.EndCourseUser = request.LoginUserId;
                                    }
                                    else
                                    {
                                        var month = tatalDay / monthToDay;
                                        var day = tatalDay % monthToDay;
                                        mySourceStudentCourseDetail.UseQuantity += changeOrderDetail.ReturnCount;
                                        mySourceStudentCourseDetail.SurplusQuantity = month;
                                        mySourceStudentCourseDetail.SurplusSmallQuantity = day;
                                    }
                                }
                            }
                        }
                        sourceStudentCourseDetailUpdateEntitys.Add(mySourceStudentCourseDetail);

                        mySourceOrderDetail.OutQuantity += (int)changeOrderDetail.ReturnCount;
                        sourceOrderDetailUpdateEntitys.Add(mySourceOrderDetail);
                        break;
                    case EmOrderProductType.Goods:
                        if (changeOrderDetail.ReturnCount > (mySourceOrderDetail.BuyQuantity + mySourceOrderDetail.GiveQuantity - mySourceOrderDetail.OutQuantity))
                        {
                            return ResponseBase.CommonError($"[{changeOrderDetail.ProductName}]剩余数量不足");
                        }
                        buyGoods.Append($"{changeOrderDetail.ProductName}({changeOrderDetail.ReturnCount.EtmsToString()}件)；");
                        mySourceOrderDetail.OutQuantity += (int)changeOrderDetail.ReturnCount;
                        sourceOrderDetailUpdateEntitys.Add(mySourceOrderDetail);
                        break;
                    case EmOrderProductType.Cost:
                        if (changeOrderDetail.ReturnCount > (mySourceOrderDetail.BuyQuantity + mySourceOrderDetail.GiveQuantity - mySourceOrderDetail.OutQuantity))
                        {
                            return ResponseBase.CommonError($"[{changeOrderDetail.ProductName}]剩余数量不足");
                        }
                        buyGoods.Append($"{changeOrderDetail.ProductName}({changeOrderDetail.ReturnCount.EtmsToString()}笔)；");
                        mySourceOrderDetail.OutQuantity += (int)changeOrderDetail.ReturnCount;
                        sourceOrderDetailUpdateEntitys.Add(mySourceOrderDetail);
                        break;
                }
            }
            if (sourceOrderDetailUpdateEntitys.Count > 0)
            {
                await _orderDAL.EditOrderDetail(sourceOrderDetailUpdateEntitys);
            }
            if (sourceStudentCourseDetailUpdateEntitys.Count > 0)
            {
                await _studentCourseDAL.UpdateStudentCourseDetail(sourceStudentCourseDetailUpdateEntitys);
            }
            _eventPublisher.Publish(new StudentCourseAnalyzeEvent(sourceOrder.TenantId)
            {
                StudentId = sourceOrder.StudentId
            });

            var returnOrder = new EtOrder()
            {
                StudentId = sourceOrder.StudentId,
                OrderType = EmOrderType.ReturnOrder,
                AptSum = request.OrderReturnOrderInfo.PaySum,
                PaySum = request.OrderReturnOrderInfo.PaySum,
                InOutType = EmOrderInOutType.Out,
                TenantId = sourceOrder.TenantId,
                ArrearsSum = 0,
                BuyCourse = EtmsHelper.DescPrefix(buyCourse.ToString().TrimEnd('；'), "退课程"),
                BuyGoods = EtmsHelper.DescPrefix(buyGoods.ToString().TrimEnd('；'), "退物品"),
                BuyCost = EtmsHelper.DescPrefix(buyCost.ToString().TrimEnd('；'), "退费用"),
                CommissionUser = sourceOrder.CommissionUser, //退单的业绩 算在原始订单业绩归属人身上
                CouponsIds = string.Empty,
                CouponsStudentGetIds = string.Empty,
                CreateOt = now,
                IsDeleted = EmIsDeleted.Normal,
                No = newOrderNo,
                Ot = request.OrderReturnOrderInfo.Ot.Date,
                Remark = request.OrderReturnOrderInfo.Remark,
                Status = EmOrderStatus.Normal,
                Sum = request.OrderReturnOrderInfo.PaySum,
                TotalPoints = request.OrderReturnOrderInfo.DePoint,
                UnionOrderId = sourceOrder.Id,
                UnionOrderNo = sourceOrder.No,
                UserId = request.LoginUserId
            };
            if (isReturnAccountRecharge)
            {
                returnOrder.PayAccountRechargeReal = request.OrderReturnOrderInfo.PaySum;
                returnOrder.PayAccountRechargeGive = 0;
                returnOrder.PayAccountRechargeId = accountLog.Id;
            }
            await _orderDAL.AddOrder(returnOrder, newOrderDetailList);

            if (request.OrderReturnOrderInfo.PaySum > 0)
            {
                if (isReturnAccountRecharge)
                {
                    //退充值账户
                    await _studentAccountRechargeCoreBLL.StudentAccountRechargeChange(new StudentAccountRechargeChangeEvent(request.LoginTenantId)
                    {
                        AddBalanceReal = request.OrderReturnOrderInfo.PaySum,
                        AddBalanceGive = 0,
                        AddRechargeSum = 0,
                        AddRechargeGiveSum = 0,
                        StudentAccountRechargeId = accountLog.Id,
                        TryCount = 0
                    });
                    await _studentAccountRechargeLogDAL.AddStudentAccountRechargeLog(new EtStudentAccountRechargeLog()
                    {
                        StudentAccountRechargeId = accountLog.Id,
                        CgBalanceGive = 0,
                        CgBalanceReal = request.OrderReturnOrderInfo.PaySum,
                        CgNo = newOrderNo,
                        CgServiceCharge = 0,
                        CommissionUser = string.Empty,
                        IsDeleted = EmIsDeleted.Normal,
                        Ot = now,
                        Phone = accountLog.Phone,
                        RelatedOrderId = returnOrder.Id,
                        Remark = "销售退单",
                        Status = EmStudentAccountRechargeLogStatus.Normal,
                        TenantId = returnOrder.TenantId,
                        Type = EmStudentAccountRechargeLogType.OrderReturn,
                        UserId = request.LoginUserId
                    });
                }
                else
                {
                    await _incomeLogDAL.AddIncomeLog(new EtIncomeLog()
                    {
                        AccountNo = string.Empty,
                        CreateOt = now,
                        IsDeleted = EmIsDeleted.Normal,
                        No = returnOrder.No,
                        OrderId = returnOrder.Id,
                        Ot = returnOrder.Ot,
                        PayType = request.OrderReturnOrderInfo.PayType,
                        ProjectType = EmIncomeLogProjectType.RetuenOrder,
                        Remark = returnOrder.Remark,
                        RepealOt = null,
                        RepealUserId = null,
                        Status = EmIncomeLogStatus.Normal,
                        Sum = request.OrderReturnOrderInfo.PaySum,
                        TenantId = returnOrder.TenantId,
                        UserId = returnOrder.UserId,
                        Type = EmIncomeLogType.AccountOut
                    });
                }
            }

            _eventPublisher.Publish(new OrderReturnProductEvent(sourceOrder.TenantId)
            {
                NewOrder = returnOrder,
                NewOrderDetails = newOrderDetailList,
                returnRequest = request,
                SourceOrder = sourceOrder,
                UserId = request.LoginUserId
            });
            _eventPublisher.Publish(new StatisticsSalesOrderEvent(request.LoginTenantId)
            {
                Order1 = returnOrder,
                OpType = StatisticsSalesOrderOpType.ReturnOrder
            });
            return ResponseBase.Success(returnOrder.Id);
        }
    }
}
