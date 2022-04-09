using ETMS.Entity.Common;
using ETMS.Entity.Database.Source;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.IDataAccess
{
    public interface IStatisticsStudentCountDAL : IBaseDAL
    {
        Task UpdateStatisticsStudentCountDay(DateTime time);

        Task<List<EtStatisticsStudentCount>> GetStatisticsStudentCount(DateTime startTime, DateTime endTime);

        Task<Tuple<IEnumerable<EtStatisticsStudentCount>, int>> GetStatisticsStudentCountPaging(IPagingRequest request);

        Task UpdateStatisticsStudentCountMonth(DateTime time);

        Task<List<EtStatisticsStudentCountMonth>> GetStatisticsStudentCountMonth(DateTime startTime, DateTime endTime);

        Task<Tuple<IEnumerable<EtStatisticsStudentCountMonth>, int>> GetStatisticsStudentCountMonthPaging(IPagingRequest request);
    }
}
