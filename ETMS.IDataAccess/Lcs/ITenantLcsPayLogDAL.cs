using ETMS.Entity.Common;
using ETMS.Entity.Database.Source;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.IDataAccess.Lcs
{
    public interface ITenantLcsPayLogDAL : IBaseDAL
    {
        Task<long> AddTenantLcsPayLog(EtTenantLcsPayLog entity);

        Task<EtTenantLcsPayLog> GetTenantLcsPayLog(long id);

        Task<EtTenantLcsPayLog> GetTenantLcsPayLogBuyOutTradeNo(string outTradeNo);

        Task EditTenantLcsPayLog(EtTenantLcsPayLog entity);

        Task<Tuple<IEnumerable<EtTenantLcsPayLog>, int>> GetTenantLcsPayLogPaging(IPagingRequest request);

        Task UpdateTenantLcsPayLog(long id, string outTradeNo, string payType, string subAppid, string totalFee);
    }
}
