using ETMS.Entity.Database.Source;
using ETMS.Entity.Enum;
using ETMS.Event.DataContract;
using ETMS.IBusiness;
using ETMS.IDataAccess;
using ETMS.IEventProvider;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using ETMS.Event.DataContract.Statistics;

namespace ETMS.Business
{
    public class OrderHandleProcessBLL : IOrderHandleProcessBLL
    {
        private readonly IOrderDAL _orderDAL;

        private readonly IStudentCourseDAL _studentCourseDAL;

        private readonly IEventPublisher _eventPublisher;

        private readonly IStudentDAL _studentDAL;

        private readonly IStudentPointsLogDAL _studentPointsLogDAL;

        private readonly IGoodsDAL _goodsDAL;

        private readonly ICostDAL _costDAL;

        private readonly IClassDAL _classDAL;

        private readonly IUserOperationLogDAL _userOperationLogDAL;

        private readonly IIncomeLogDAL _incomeLogDAL;

        private readonly IStudentAccountRechargeDAL _studentAccountRechargeDAL;

        private readonly IStudentAccountRechargeLogDAL _studentAccountRechargeLogDAL;

        private readonly IStudentAccountRechargeCoreBLL _studentAccountRechargeCoreBLL;

        public OrderHandleProcessBLL(IOrderDAL orderDAL, IStudentCourseDAL studentCourseDAL, IEventPublisher eventPublisher, IStudentDAL studentDAL,
            IStudentPointsLogDAL studentPointsLogDAL, IGoodsDAL goodsDAL, ICostDAL costDAL, IClassDAL classDAL, IUserOperationLogDAL userOperationLogDAL,
            IIncomeLogDAL incomeLogDAL, IStudentAccountRechargeDAL studentAccountRechargeDAL, IStudentAccountRechargeLogDAL studentAccountRechargeLogDAL,
            IStudentAccountRechargeCoreBLL studentAccountRechargeCoreBLL)
        {
            this._orderDAL = orderDAL;
            this._studentCourseDAL = studentCourseDAL;
            this._eventPublisher = eventPublisher;
            this._studentDAL = studentDAL;
            this._studentPointsLogDAL = studentPointsLogDAL;
            this._goodsDAL = goodsDAL;
            this._costDAL = costDAL;
            this._classDAL = classDAL;
            this._userOperationLogDAL = userOperationLogDAL;
            this._incomeLogDAL = incomeLogDAL;
            this._studentAccountRechargeDAL = studentAccountRechargeDAL;
            this._studentAccountRechargeLogDAL = studentAccountRechargeLogDAL;
            this._studentAccountRechargeCoreBLL = studentAccountRechargeCoreBLL;
        }

        public void InitTenantId(int tenantId)
        {
            this._studentAccountRechargeCoreBLL.InitTenantId(tenantId);
            this.InitDataAccess(tenantId, _orderDAL, _studentCourseDAL, _studentDAL,
                _studentPointsLogDAL, _goodsDAL, _costDAL, _classDAL, _userOperationLogDAL, _incomeLogDAL,
                _studentAccountRechargeDAL, _studentAccountRechargeLogDAL);
        }

        /// <summary>
        /// 报名订单作废
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task OrderStudentEnrolmentRepealEventProcess(OrderStudentEnrolmentRepealEvent request)
        {
            var order = await _orderDAL.GetOrder(request.OrderId);

            var now = DateTime.Now;
            //学员课程信息处理 （删除此订单所生成的学员课程记录）
            await _studentCourseDAL.DelStudentCourseDetailByOrderId(request.OrderId);
            _eventPublisher.Publish(new StudentCourseAnalyzeEvent(request.TenantId)
            {
                StudentId = order.StudentId
            });

            //充值账户
            if (order.PayAccountRechargeId != null)
            {
                //退还至账户
                var myAccountRecharge = await _studentAccountRechargeDAL.GetStudentAccountRecharge(order.PayAccountRechargeId.Value);
                if (myAccountRecharge == null)
                {
                    LOG.Log.Error($"[OrderStudentEnrolmentRepealEventProcess]订单作废时，充值账户未找到", request, this.GetType());
                }
                else
                {
                    await _studentAccountRechargeCoreBLL.StudentAccountRechargeChange(new StudentAccountRechargeChangeEvent(order.TenantId)
                    {
                        AddBalanceReal = order.PayAccountRechargeReal,
                        AddBalanceGive = order.PayAccountRechargeGive,
                        AddRechargeSum = 0,
                        AddRechargeGiveSum = 0,
                        StudentAccountRechargeId = order.PayAccountRechargeId.Value,
                        TryCount = 0
                    });
                    await _studentAccountRechargeLogDAL.AddStudentAccountRechargeLog(new EtStudentAccountRechargeLog()
                    {
                        TenantId = order.TenantId,
                        CgBalanceGive = order.PayAccountRechargeGive,
                        CgBalanceReal = order.PayAccountRechargeReal,
                        CgNo = order.No,
                        CgServiceCharge = 0,
                        CommissionUser = string.Empty,
                        IsDeleted = order.IsDeleted,
                        Ot = DateTime.Now,
                        Phone = myAccountRecharge.Phone,
                        Remark = "订单作废",
                        RelatedOrderId = order.Id,
                        Status = EmStudentAccountRechargeLogStatus.Normal,
                        StudentAccountRechargeId = order.PayAccountRechargeId.Value,
                        Type = EmStudentAccountRechargeLogType.OrderReturn,
                        UserId = order.UserId
                    });
                }
            }

            //扣除奖励积分
            if (order.TotalPoints > 0)
            {
                await _studentDAL.DeductionPoint(order.StudentId, order.TotalPoints);
                await _studentPointsLogDAL.AddStudentPointsLog(new EtStudentPointsLog()
                {
                    IsDeleted = EmIsDeleted.Normal,
                    No = order.No,
                    Ot = now,
                    Points = order.TotalPoints,
                    Remark = string.Empty,
                    StudentId = order.StudentId,
                    TenantId = order.TenantId,
                    Type = EmStudentPointsLogType.OrderStudentEnrolmentRepeal
                });
            }

            var orderDetail = await _orderDAL.GetOrderDetail(request.OrderId);

            //物品销售数量和库存变动
            var orderGoodsList = orderDetail.Where(p => p.ProductType == EmProductType.Goods);
            if (orderGoodsList.Any())
            {
                foreach (var p in orderGoodsList)
                {
                    await _goodsDAL.AddInventoryAndDeductionSaleQuantity(p.ProductId, p.BuyQuantity);
                    await _goodsDAL.AddGoodsInventoryLog(new EtGoodsInventoryLog()
                    {
                        ChangeQuantity = p.BuyQuantity,
                        GoodsId = p.ProductId,
                        IsDeleted = EmIsDeleted.Normal,
                        Ot = request.CreateTime,
                        Prince = p.Price,
                        Remark = string.Empty,
                        TenantId = request.TenantId,
                        TotalMoney = p.ItemAptSum,
                        Type = EmGoodsInventoryType.OrderStudentEnrolmentRepeal,
                        UserId = request.UserId
                    });
                }
            }

            //费用销售数量
            var orderCostList = orderDetail.Where(p => p.ProductType == EmProductType.Cost);
            if (orderCostList.Any())
            {
                foreach (var p in orderCostList)
                {
                    await _costDAL.DeductioneSaleQuantity(p.ProductId, p.BuyQuantity);
                }
            }

            //删除因此订单所创建的一对一班级
            var myClass = await _classDAL.GetEtClassByOrderId(request.OrderId);
            if (myClass.Any())
            {
                foreach (var p in myClass)
                {
                    await _classDAL.DelClassDepth(p.Id);
                }
            }

            //订单操作记录
            await _orderDAL.AddOrderOperationLog(new EtOrderOperationLog()
            {
                IsDeleted = EmIsDeleted.Normal,
                OpContent = $"订单作废-{request.Remark}",
                OpType = EmOrderOperationLogType.Repeal,
                OrderId = order.Id,
                OrderNo = order.No,
                Ot = now,
                Remark = string.Empty,
                TenantId = order.TenantId,
                UserId = request.UserId
            });

            await _userOperationLogDAL.AddUserLog(new EtUserOperationLog()
            {
                IpAddress = string.Empty,
                IsDeleted = EmIsDeleted.Normal,
                OpContent = $"订单作废-订单号:{order.No},作废原因:{request.Remark}",
                Ot = now,
                Remark = string.Empty,
                TenantId = order.TenantId,
                Type = (int)EmUserOperationType.OrderMgr,
                UserId = order.UserId,
                ClientType = request.LoginClientType
            });

            //统计信息
            _eventPublisher.Publish(new StatisticsSalesProductEvent(request.TenantId)
            {
                StatisticsDate = order.Ot
            });
            _eventPublisher.Publish(new StatisticsFinanceIncomeEvent(request.TenantId)
            {
                StatisticsDate = order.Ot
            });
            _eventPublisher.Publish(new StatisticsSalesCourseEvent(request.TenantId)
            {
                StatisticsDate = order.Ot
            });
            _eventPublisher.Publish(new StatisticsSalesOrderEvent(request.TenantId)
            {
                Order1 = order,
                OpType = StatisticsSalesOrderOpType.Repeal
            });
            _eventPublisher.Publish(new SysTenantStatisticsWeekAndMonthEvent(request.TenantId));
        }

        public async Task OrderReturnProductEventProcess(OrderReturnProductEvent request)
        {
            if (request.SourceOrder.IsReturn != EmBool.True)
            {
                await _orderDAL.SetOrderHasIsReturn(request.SourceOrder.Id);
            }

            var now = request.NewOrder.CreateOt;
            if (request.returnRequest.OrderReturnOrderInfo.DePoint > 0)
            {
                await _studentDAL.DeductionPoint(request.NewOrder.StudentId, request.returnRequest.OrderReturnOrderInfo.DePoint);
                await _studentPointsLogDAL.AddStudentPointsLog(new EtStudentPointsLog()
                {
                    StudentId = request.NewOrder.StudentId,
                    IsDeleted = EmIsDeleted.Normal,
                    No = request.NewOrder.No,
                    Ot = now,
                    Points = request.returnRequest.OrderReturnOrderInfo.DePoint,
                    Remark = request.NewOrder.Remark,
                    TenantId = request.TenantId,
                    Type = EmStudentPointsLogType.OrderReturn
                });
            }

            var changeCourseIds = new List<long>();
            foreach (var returnDetail in request.NewOrderDetails) //处理库存和销售数量
            {
                switch (returnDetail.ProductType)
                {
                    case EmProductType.Course:
                        changeCourseIds.Add(returnDetail.ProductId);
                        break;
                    case EmProductType.Goods:
                        var tempMyGoodsReturnQuantity = (int)returnDetail.OutQuantity;
                        await _goodsDAL.AddInventoryAndDeductionSaleQuantity(returnDetail.ProductId, tempMyGoodsReturnQuantity);
                        await _goodsDAL.AddGoodsInventoryLog(new EtGoodsInventoryLog()
                        {
                            ChangeQuantity = tempMyGoodsReturnQuantity,
                            GoodsId = returnDetail.ProductId,
                            IsDeleted = EmIsDeleted.Normal,
                            Ot = now,
                            Prince = returnDetail.Price,
                            Remark = string.Empty,
                            TenantId = request.TenantId,
                            TotalMoney = returnDetail.ItemAptSum,
                            Type = EmGoodsInventoryType.OrderReturn,
                            UserId = request.UserId
                        });
                        break;
                    case EmProductType.Cost:
                        var tempMyCostReturnQuantity = (int)returnDetail.OutQuantity;
                        await _costDAL.DeductioneSaleQuantity(returnDetail.ProductId, tempMyCostReturnQuantity);
                        break;
                }
            }

            var desc = $"销售退单-{request.NewOrder.BuyCourse} {request.NewOrder.BuyGoods} {request.NewOrder.BuyCost}";
            await _orderDAL.AddOrderOperationLog(new EtOrderOperationLog()
            {
                IsDeleted = EmIsDeleted.Normal,
                OpContent = desc,
                OpType = EmOrderOperationLogType.OrderReturn,
                OrderId = request.SourceOrder.Id,
                OrderNo = request.SourceOrder.No,
                Ot = now,
                Remark = string.Empty,
                TenantId = request.SourceOrder.TenantId,
                UserId = request.NewOrder.UserId
            });

            await _userOperationLogDAL.AddUserLog(request.returnRequest, desc, EmUserOperationType.OrderMgr, now);

            //统计信息
            _eventPublisher.Publish(new StatisticsSalesProductEvent(request.TenantId)
            {
                StatisticsDate = request.NewOrder.Ot
            });
            _eventPublisher.Publish(new StatisticsFinanceIncomeEvent(request.TenantId)
            {
                StatisticsDate = request.NewOrder.Ot
            });
            _eventPublisher.Publish(new StatisticsSalesCourseEvent(request.TenantId)
            {
                StatisticsDate = request.NewOrder.Ot
            });

            _eventPublisher.Publish(new SysTenantStatisticsWeekAndMonthEvent(request.TenantId));

            if (changeCourseIds.Any())
            {
                foreach (var myCourseId in changeCourseIds)
                {
                    _eventPublisher.Publish(new NoticeStudentCourseSurplusEvent(request.TenantId)
                    {
                        CourseId = myCourseId,
                        StudentId = request.SourceOrder.StudentId
                    });
                }
            }
        }
    }
}
