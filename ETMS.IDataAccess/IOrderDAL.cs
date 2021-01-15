using ETMS.Entity.Common;
using ETMS.Entity.Database.Source;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.IDataAccess
{
    public interface IOrderDAL : IBaseDAL
    {
        Task<long> AddOrder(EtOrder order);

        bool AddOrderDetail(List<EtOrderDetail> orderDetails);

        Task<long> AddOrder(EtOrder order, List<EtOrderDetail> orderDetails);

        Task<bool> ExistProduct(byte productType, long productId);

        Task<Tuple<IEnumerable<EtOrder>, int>> GetOrderPaging(IPagingRequest request);

        Task<EtOrder> GetOrder(long id);

        Task<List<EtOrderDetail>> GetOrderDetail(long orderId);

        Task<EtOrderDetail> GetOrderDetail(long orderId, long productId, byte productType);

        Task<bool> EditOrderDetail(List<EtOrderDetail> entitys);

        Task<bool> UpdateOrder(EtOrder order);

        Task<bool> AddOrderOperationLog(EtOrderOperationLog etOrderOperationLog);

        bool AddOrderOperationLog(List<EtOrderOperationLog> etOrderOperationLogs);

        Task<Tuple<IEnumerable<EtOrderOperationLog>, int>> GetOrderOperationLogPaging(IPagingRequest request);

        Task<bool> OrderStudentEnrolmentRepeal(long orderId);

        Task<List<EtOrder>> GetUnionOrderSource(long orderId);

        Task<IEnumerable<EtOrder>> GetUnionTransferOrder(long orderId);

        Task<bool> ExistOutOrder(long orderId);

        Task<List<EtOrderDetail>> GetOrderDetail(List<long> orderIds);

        Task<EtOrderDetail> GetOrderDetailById(long orderDetailId);

        Task<bool> SetOrderHasIsReturn(long orderId);

        Task<bool> SetOrderHasIsTransferCourse(List<long> orderIds);
    }
}
