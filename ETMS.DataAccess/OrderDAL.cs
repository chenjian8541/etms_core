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

        public async Task<EtOrder> GetOrder(long id)
        {
            return await _dbWrapper.Find<EtOrder>(id);
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
    }
}
