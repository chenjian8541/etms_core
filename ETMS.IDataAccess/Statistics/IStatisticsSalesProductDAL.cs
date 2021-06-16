using ETMS.Entity.Common;
using ETMS.Entity.Database.Source;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.IDataAccess
{
    public interface IStatisticsSalesProductDAL: IBaseDAL
    {
        Task UpdateStatisticsSales(DateTime date);

        Task UpdateStatisticsSalesProductMonth(DateTime date);

        Task<List<EtStatisticsSalesProduct>> GetStatisticsSalesProduct(DateTime startTime, DateTime endTime);

        Task<List<EtStatisticsSalesProductMonth>> GetEtStatisticsSalesProductMonth(DateTime startTime, DateTime endTime);

        Task<Tuple<IEnumerable<EtStatisticsSalesProductMonth>, int>> GetEtStatisticsSalesProductMonthPaging(RequestPagingBase request);
    }
}
