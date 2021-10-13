using ETMS.Entity.CacheBucket;
using ETMS.Entity.Common;
using ETMS.Entity.Database.Source;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.IDataAccess.Statistics
{
    public interface IStatisticsLcsPayDAL : IBaseDAL
    {
        Task UpdateStatisticsLcsPayDay(DateTime date);

        Task UpdateStatisticsLcsPayMonth(DateTime date);

        Task<List<EtStatisticsLcsPayDay>> GetStatisticsLcsPayDay(DateTime startTime, DateTime endTime);

        Task<List<EtStatisticsLcsPayMonth>> GetStatisticsLcsPayMonth(DateTime startTime, DateTime endTime);

        Task<Tuple<IEnumerable<EtStatisticsLcsPayDay>, int>> GetStatisticsLcsPayDayPaging(RequestPagingBase request);

        Task<Tuple<IEnumerable<EtStatisticsLcsPayMonth>, int>> GetStatisticsLcsPayMonthPaging(RequestPagingBase request);

        Task<StatisticsLcsPayYearBucket> GetStatisticsLcsPayYear(int year);
    }
}
