using ETMS.Entity.Common;
using ETMS.Entity.Dto.HisData.Output;
using ETMS.Entity.Dto.HisData.Request;
using ETMS.Event.DataContract;
using ETMS.IBusiness;
using ETMS.IDataAccess;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using ETMS.Entity.Database.Source;
using ETMS.Entity.Enum;
using ETMS.Utility;

namespace ETMS.Business
{
    public class StatisticsSalesBLL : IStatisticsSalesBLL
    {
        private readonly IStatisticsSalesProductDAL _statisticsSalesProductDAL;

        private readonly IStatisticsSalesTenantDAL _statisticsSalesTenantDAL;

        private readonly IStatisticsSalesUserDAL _statisticsSalesUserDAL;

        private readonly IUserDAL _userDAL;

        public StatisticsSalesBLL(IStatisticsSalesProductDAL statisticsSalesProductDAL, IStatisticsSalesTenantDAL statisticsSalesTenantDAL,
           IStatisticsSalesUserDAL statisticsSalesUserDAL, IUserDAL userDAL)
        {
            this._statisticsSalesProductDAL = statisticsSalesProductDAL;
            this._statisticsSalesTenantDAL = statisticsSalesTenantDAL;
            this._statisticsSalesUserDAL = statisticsSalesUserDAL;
            this._userDAL = userDAL;
        }

        public void InitTenantId(int tenantId)
        {
            this.InitDataAccess(tenantId, _statisticsSalesProductDAL,
                _statisticsSalesTenantDAL, _statisticsSalesUserDAL, _userDAL);
        }

        public async Task StatisticsSalesProductConsumeEvent(StatisticsSalesProductEvent request)
        {
            await _statisticsSalesProductDAL.UpdateStatisticsSales(request.StatisticsDate.Date);
            await _statisticsSalesProductDAL.UpdateStatisticsSalesProductMonth(request.StatisticsDate.Date);
        }

        public async Task<ResponseBase> GetStatisticsSalesProduct(GetStatisticsSalesProductRequest request)
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

        public async Task<ResponseBase> GetStatisticsSalesProductProportion(GetStatisticsSalesProductProportionRequest request)
        {
            var statisticsSalesProductData = await _statisticsSalesProductDAL.GetStatisticsSalesProduct(request.StartOt.Value, request.EndOt.Value);
            var myTotalCourseSum = statisticsSalesProductData.Sum(p => p.CourseSum);
            var myTotalGoodsSum = statisticsSalesProductData.Sum(p => p.GoodsSum);
            var myCostSum = statisticsSalesProductData.Sum(p => p.CostSum);
            var echartsStatisticsSalesProduct = new EchartsPie<decimal>();
            echartsStatisticsSalesProduct.LegendData.Add("教程");
            echartsStatisticsSalesProduct.MyData.Add(new EchartsPieData<decimal>() { Name = "教程", Value = myTotalCourseSum });
            echartsStatisticsSalesProduct.LegendData.Add("物品");
            echartsStatisticsSalesProduct.MyData.Add(new EchartsPieData<decimal>() { Name = "物品", Value = myTotalGoodsSum });
            echartsStatisticsSalesProduct.LegendData.Add("费用");
            echartsStatisticsSalesProduct.MyData.Add(new EchartsPieData<decimal>() { Name = "费用", Value = myCostSum });
            return ResponseBase.Success(echartsStatisticsSalesProduct);
        }

        public async Task<ResponseBase> GetStatisticsSalesProductMonth(GetStatisticsSalesProductMonthRequest request)
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

        public async Task<ResponseBase> GetStatisticsSalesProductMonthPaging(GetStatisticsSalesProductMonthPagingRequest request)
        {
            var pagingData = await _statisticsSalesProductDAL.GetEtStatisticsSalesProductMonthPaging(request);
            var output = new List<GetStatisticsSalesProductMonthPagingOutput>();
            foreach (var p in pagingData.Item1)
            {
                output.Add(new GetStatisticsSalesProductMonthPagingOutput()
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
            return ResponseBase.Success(new ResponsePagingDataBase<GetStatisticsSalesProductMonthPagingOutput>(pagingData.Item2, output));
        }

        public async Task StatisticsSalesOrderConsumerEvent(StatisticsSalesOrderEvent request)
        {
            if (!EmOrderType.IsBuyOrder(request.Order1.OrderType))
            {
                return;
            }
            //处理提成人,如果未设置提成人，则加入“未分配”统计
            if (string.IsNullOrEmpty(request.Order1.CommissionUser))
            {
                request.Order1.CommissionUser = "0";
            }
            if (string.IsNullOrEmpty(request.OldCommissionUser))
            {
                request.OldCommissionUser = "0";
            }
            switch (request.OpType)
            {
                case StatisticsSalesOrderOpType.StudentEnrolment:
                    await StatisticsSalesOrderConsumerEventStudentEnrolment(request);
                    break;
                case StatisticsSalesOrderOpType.Repeal:
                    await StatisticsSalesOrderConsumerEventRepeal(request);
                    break;
                case StatisticsSalesOrderOpType.ReturnOrder:
                    await StatisticsSalesOrderConsumerEventReturnOrder(request);
                    break;
                case StatisticsSalesOrderOpType.TransferCourse:
                    await StatisticsSalesOrderConsumerEventTransferCourse(request);
                    break;
                case StatisticsSalesOrderOpType.ChangeCommissionUser:
                    await StatisticsSalesOrderConsumerEventChangeCommissionUser(request);
                    break;
            }
        }

        private void ProcessTotal(EtStatisticsSalesTenant log)
        {
            log.OrderBuyCount = log.OrderNewCount + log.OrderRenewCount;
            log.OrderSum = log.OrderNewSum + log.OrderRenewSum + log.OrderTransferOutSum + log.OrderReturnSum;
        }

        private void ProcessTotal(EtStatisticsSalesUser log)
        {
            log.OrderBuyCount = log.OrderNewCount + log.OrderRenewCount;
            log.OrderSum = log.OrderNewSum + log.OrderRenewSum + log.OrderTransferOutSum + log.OrderReturnSum;
        }

        private async Task StatisticsSalesOrderConsumerEventStudentEnrolment(StatisticsSalesOrderEvent request)
        {
            //处理机构销售数据
            var order = request.Order1;
            var ot = order.Ot.Date;
            var addNewCount = 0;
            var addRenewCount = 0;
            var addNewSum = 0M;
            var addRenewSum = 0M;
            if (order.BuyType == EmOrderBuyType.New)
            {
                addNewCount = 1;
                addNewSum = order.AptSum;
            }
            else
            {
                addRenewCount = 1;
                addRenewSum = order.AptSum;
            }

            var hisData = await _statisticsSalesTenantDAL.GetStatisticsSalesTenant(ot);
            if (hisData == null)
            {
                hisData = new EtStatisticsSalesTenant()
                {
                    TenantId = request.TenantId,
                    Ot = ot,
                    IsDeleted = EmIsDeleted.Normal
                };
            }
            hisData.OrderNewCount += addNewCount;
            hisData.OrderRenewCount += addRenewCount;
            hisData.OrderNewSum += addNewSum;
            hisData.OrderRenewSum += addRenewSum;
            ProcessTotal(hisData);
            await _statisticsSalesTenantDAL.SaveStatisticsSalesTenant(hisData);

            //处理提成人销售数据
            if (string.IsNullOrEmpty(order.CommissionUser))
            {
                return;
            }
            var commissionUsers = EtmsHelper.AnalyzeMuIds(order.CommissionUser);
            foreach (var p in commissionUsers)
            {
                var userHisData = await _statisticsSalesUserDAL.GetStatisticsSalesUser(p, ot);
                if (userHisData == null)
                {
                    userHisData = new EtStatisticsSalesUser()
                    {
                        TenantId = request.TenantId,
                        Ot = ot,
                        IsDeleted = EmIsDeleted.Normal,
                        UserId = p
                    };
                }
                userHisData.OrderNewCount += addNewCount;
                userHisData.OrderRenewCount += addRenewCount;
                userHisData.OrderNewSum += addNewSum;
                userHisData.OrderRenewSum += addRenewSum;
                ProcessTotal(userHisData);
                await _statisticsSalesUserDAL.SaveStatisticsSalesUser(userHisData);
            }
        }

        private async Task StatisticsSalesOrderConsumerEventRepeal(StatisticsSalesOrderEvent request)
        {
            if (request.Order1.OrderType != EmOrderType.StudentEnrolment)
            {
                return;
            }
            //处理机构销售数据
            var order = request.Order1;
            var ot = order.Ot.Date;
            var addNewCount = 0;
            var addRenewCount = 0;
            var addNewSum = 0M;
            var addRenewSum = 0M;
            if (order.BuyType == EmOrderBuyType.New)
            {
                addNewCount = -1;
                addNewSum = -order.AptSum;
            }
            else
            {
                addRenewCount = -1;
                addRenewSum = -order.AptSum;
            }

            var hisData = await _statisticsSalesTenantDAL.GetStatisticsSalesTenant(ot);
            if (hisData == null)
            {
                hisData = new EtStatisticsSalesTenant()
                {
                    TenantId = request.TenantId,
                    Ot = ot,
                    IsDeleted = EmIsDeleted.Normal
                };
            }
            hisData.OrderNewCount += addNewCount;
            hisData.OrderRenewCount += addRenewCount;
            hisData.OrderNewSum += addNewSum;
            hisData.OrderRenewSum += addRenewSum;
            ProcessTotal(hisData);
            await _statisticsSalesTenantDAL.SaveStatisticsSalesTenant(hisData);

            //处理提成人销售数据
            if (string.IsNullOrEmpty(order.CommissionUser))
            {
                return;
            }
            var commissionUsers = EtmsHelper.AnalyzeMuIds(order.CommissionUser);
            foreach (var p in commissionUsers)
            {
                var userHisData = await _statisticsSalesUserDAL.GetStatisticsSalesUser(p, ot);
                if (userHisData == null)
                {
                    userHisData = new EtStatisticsSalesUser()
                    {
                        TenantId = request.TenantId,
                        Ot = ot,
                        IsDeleted = EmIsDeleted.Normal,
                        UserId = p
                    };
                }
                userHisData.OrderNewCount += addNewCount;
                userHisData.OrderRenewCount += addRenewCount;
                userHisData.OrderNewSum += addNewSum;
                userHisData.OrderRenewSum += addRenewSum;
                ProcessTotal(userHisData);
                await _statisticsSalesUserDAL.SaveStatisticsSalesUser(userHisData);
            }
        }

        private async Task StatisticsSalesOrderConsumerEventReturnOrder(StatisticsSalesOrderEvent request)
        {
            //处理机构销售数据
            var order = request.Order1;
            var ot = order.Ot.Date;
            var deOrderReturnSum = -Math.Abs(order.AptSum);

            var hisData = await _statisticsSalesTenantDAL.GetStatisticsSalesTenant(ot);
            if (hisData == null)
            {
                hisData = new EtStatisticsSalesTenant()
                {
                    TenantId = request.TenantId,
                    Ot = ot,
                    IsDeleted = EmIsDeleted.Normal
                };
            }
            hisData.OrderReturnSum += deOrderReturnSum;
            ProcessTotal(hisData);
            await _statisticsSalesTenantDAL.SaveStatisticsSalesTenant(hisData);

            //处理提成人销售数据
            if (string.IsNullOrEmpty(order.CommissionUser))
            {
                return;
            }
            var commissionUsers = EtmsHelper.AnalyzeMuIds(order.CommissionUser);
            foreach (var p in commissionUsers)
            {
                var userHisData = await _statisticsSalesUserDAL.GetStatisticsSalesUser(p, ot);
                if (userHisData == null)
                {
                    userHisData = new EtStatisticsSalesUser()
                    {
                        TenantId = request.TenantId,
                        Ot = ot,
                        IsDeleted = EmIsDeleted.Normal,
                        UserId = p
                    };
                }
                userHisData.OrderReturnSum += deOrderReturnSum;
                ProcessTotal(userHisData);
                await _statisticsSalesUserDAL.SaveStatisticsSalesUser(userHisData);
            }
        }

        private async Task StatisticsSalesOrderConsumerEventTransferCourse(StatisticsSalesOrderEvent request)
        {
            //处理机构销售数据
            var order = request.Order1;
            var ot = order.Ot.Date;

            var changeTransferOutSum = Math.Abs(order.AptSum);
            if (order.InOutType == EmOrderInOutType.Out)
            {
                changeTransferOutSum = -changeTransferOutSum;
            }

            var hisData = await _statisticsSalesTenantDAL.GetStatisticsSalesTenant(ot);
            if (hisData == null)
            {
                hisData = new EtStatisticsSalesTenant()
                {
                    TenantId = request.TenantId,
                    Ot = ot,
                    IsDeleted = EmIsDeleted.Normal
                };
            }
            hisData.OrderTransferOutSum += changeTransferOutSum;
            ProcessTotal(hisData);
            await _statisticsSalesTenantDAL.SaveStatisticsSalesTenant(hisData);

            //处理提成人销售数据
            if (string.IsNullOrEmpty(order.CommissionUser))
            {
                return;
            }
            var commissionUsers = EtmsHelper.AnalyzeMuIds(order.CommissionUser);
            foreach (var p in commissionUsers)
            {
                var userHisData = await _statisticsSalesUserDAL.GetStatisticsSalesUser(p, ot);
                if (userHisData == null)
                {
                    userHisData = new EtStatisticsSalesUser()
                    {
                        TenantId = request.TenantId,
                        Ot = ot,
                        IsDeleted = EmIsDeleted.Normal,
                        UserId = p
                    };
                }
                userHisData.OrderTransferOutSum += changeTransferOutSum;
                ProcessTotal(userHisData);
                await _statisticsSalesUserDAL.SaveStatisticsSalesUser(userHisData);
            }
        }

        private async Task StatisticsSalesOrderConsumerEventChangeCommissionUser(StatisticsSalesOrderEvent request)
        {
            var order = request.Order1;
            var ot = order.Ot.Date;
            if (request.OldCommissionUser == order.CommissionUser)
            {
                return;
            }
            var aptSum = Math.Abs(order.AptSum);
            //修改前 业绩归属人处理
            if (!string.IsNullOrEmpty(request.OldCommissionUser))
            {
                var oldOrderNewCount = 0;
                var oldOrderRenewCount = 0;
                var oldOrderNewSum = 0M;
                var oldOrderRenewSum = 0M;
                var oldOrderReturnSum = 0M;
                var oldOrderTransferOutSum = 0M;
                switch (order.OrderType)
                {
                    case EmOrderType.StudentEnrolment:
                        if (order.BuyType == EmOrderBuyType.New)
                        {
                            oldOrderNewCount = 1;
                            oldOrderNewSum = aptSum;
                        }
                        else
                        {
                            oldOrderRenewCount = 1;
                            oldOrderRenewSum = aptSum;
                        }
                        break;
                    case EmOrderType.ReturnOrder:
                        oldOrderReturnSum = -aptSum;
                        break;
                    case EmOrderType.TransferCourse:
                        if (order.InOutType == EmOrderInOutType.In)
                        {
                            oldOrderTransferOutSum = aptSum;
                        }
                        else
                        {
                            oldOrderTransferOutSum = -aptSum;
                        }
                        break;
                }
                var oldCommissionUsers = EtmsHelper.AnalyzeMuIds(request.OldCommissionUser);
                foreach (var p in oldCommissionUsers)
                {
                    var oldUserHisData = await _statisticsSalesUserDAL.GetStatisticsSalesUser(p, ot);
                    if (oldUserHisData == null)
                    {
                        oldUserHisData = new EtStatisticsSalesUser()
                        {
                            TenantId = request.TenantId,
                            Ot = ot,
                            IsDeleted = EmIsDeleted.Normal,
                            UserId = p
                        };
                    }
                    oldUserHisData.OrderNewCount += -oldOrderNewCount;
                    oldUserHisData.OrderRenewCount += -oldOrderRenewCount;
                    oldUserHisData.OrderNewSum += -oldOrderNewSum;
                    oldUserHisData.OrderRenewSum += -oldOrderRenewSum;
                    oldUserHisData.OrderReturnSum += -oldOrderReturnSum;
                    oldUserHisData.OrderTransferOutSum += -oldOrderTransferOutSum;
                    ProcessTotal(oldUserHisData);
                    await _statisticsSalesUserDAL.SaveStatisticsSalesUser(oldUserHisData);
                }
            }

            //修改后 业绩归属人处理
            if (!string.IsNullOrEmpty(order.CommissionUser))
            {
                var newOrderNewCount = 0;
                var newOrderRenewCount = 0;
                var newOrderNewSum = 0M;
                var newOrderRenewSum = 0M;
                var newOrderTransferOutSum = 0M;
                var newOrderReturnSum = 0M;
                switch (order.OrderType)
                {
                    case EmOrderType.StudentEnrolment:
                        if (order.BuyType == EmOrderBuyType.New)
                        {
                            newOrderNewCount = 1;
                            newOrderNewSum = aptSum;
                        }
                        else
                        {
                            newOrderRenewCount = 1;
                            newOrderRenewSum = aptSum;
                        }
                        break;
                    case EmOrderType.ReturnOrder:
                        newOrderReturnSum = -aptSum;
                        break;
                    case EmOrderType.TransferCourse:
                        if (order.InOutType == EmOrderInOutType.In)
                        {
                            newOrderTransferOutSum = aptSum;
                        }
                        else
                        {
                            newOrderTransferOutSum = -aptSum;
                        }
                        break;
                }
                var newCommissionUsers = EtmsHelper.AnalyzeMuIds(order.CommissionUser);
                foreach (var p in newCommissionUsers)
                {
                    var newUserHisData = await _statisticsSalesUserDAL.GetStatisticsSalesUser(p, ot);
                    if (newUserHisData == null)
                    {
                        newUserHisData = new EtStatisticsSalesUser()
                        {
                            TenantId = request.TenantId,
                            Ot = ot,
                            IsDeleted = EmIsDeleted.Normal,
                            UserId = p
                        };
                    }
                    newUserHisData.OrderNewCount += newOrderNewCount;
                    newUserHisData.OrderRenewCount += newOrderRenewCount;
                    newUserHisData.OrderNewSum += newOrderNewSum;
                    newUserHisData.OrderRenewSum += newOrderRenewSum;
                    newUserHisData.OrderReturnSum += newOrderReturnSum;
                    newUserHisData.OrderTransferOutSum += newOrderTransferOutSum;
                    ProcessTotal(newUserHisData);
                    await _statisticsSalesUserDAL.SaveStatisticsSalesUser(newUserHisData);
                }
            }
        }

        public async Task<ResponseBase> StatisticsSalesTenantGet(RequestBase request)
        {
            var output = new StatisticsSalesTenantGetOutput();
            var nowDate = DateTime.Now.Date;
            var thisWeek = EtmsHelper2.GetThisWeek(nowDate);
            var thisWeekData = await _statisticsSalesTenantDAL.GetStatisticsSalesTenant(thisWeek.Item1, thisWeek.Item2);
            if (thisWeekData != null)
            {
                output.ThisWeek = new StatisticsSalesTenantValue()
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
                output.ThisMonth = new StatisticsSalesTenantValue()
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
                output.LastWeek = new StatisticsSalesTenantValue()
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
                output.LastMonth = new StatisticsSalesTenantValue()
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

        public async Task<ResponseBase> StatisticsSalesTenantGet2(StatisticsSalesTenantGet2Request request)
        {
            var output = new StatisticsSalesTenantValue();
            var myData = await _statisticsSalesTenantDAL.GetStatisticsSalesTenant(request.StartOt.Value, request.EndOt.Value);
            if (myData != null)
            {
                output = new StatisticsSalesTenantValue()
                {
                    OrderBuyCount = myData.TotalOrderBuyCount,
                    OrderNewCount = myData.TotalOrderNewCount,
                    OrderNewSum = myData.TotalOrderNewSum,
                    OrderRenewCount = myData.TotalOrderRenewCount,
                    OrderRenewSum = myData.TotalOrderRenewSum,
                    OrderReturnSum = myData.TotalOrderReturnSum,
                    OrderSum = myData.TotalOrderSum,
                    OrderTransferOutSum = myData.TotalOrderTransferOutSum
                };
            }
            return ResponseBase.Success(output);
        }

        public async Task<ResponseBase> StatisticsSalesUserGet(StatisticsSalesUserGetRequest request)
        {
            var outPut = new List<StatisticsSalesUserGetOutput>();
            var myData = await _statisticsSalesUserDAL.GetStatisticsSalesUser(request.StartOt.Value, request.EndOt.Value, request.OrderType);
            if (myData != null && myData.Any())
            {
                foreach (var p in myData)
                {
                    var userName = string.Empty;
                    if (p.UserId > 0)
                    {
                        var user = await _userDAL.GetUser(p.UserId);
                        if (user == null)
                        {
                            continue;
                        }
                        userName = user.Name;
                    }
                    else
                    {
                        userName = "未分配";
                    }
                    outPut.Add(new StatisticsSalesUserGetOutput()
                    {
                        UserId = p.UserId,
                        OrderBuyCount = p.TotalOrderBuyCount,
                        OrderNewCount = p.TotalOrderNewCount,
                        OrderNewSum = p.TotalOrderNewSum,
                        OrderRenewCount = p.TotalOrderRenewCount,
                        OrderRenewSum = p.TotalOrderRenewSum,
                        OrderReturnSum = p.TotalOrderReturnSum,
                        OrderSum = p.TotalOrderSum,
                        OrderTransferOutSum = p.TotalOrderTransferOutSum,
                        UserName = userName
                    });
                }
            }
            return ResponseBase.Success(outPut);
        }

        public async Task<ResponseBase> StatisticsSalesTenantEchartsBarMulti1(StatisticsSalesTenantEchartsBarMulti1Request request)
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

        public async Task<ResponseBase> StatisticsSalesTenantEchartsBarMulti2(StatisticsSalesTenantEchartsBarMulti2Request request)
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
    }
}
