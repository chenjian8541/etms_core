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
using ETMS.IBusiness;
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

        private readonly IStudentAccountRechargeCoreBLL _studentAccountRechargeCoreBLL;

        private readonly IStudentAccountRechargeLogDAL _studentAccountRechargeLogDAL;
        public AlienTenantStatistics2BLL(IStatisticsSalesProductDAL statisticsSalesProductDAL,
            IStatisticsSalesTenantDAL statisticsSalesTenantDAL, IOrderDAL orderDAL, IUserDAL userDAL,
            IStudentDAL studentDAL, IStudentAccountRechargeDAL studentAccountRechargeDAL,
            IStudentCourseDAL studentCourseDAL, ICourseDAL courseDAL, IGoodsDAL goodsDAL,
            ICostDAL costDAL, ICouponsDAL couponsDAL, IIncomeLogDAL incomeLogDAL,
            IStudentAccountRechargeLogDAL studentAccountRechargeLogDAL, IStudentAccountRechargeCoreBLL studentAccountRechargeCoreBLL)
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
            this._studentAccountRechargeLogDAL = studentAccountRechargeLogDAL;
            this._studentAccountRechargeCoreBLL = studentAccountRechargeCoreBLL;
        }

        public void InitHeadId(int headId)
        {
        }

        public void InitTenant(int tenantId)
        {
            this._studentAccountRechargeCoreBLL.InitTenantId(tenantId);
            this.InitTenantDataAccess(tenantId, _statisticsSalesProductDAL, _statisticsSalesTenantDAL,
                _orderDAL, _userDAL, _studentDAL, _studentAccountRechargeDAL, _studentCourseDAL,
                _courseDAL, _goodsDAL, _costDAL, _couponsDAL, _incomeLogDAL, _studentAccountRechargeLogDAL);
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

        public async Task<ResponseBase> AlTenantOrderReturnLogGet(AlTenantOrderReturnLogGetRequest request)
        {
            var returnOrder = await _orderDAL.GetUnionOrderSource(request.CId);
            var output = new List<AlTenantOrderReturnLogGetOutput>();
            if (returnOrder.Count > 0)
            {
                var tempBoxUser = new DataTempBox<EtUser>();
                var returnOrderDetail = await _orderDAL.GetOrderDetail(returnOrder.Select(p => p.Id).ToList());
                foreach (var order in returnOrder)
                {
                    var myOutputItem = new AlTenantOrderReturnLogGetOutput()
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
                        LogDetails = new List<AlTenantOrderReturnLogDetail>()
                    };
                    var myOrderDetail = returnOrderDetail.Where(p => p.OrderId == order.Id);
                    foreach (var orderDetail in myOrderDetail)
                    {
                        var productName = string.Empty;
                        switch (orderDetail.ProductType)
                        {
                            case EmProductType.Cost:
                                var myCost = await _costDAL.GetCost(orderDetail.ProductId);
                                productName = myCost?.Name;
                                break;
                            case EmProductType.Goods:
                                var myGoods = await _goodsDAL.GetGoods(orderDetail.ProductId);
                                productName = myGoods?.Name;
                                break;
                            case EmProductType.Course:
                                var myCourse = await _courseDAL.GetCourse(orderDetail.ProductId);
                                productName = myCourse?.Item1.Name;
                                break;
                        }
                        myOutputItem.LogDetails.Add(new AlTenantOrderReturnLogDetail()
                        {
                            CId = orderDetail.Id,
                            ItemAptSum = Math.Abs(orderDetail.ItemAptSum),
                            ItemSum = Math.Abs(orderDetail.ItemSum),
                            OutQuantity = orderDetail.OutQuantity.EtmsToString(),
                            ProductTypeDesc = EmProductType.GetProductType(orderDetail.ProductType),
                            ProductName = productName,
                            OutQuantityDesc = ComBusiness.GetOutQuantityDesc(orderDetail.OutQuantity, orderDetail.BugUnit, orderDetail.ProductType)
                        });
                    }
                    output.Add(myOutputItem);
                }
            }
            return ResponseBase.Success(output);
        }

        public async Task<ResponseBase> AlTenantOrderTransferCoursesLogGet(AlTenantOrderTransferCoursesLogGetRequest request)
        {
            var unionTransferOrder = await _orderDAL.GetUnionTransferOrder(request.CId);
            var output = new List<AlTenantOrderTransferCoursesLogGetOutput>();
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
                        LOG.Log.Error("[AlTenantOrderTransferCoursesLogGet]课程不存在", request, this.GetType());
                        continue;
                    }
                    output.Add(new AlTenantOrderTransferCoursesLogGetOutput
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

        public async Task<ResponseBase> AlTenantOrderTransferCoursesGetDetailGet(AlTenantOrderTransferCoursesGetDetailGetRequest request)
        {
            var order = await _orderDAL.GetOrder(request.CId);
            if (order == null)
            {
                return ResponseBase.CommonError("订单不存在");
            }
            var output = new AlTenantOrderTransferCoursesGetDetailGetOutput()
            {
                InList = new List<AlTenantOrderTransferCoursesGetDetailIn>(),
                OutList = new List<AlTenantOrderTransferCoursesGetDetailOut>()
            };
            var tempBoxUser = new DataTempBox<EtUser>();
            var studentInfo = await OrderStudentGet(order);
            var commissionUsers = await ComBusiness.GetUserMultiSelectValue(tempBoxUser, _userDAL, order.CommissionUser);
            output.BascInfo = new AlTenantOrderTransferCoursesGetDetailBascInfo()
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
                output.InList.Add(new AlTenantOrderTransferCoursesGetDetailIn()
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
                    ProductName = await ComBusiness.GetCourseName(tempBoxCourse, _courseDAL, myItem.ProductId),
                    CId = myItem.Id,
                    OutQuantity = myItem.OutQuantity,
                    OutQuantityDesc = ComBusiness.GetOutQuantityDesc(myItem.OutQuantity, myItem.BugUnit, myItem.ProductType)
                });
            }
            var outDetail = orderDetail.Where(p => p.InOutType == EmOrderInOutType.Out);
            foreach (var myItem in outDetail)
            {
                output.OutList.Add(new AlTenantOrderTransferCoursesGetDetailOut()
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
            output.OrderGetDetailIncomeLogs = new List<AlTenantOrderGetDetailIncomeLog>();
            if (payLog != null && payLog.Any())
            {
                foreach (var p in payLog)
                {
                    output.OrderGetDetailIncomeLogs.Add(new AlTenantOrderGetDetailIncomeLog()
                    {
                        PayOt = p.Ot.EtmsToDateString(),
                        PayType = p.PayType,
                        PayTypeDesc = EmPayType.GetPayType(p.PayType, EmAgtPayType.Lcsw),
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

        public async Task<ResponseBase> AlTenantOrderGetDetailAccountRechargeGet(AlTenantOrderGetDetailAccountRechargeGetRequest request)
        {
            var order = await _orderDAL.GetOrder(request.CId);
            if (order == null)
            {
                return ResponseBase.CommonError("订单不存在");
            }
            var output = new AlTenantOrderGetDetailAccountRechargeGetOutput();
            var tempBoxUser = new DataTempBox<EtUser>();
            var commissionUsers = await ComBusiness.GetUserMultiSelectValue(tempBoxUser, _userDAL, order.CommissionUser);
            output.BascInfo = new AlTenantOrderGetDetailAccountRechargeOutputBasc()
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
                output.RechargeLog = new AlTenantOrderGetDetailAccountRechargeOutputRecharge()
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
            output.IncomeLogs = new List<AlTenantOrderGetDetailAccountRechargeOutputIncomeLog>();
            if (payLog != null && payLog.Any())
            {
                foreach (var p in payLog)
                {
                    output.IncomeLogs.Add(new AlTenantOrderGetDetailAccountRechargeOutputIncomeLog()
                    {
                        PayOt = p.Ot.EtmsToDateString(),
                        PayType = p.PayType,
                        PayTypeDesc = EmPayType.GetPayType(p.PayType, EmAgtPayType.Lcsw),
                        ProjectType = p.ProjectType,
                        ProjectTypeName = EmIncomeLogProjectType.GetIncomeLogProjectType(p.ProjectType),
                        Sum = p.Sum,
                        UserName = await ComBusiness.GetUserName(tempBoxUser, _userDAL, p.UserId),
                        CId = p.Id
                    }); ;
                }
            }
            return ResponseBase.Success(output);
        }
    }
}
