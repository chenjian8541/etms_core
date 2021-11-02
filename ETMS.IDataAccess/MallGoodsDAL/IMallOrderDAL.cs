using ETMS.Entity.Common;
using ETMS.Entity.Database.Source;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.IDataAccess.MallGoodsDAL
{
    public interface IMallOrderDAL : IBaseDAL
    {
        Task<long> AddMallOrder(EtMallOrder entity);

        Task EditMallOrder(EtMallOrder entity);

        Task SetMallOrderOrderId(long id, long orderId);

        Task<EtMallOrder> GetMallOrder(long id);

        Task<Tuple<IEnumerable<EtMallOrder>, int>> GetPaging(IPagingRequest request);
    }
}
