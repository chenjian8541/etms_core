using ETMS.DataAccess.Core;
using ETMS.Entity.Common;
using ETMS.Entity.Database.Source;
using ETMS.Entity.Enum;
using ETMS.IDataAccess;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using ETMS.Entity.Temp;

namespace ETMS.DataAccess
{
    public class OrderDAL : DataAccessBase, IOrderDAL
    {
        public OrderDAL(IDbWrapper dbWrapper) : base(dbWrapper)
        { }

        public async Task<long> AddOrder(EtOrder order)
        {
            await _dbWrapper.Insert(order);
            return order.Id;
        }

        public bool AddOrderDetail(List<EtOrderDetail> orderDetails)
        {
            _dbWrapper.InsertRange(orderDetails);
            return true;
        }

        public async Task<long> AddOrder(EtOrder order, List<EtOrderDetail> orderDetails)
        {
            await _dbWrapper.Insert(order);
            foreach (var detail in orderDetails)
            {
                detail.Status = order.Status;
                detail.OrderId = order.Id;
            }
            _dbWrapper.InsertRange(orderDetails);
            return order.Id;
        }

        public async Task<bool> ExistProduct(byte productType, long productId)
        {
            var obj = await _dbWrapper.ExecuteScalar($"SELECT TOP 1 0 FROM EtOrderDetail WHERE ProductType = {productType} AND ProductId = {productId} AND IsDeleted = {EmIsDeleted.Normal}");
            return obj != null;
        }

        public async Task<Tuple<IEnumerable<EtOrder>, int>> GetOrderPaging(IPagingRequest request)
        {
            return await _dbWrapper.ExecutePage<EtOrder>("EtOrder", "*", request.PageSize, request.PageCurrent, "Id DESC", request.ToString());
        }

        public async Task<Tuple<IEnumerable<EtOrderDetail>, int>> GetOrderDetailPaging(IPagingRequest request)
        {
            return await _dbWrapper.ExecutePage<EtOrderDetail>("EtOrderDetail", "*", request.PageSize, request.PageCurrent, "Id DESC", request.ToString());
        }

        public async Task<EtOrder> GetOrder(long id)
        {
            return await _dbWrapper.Find<EtOrder>(id);
        }

        public async Task<EtOrder> GetOrder(string orderNo)
        {
            return await _dbWrapper.Find<EtOrder>(p => p.No == orderNo && p.IsDeleted == EmIsDeleted.Normal && p.TenantId == _tenantId);
        }

        public async Task<List<EtOrderDetail>> GetOrderDetail(long orderId)
        {
            return await _dbWrapper.FindList<EtOrderDetail>(p => p.OrderId == orderId && p.IsDeleted == EmIsDeleted.Normal);
        }

        public async Task<EtOrderDetail> GetOrderDetail(long orderId, long productId, byte productType)
        {
            return await _dbWrapper.Find<EtOrderDetail>(p => p.TenantId == _tenantId && p.OrderId == orderId
            && p.IsDeleted == EmIsDeleted.Normal && p.ProductId == productId && p.ProductType == productType);
        }

        public async Task<bool> EditOrderDetail(List<EtOrderDetail> entitys)
        {
            await _dbWrapper.UpdateRange(entitys);
            return true;
        }

        public async Task<bool> UpdateOrder(EtOrder order)
        {
            return await _dbWrapper.Update(order);
        }

        public async Task<bool> AddOrderOperationLog(EtOrderOperationLog etOrderOperationLog)
        {
            await _dbWrapper.Insert(etOrderOperationLog);
            return true;
        }

        public bool AddOrderOperationLog(List<EtOrderOperationLog> etOrderOperationLogs)
        {
            _dbWrapper.InsertRange(etOrderOperationLogs);
            return true;
        }

        public async Task<Tuple<IEnumerable<EtOrderOperationLog>, int>> GetOrderOperationLogPaging(IPagingRequest request)
        {
            return await _dbWrapper.ExecutePage<EtOrderOperationLog>("EtOrderOperationLog", "*", request.PageSize, request.PageCurrent, "Id DESC", request.ToString());
        }

        /// <summary>
        /// 报名订单作废
        /// </summary>
        /// <param name="orderId"></param>
        /// <returns></returns>
        public async Task<bool> OrderStudentEnrolmentRepeal(long orderId)
        {
            var sql = new StringBuilder();
            sql.Append($"UPDATE EtOrder SET [Status] = {EmOrderStatus.Repeal} WHERE TenantId = {_tenantId} AND Id = {orderId} ;");
            sql.Append($"UPDATE EtOrderDetail SET [Status] = {EmOrderStatus.Repeal} WHERE TenantId = {_tenantId} AND OrderId = {orderId} ;");
            sql.Append($"UPDATE EtIncomeLog SET [Status] = {EmIncomeLogStatus.Repeal} WHERE TenantId = {_tenantId} AND OrderId = {orderId} ;");
            await _dbWrapper.Execute(sql.ToString());
            return true;
        }

        public async Task<List<EtOrder>> GetUnionOrderSource(long orderId)
        {
            return await this._dbWrapper.FindList<EtOrder>(p => p.TenantId == _tenantId && p.IsDeleted == EmIsDeleted.Normal && p.UnionOrderId == orderId);
        }

        public async Task<IEnumerable<EtOrder>> GetUnionTransferOrder(long orderId)
        {
            return await _dbWrapper.ExecuteObject<EtOrder>($"SELECT * FROM EtOrder WHERE TenantId = {_tenantId} AND IsDeleted = {EmIsDeleted.Normal} AND UnionTransferOrderIds LIKE '%,{orderId},%'");
        }

        public async Task<bool> ExistOutOrder(long orderId)
        {
            var sql = $"SELECT TOP 1 0 FROM EtOrderDetail WHERE TenantId = {_tenantId} AND IsDeleted = {EmIsDeleted.Normal} AND OutOrderId = {orderId} and [Status] <> {EmOrderStatus.Repeal}";
            var obj = await _dbWrapper.ExecuteScalar(sql);
            return obj != null;
        }

        public async Task<List<EtOrderDetail>> GetOrderDetail(List<long> orderIds)
        {
            if (orderIds.Count == 1)
            {
                return await GetOrderDetail(orderIds[0]);
            }
            var temp = await _dbWrapper.ExecuteObject<EtOrderDetail>($"SELECT * FROM EtOrderDetail WHERE OrderId IN ({string.Join(',', orderIds)}) AND IsDeleted = {EmIsDeleted.Normal} ");
            return temp.ToList();
        }

        public async Task<EtOrderDetail> GetOrderDetailById(long orderDetailId)
        {
            return await _dbWrapper.Find<EtOrderDetail>(p => p.TenantId == _tenantId && p.Id == orderDetailId && p.IsDeleted == EmIsDeleted.Normal);
        }

        public async Task<bool> SetOrderHasIsReturn(long orderId)
        {
            await _dbWrapper.Execute($"UPDATE EtOrder SET IsReturn = {EmBool.True} WHERE Id = {orderId}");
            return true;
        }

        public async Task<bool> SetOrderHasIsTransferCourse(List<long> orderIds)
        {
            if (orderIds.Count == 1)
            {
                await _dbWrapper.Execute($"UPDATE EtOrder SET IsTransferCourse = {EmBool.True} WHERE Id = {orderIds[0]} ");
            }
            else
            {
                await _dbWrapper.Execute($"UPDATE EtOrder SET IsTransferCourse = {EmBool.True} WHERE Id IN ({string.Join(',', orderIds)}) ");
            }
            return true;
        }

        public async Task<bool> ExistOrderProduct(byte productType, long productId)
        {
            var log = await _dbWrapper.Find<EtOrderDetail>(p => p.TenantId == _tenantId && p.ProductType == productType
            && p.ProductId == productId && p.IsDeleted == EmIsDeleted.Normal);
            return log != null;
        }

        public async Task<IEnumerable<OrderStudentOt>> GetOrderStudentOt(long studentId)
        {
            var sql = $"SELECT TOP 200 Id,Ot FROM EtOrder WHERE TenantId = {_tenantId} AND StudentId = {studentId} AND IsDeleted = {EmIsDeleted.Normal}";
            return await _dbWrapper.ExecuteObject<OrderStudentOt>(sql);
        }

        public async Task UpdateOrderDetailStatus(long orderId, byte newStatus)
        {
            await _dbWrapper.Execute($"UPDATE EtOrderDetail SET [Status] = {newStatus} WHERE TenantId = {_tenantId} AND OrderId = {orderId}");
        }
    }
}
