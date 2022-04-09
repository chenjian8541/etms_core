using ETMS.Business.Common;
using ETMS.Entity.Alien.Dto.TenantStatistics.Output;
using ETMS.Entity.Alien.Dto.TenantStatistics.Request;
using ETMS.Entity.Common;
using ETMS.Entity.Database.Source;
using ETMS.Entity.Dto.HisData.Output;
using ETMS.Entity.Enum;
using ETMS.Entity.Enum.EtmsManage;
using ETMS.Entity.Temp;
using ETMS.Entity.View.Persistence;
using ETMS.IBusiness.Alien;
using ETMS.IDataAccess;
using ETMS.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.Business.Alien
{
    public class AlienTenantStatistics2BLL : IAlienTenantStatistics2BLL
    {
        private readonly IStatisticsSalesProductDAL _statisticsSalesProductDAL;

        private readonly IStatisticsSalesTenantDAL _statisticsSalesTenantDAL;

        private readonly IOrderDAL _orderDAL;

        private readonly IUserDAL _userDAL;

        private readonly IStudentDAL _studentDAL;

        private readonly IStudentAccountRechargeDAL _studentAccountRechargeDAL;

        private readonly IStudentCourseDAL _studentCourseDAL;

        private readonly ICourseDAL _courseDAL;

        private readonly IGoodsDAL _goodsDAL;

        private readonly ICostDAL _costDAL;

        private readonly ICouponsDAL _couponsDAL;

        private readonly IIncomeLogDAL _incomeLogDAL;
        public AlienTenantStatistics2BLL(IStatisticsSalesProductDAL statisticsSalesProductDAL,
            IStatisticsSalesTenantDAL statisticsSalesTenantDAL, IOrderDAL orderDAL, IUserDAL userDAL,
            IStudentDAL studentDAL, IStudentAccountRechargeDAL studentAccountRechargeDAL,
            IStudentCourseDAL studentCourseDAL, ICourseDAL courseDAL, IGoodsDAL goodsDAL,
            ICostDAL costDAL, ICouponsDAL couponsDAL, IIncomeLogDAL incomeLogDAL)
        {
            this._statisticsSalesProductDAL = statisticsSalesProductDAL;
            this._statisticsSalesTenantDAL = statisticsSalesTenantDAL;
            this._orderDAL = orderDAL;
            this._userDAL = userDAL;
            this._studentDAL = studentDAL;
            this._studentAccountRechargeDAL = studentAccountRechargeDAL;
            this._studentCourseDAL = studentCourseDAL;
            this._courseDAL = courseDAL;
            this._goodsDAL = goodsDAL;
            this._costDAL = costDAL;
            this._couponsDAL = couponsDAL;
            this._incomeLogDAL = incomeLogDAL;
        }

        public void InitHeadId(int headId)
        {
        }

        public void InitTenant(int tenantId)
        {
            this.InitTenantDataAccess(tenantId, _statisticsSalesProductDAL, _statisticsSalesTenantDAL,
                _orderDAL, _userDAL, _studentDAL, _studentAccountRechargeDAL, _studentCourseDAL,
                _courseDAL, _incomeLogDAL);
        }

        public async Task<ResponseBase> AlTenantStatisticsSalesProductGet(AlTenantStatisticsSalesProductGetRequest request)
        {
            var currentDate = request.StartOt.Value;
            var endDate = request.EndOt.Value;
            var statisticsSalesProductData = await _statisticsSalesProductDAL.GetStatisticsSalesProduct(currentDate, endDate);
            var outPut = new EchartsBarMulti();
            outPut.SourceItems.Add(new List<string>() { "ot", "教程", "物品", "费用" });
            while (currentDate <= endDate)
            {
                var mySourceItem = new List<string>();
                mySourceItem.Add(currentDate.ToString("MM-dd"));
                var myCourseSum = statisticsSalesProductData.FirstOrDefault(p => p.Ot == currentDate);
                if (myCourseSum != null)
                {
                    mySourceItem.Add(myCourseSum.CourseSum.ToString());
                    mySourceItem.Add(myCourseSum.GoodsSum.ToString());
                    mySourceItem.Add(myCourseSum.CostSum.ToString());
                }
                else
                {
                    mySourceItem.Add("0");
                    mySourceItem.Add("0");
                    mySourceItem.Add("0");
                }
                outPut.SourceItems.Add(mySourceItem);
                currentDate = currentDate.AddDays(1);
            }
            return ResponseBase.Success(outPut);
        }

        public async Task<ResponseBase> AlTenantStatisticsSalesTenantEchartsBarMulti1(AlTenantStatisticsSalesTenantEchartsBarMulti1Request request)
        {
            var currentDate = request.StartOt.Value;
            var endDate = request.EndOt.Value;
            var statisticsSalesTenantGroupByOt = await _statisticsSalesTenantDAL.GetStatisticsSalesTenantGroupByOt(currentDate, endDate);
            var outPut = new EchartsBarMulti();
            outPut.SourceItems.Add(new List<string>() { "ot", "销售金额", "新签", "续签", "转课", "退单" });
            while (currentDate <= endDate)
            {
                var mySourceItem = new List<string>();
                mySourceItem.Add(currentDate.ToString("MM-dd"));
                var myCourseSum = statisticsSalesTenantGroupByOt.FirstOrDefault(p => p.Ot == currentDate);
                if (myCourseSum != null)
                {
                    mySourceItem.Add(myCourseSum.TotalOrderSum.ToString());
                    mySourceItem.Add(myCourseSum.TotalOrderNewSum.ToString());
                    mySourceItem.Add(myCourseSum.TotalOrderRenewSum.ToString());
                    mySourceItem.Add(myCourseSum.TotalOrderTransferOutSum.ToString());
                    mySourceItem.Add(myCourseSum.TotalOrderReturnSum.ToString());
                }
                else
                {
                    mySourceItem.Add("0");
                    mySourceItem.Add("0");
                    mySourceItem.Add("0");
                }
                outPut.SourceItems.Add(mySourceItem);
                currentDate = currentDate.AddDays(1);
            }
            return ResponseBase.Success(outPut);
        }

        public async Task<ResponseBase> AlTenantStatisticsSalesTenantEchartsBarMulti2(AlTenantStatisticsSalesTenantEchartsBarMulti2Request request)
        {
            var currentDate = request.StartOt.Value;
            var endDate = request.EndOt.Value;
            var statisticsSalesTenantGroupByOt = await _statisticsSalesTenantDAL.GetStatisticsSalesTenantGroupByOt(currentDate, endDate);
            var outPut = new EchartsBarMulti();
            outPut.SourceItems.Add(new List<string>() { "ot", "销售成单", "新签", "续签" });
            while (currentDate <= endDate)
            {
                var mySourceItem = new List<string>();
                mySourceItem.Add(currentDate.ToString("MM-dd"));
                var myCourseSum = statisticsSalesTenantGroupByOt.FirstOrDefault(p => p.Ot == currentDate);
                if (myCourseSum != null)
                {
                    mySourceItem.Add(myCourseSum.TotalOrderBuyCount.ToString());
                    mySourceItem.Add(myCourseSum.TotalOrderNewCount.ToString());
                    mySourceItem.Add(myCourseSum.TotalOrderRenewCount.ToString());
                }
                else
                {
                    mySourceItem.Add("0");
                    mySourceItem.Add("0");
                    mySourceItem.Add("0");
                }
                outPut.SourceItems.Add(mySourceItem);
                currentDate = currentDate.AddDays(1);
            }
            return ResponseBase.Success(outPut);
        }

        public async Task<ResponseBase> AlTenantStatisticsSalesTenantGet(AlTenantStatisticsSalesTenantGetRequest request)
        {
            var output = new AlTenantStatisticsSalesTenantGetOutput();
            var nowDate = DateTime.Now.Date;
            var thisWeek = EtmsHelper2.GetThisWeek(nowDate);
            var thisWeekData = await _statisticsSalesTenantDAL.GetStatisticsSalesTenant(thisWeek.Item1, thisWeek.Item2);
            if (thisWeekData != null)
            {
                output.ThisWeek = new AlTenantStatisticsSalesTenantValue()
                {
                    OrderBuyCount = thisWeekData.TotalOrderBuyCount,
                    OrderNewCount = thisWeekData.TotalOrderNewCount,
                    OrderNewSum = thisWeekData.TotalOrderNewSum,
                    OrderRenewCount = thisWeekData.TotalOrderRenewCount,
                    OrderRenewSum = thisWeekData.TotalOrderRenewSum,
                    OrderReturnSum = thisWeekData.TotalOrderReturnSum,
                    OrderSum = thisWeekData.TotalOrderSum,
                    OrderTransferOutSum = thisWeekData.TotalOrderTransferOutSum,
                    DateDesc = "本周"
                };
            }
            var thisMonth = EtmsHelper2.GetThisMonth(nowDate);
            var thisMonthData = await _statisticsSalesTenantDAL.GetStatisticsSalesTenant(thisMonth.Item1, thisMonth.Item2);
            if (thisMonthData != null)
            {
                output.ThisMonth = new AlTenantStatisticsSalesTenantValue()
                {
                    OrderBuyCount = thisMonthData.TotalOrderBuyCount,
                    OrderNewCount = thisMonthData.TotalOrderNewCount,
                    OrderNewSum = thisMonthData.TotalOrderNewSum,
                    OrderRenewCount = thisMonthData.TotalOrderRenewCount,
                    OrderRenewSum = thisMonthData.TotalOrderRenewSum,
                    OrderReturnSum = thisMonthData.TotalOrderReturnSum,
                    OrderSum = thisMonthData.TotalOrderSum,
                    OrderTransferOutSum = thisMonthData.TotalOrderTransferOutSum,
                    DateDesc = "本月"
                };
            }

            var lastWeek = EtmsHelper2.GetLastWeek(nowDate);
            var lastWeekData = await _statisticsSalesTenantDAL.GetStatisticsSalesTenant(lastWeek.Item1, lastWeek.Item2);
            if (lastWeekData != null)
            {
                output.LastWeek = new AlTenantStatisticsSalesTenantValue()
                {
                    OrderBuyCount = lastWeekData.TotalOrderBuyCount,
                    OrderNewCount = lastWeekData.TotalOrderNewCount,
                    OrderNewSum = lastWeekData.TotalOrderNewSum,
                    OrderRenewCount = lastWeekData.TotalOrderRenewCount,
                    OrderRenewSum = lastWeekData.TotalOrderRenewSum,
                    OrderReturnSum = lastWeekData.TotalOrderReturnSum,
                    OrderSum = lastWeekData.TotalOrderSum,
                    OrderTransferOutSum = lastWeekData.TotalOrderTransferOutSum,
                    DateDesc = "上周"
                };
            }

            var lastMonth = EtmsHelper2.GetLastMonth(nowDate);
            var lastMonthData = await _statisticsSalesTenantDAL.GetStatisticsSalesTenant(lastMonth.Item1, lastMonth.Item2);
            if (lastMonthData != null)
            {
                output.LastMonth = new AlTenantStatisticsSalesTenantValue()
                {
                    OrderBuyCount = lastMonthData.TotalOrderBuyCount,
                    OrderNewCount = lastMonthData.TotalOrderNewCount,
                    OrderNewSum = lastMonthData.TotalOrderNewSum,
                    OrderRenewCount = lastMonthData.TotalOrderRenewCount,
                    OrderRenewSum = lastMonthData.TotalOrderRenewSum,
                    OrderReturnSum = lastMonthData.TotalOrderReturnSum,
                    OrderSum = lastMonthData.TotalOrderSum,
                    OrderTransferOutSum = lastMonthData.TotalOrderTransferOutSum,
                    DateDesc = "上月"
                };
            }

            return ResponseBase.Success(output);
        }

        public async Task<ResponseBase> AlTenantStatisticsSalesProductMonthGet(AlTenantStatisticsSalesProductMonthGetRequest request)
        {
            DateTime startTime;
            DateTime endTime;
            if (request.Year != null)
            {
                startTime = new DateTime(request.Year.Value, 1, 1);
                endTime = startTime.AddYears(1).AddDays(-1);
            }
            else
            {
                startTime = new DateTime(DateTime.Now.Year, 1, 1);
                endTime = startTime.AddYears(1).AddDays(-1);
            }
            var statisticsSalesMonth = await _statisticsSalesProductDAL.GetEtStatisticsSalesProductMonth(startTime, endTime);
            var echartsBar = new EchartsBar<decimal>();
            var index = 1;
            while (index <= 12)
            {
                var myStatisticsSalesMonth = statisticsSalesMonth.FirstOrDefault(p => p.Month == index);
                echartsBar.XData.Add($"{index}月");
                echartsBar.MyData.Add(myStatisticsSalesMonth == null ? 0 : myStatisticsSalesMonth.SalesTotalSum);
                index++;
            }
            return ResponseBase.Success(echartsBar);
        }

        public async Task<ResponseBase> AlTenantStatisticsSalesProductMonthPagingGet(AlTenantStatisticsSalesProductMonthPagingGetRequest request)
        {
            var pagingData = await _statisticsSalesProductDAL.GetEtStatisticsSalesProductMonthPaging(request);
            var output = new List<AlTenantStatisticsSalesProductMonthPagingGetOutput>();
            foreach (var p in pagingData.Item1)
            {
                output.Add(new AlTenantStatisticsSalesProductMonthPagingGetOutput()
                {
                    Id = p.Id,
                    Month = p.Month,
                    Year = p.Year,
                    SalesTotalSum = p.SalesTotalSum,
                    CostSum = p.CostSum,
                    CourseSum = p.CourseSum,
                    GoodsSum = p.GoodsSum
                });
            }
            return ResponseBase.Success(new ResponsePagingDataBase<AlTenantStatisticsSalesProductMonthPagingGetOutput>(pagingData.Item2, output));
        }

        private DataTempBox<EtStudent> tempBoxStudent = null;
        private async Task<OrderStudentView> OrderStudentGet(EtOrder order)
        {
            if (tempBoxStudent == null)
            {
                tempBoxStudent = new DataTempBox<EtStudent>();
            }
            var orderStudentView = new OrderStudentView()
            {
                StudentId = order.StudentId
            };
            if (order.StudentId > 0)
            {
                var myStudent = await ComBusiness.GetStudent(tempBoxStudent, _studentDAL, order.StudentId);
                if (myStudent != null)
                {
                    orderStudentView.StudentPhone = myStudent.Phone;
                    orderStudentView.StudentName = myStudent.Name;
                    orderStudentView.StudentCardNo = myStudent.CardNo;
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

        public async Task<ResponseBase> AlTenantOrderGetPagingGet(AlTenantOrderGetPagingGetRequest request)
        {
            var pagingData = await _orderDAL.GetOrderPaging(request);
            var orderOutput = new List<AlTenantOrderGetPagingGetOutput>();
            var tempBoxUser = new DataTempBox<EtUser>();
            foreach (var p in pagingData.Item1)
            {
                var studentInfo = await OrderStudentGet(p);
                orderOutput.Add(new AlTenantOrderGetPagingGetOutput()
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
                    OrderTypeDesc = EmOrderType.GetOrderTypeDesc(p.OrderType),
                    OrderSource = p.OrderSource,
                    OrderSourceDesc = EmOrderSource.GetOrderSourceDesc(p.OrderSource)
                });
            }
            return ResponseBase.Success(new ResponsePagingDataBase<AlTenantOrderGetPagingGetOutput>(pagingData.Item2, orderOutput));
        }

        private void ProcessOrderAccountRechargePay(List<AlTenantOrderGetDetailIncomeLog> incomeLogs, EtOrder order, string userName)
        {
            if (order.PayAccountRechargeId == null || (order.PayAccountRechargeReal == 0 && order.PayAccountRechargeGive == 0))
            {
                return;
            }
            incomeLogs.Insert(0, new AlTenantOrderGetDetailIncomeLog()
            {
                PayOt = order.Ot.EtmsToDateString(),
                PayType = EmPayType.PayAccountRecharge,
                PayTypeDesc = EmPayType.GetPayType(EmPayType.PayAccountRecharge, 0),
                ProjectType = 0,
                ProjectTypeName = EmOrderType.GetOrderTypeDesc(order.OrderType),
                Sum = order.PayAccountRechargeReal + order.PayAccountRechargeGive,
                UserName = userName
            });
        }

        public async Task<ResponseBase> AlTenantOrderGetDetailGet(AlTenantOrderGetDetailGetRequest request)
        {
            var order = await _orderDAL.GetOrder(request.CId);
            if (order == null)
            {
                return ResponseBase.CommonError("订单不存在");
            }
            var output = new AlTenantOrderGetDetailGetOutput();
            var tempBoxUser = new DataTempBox<EtUser>();
            var studentInfo = await OrderStudentGet(order);
            var commissionUsers = await ComBusiness.GetUserMultiSelectValue(tempBoxUser, _userDAL, order.CommissionUser);
            output.BascInfo = new AlTenantOrderGetDetailBascInfo()
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
                StudentAvatar = studentInfo.StudentAvatar,
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
                GiveSum = order.AptSum - order.Sum,
                IsHasLcsPay = false,
                OrderSource = order.OrderSource,
                OrderSourceDesc = EmOrderSource.GetOrderSourceDesc(order.OrderSource)
            };
            if (!string.IsNullOrEmpty(order.CouponsIds) && !string.IsNullOrEmpty(order.CouponsStudentGetIds))
            {
                output.OrderGetDetailCoupons = new List<AlTenantOrderGetDetailCoupons>();
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
                    output.OrderGetDetailCoupons.Add(new AlTenantOrderGetDetailCoupons()
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
            output.OrderGetDetailProducts = new List<AlTenantOrderGetDetailProductInfo>();
            var isHasCourse = false;
            var isOnlyOneToOneCourse = true;
            foreach (var myItem in orderDetail)
            {
                var productName = string.Empty;
                var courseDesc = string.Empty;
                switch (myItem.ProductType)
                {
                    case EmProductType.Cost:
                        var myCost = await _costDAL.GetCost(myItem.ProductId);
                        productName = myCost?.Name;
                        break;
                    case EmProductType.Goods:
                        var myGoods = await _goodsDAL.GetGoods(myItem.ProductId);
                        productName = myGoods?.Name;
                        break;
                    case EmProductType.Course:
                        var myCourse = await _courseDAL.GetCourse(myItem.ProductId);
                        isHasCourse = true;
                        if (myCourse != null && myCourse.Item1 != null)
                        {
                            productName = myCourse.Item1.Name;
                            if (myCourse.Item1.Type == EmCourseType.OneToMany)
                            {
                                isOnlyOneToOneCourse = false;
                            }
                            //if (request.IsGetCourseDesc)
                            //{
                            var myStudentCourse = await _studentCourseDAL.GetEtStudentCourseDetail(request.CId, myItem.ProductId);
                            if (myStudentCourse != null && myStudentCourse.EndTime != null)
                            {
                                if (myStudentCourse.DeType == EmDeClassTimesType.ClassTimes)
                                {
                                    courseDesc = $"有效期至：{myStudentCourse.EndTime.EtmsToDateString()}";
                                }
                                else
                                {
                                    courseDesc = $"起止时间：{myStudentCourse.StartTime.EtmsToDateString()}至{myStudentCourse.EndTime.EtmsToDateString()}";
                                }
                            }
                            //}
                        }
                        break;
                }
                output.OrderGetDetailProducts.Add(new AlTenantOrderGetDetailProductInfo()
                {
                    BugUnit = myItem.BugUnit,
                    BuyQuantity = myItem.BuyQuantity,
                    BuyQuantityDesc = ComBusiness.GetBuyQuantityDesc(myItem.BuyQuantity, 0, myItem.BugUnit, myItem.ProductType),
                    DiscountDesc = ComBusiness.GetDiscountDesc(myItem.DiscountValue, myItem.DiscountType),
                    GiveQuantityDesc = ComBusiness.GetGiveQuantityDesc(myItem.GiveQuantity, myItem.GiveUnit),
                    ItemAptSum = Math.Abs(myItem.ItemAptSum),
                    ItemSum = Math.Abs(myItem.ItemSum),
                    PriceRule = myItem.PriceRule,
                    ProductTypeDesc = EmProductType.GetProductType(myItem.ProductType),
                    ProductName = productName,
                    CId = myItem.Id,
                    OutQuantity = myItem.OutQuantity,
                    OutQuantityDesc = ComBusiness.GetOutQuantityDesc(myItem.OutQuantity, myItem.BugUnit, myItem.ProductType),
                    CourseDesc = courseDesc
                });
            }
            var payLog = await _incomeLogDAL.GetIncomeLogByOrderId(request.CId);
            output.OrderGetDetailIncomeLogs = new List<AlTenantOrderGetDetailIncomeLog>();
            if (payLog != null && payLog.Any())
            {
                foreach (var p in payLog)
                {
                    output.OrderGetDetailIncomeLogs.Add(new AlTenantOrderGetDetailIncomeLog()
                    {
                        PayOt = p.Ot.EtmsToDateString(),
                        PayType = p.PayType,
                        PayTypeDesc = EmPayType.GetPayType(p.PayType, EmAgtPayType.Fubei),
                        ProjectType = p.ProjectType,
                        ProjectTypeName = EmIncomeLogProjectType.GetIncomeLogProjectType(p.ProjectType),
                        Sum = p.Sum,
                        UserName = await ComBusiness.GetUserName(tempBoxUser, _userDAL, p.UserId),
                        CId = p.Id
                    });
                    if (p.PayType == EmPayType.AgtPay)
                    {
                        output.BascInfo.IsHasLcsPay = true;
                    }
                }
            }
            ProcessOrderAccountRechargePay(output.OrderGetDetailIncomeLogs, order, output.BascInfo.UserName);

            output.BascInfo.IsHasCourse = isHasCourse;
            output.BascInfo.IsOnlyOneToOneCourse = isOnlyOneToOneCourse;
            return ResponseBase.Success(output);
        }
    }
}
