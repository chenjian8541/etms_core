using ETMS.Entity.Common;
using ETMS.Entity.Database.Source;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.IDataAccess.Statistics
{
    public interface IStatisticsEducationDAL : IBaseDAL
    {
        Task StatisticsEducationUpdate(DateTime time);

        Task<EtStatisticsEducationMonth> GetStatisticsEducationMonth(DateTime firstDate);

        Task<Tuple<IEnumerable<EtStatisticsEducationTeacherMonth>, int>> GetEtStatisticsEducationTeacherMonthPaging(IPagingRequest request);

        Task<Tuple<IEnumerable<EtStatisticsEducationClassMonth>, int>> GetEtStatisticsEducationClassMonthPaging(IPagingRequest request);

        Task<Tuple<IEnumerable<EtStatisticsEducationCourseMonth>, int>> GetEtStatisticsEducationCourseMonthPaging(IPagingRequest request);

        Task<Tuple<IEnumerable<EtStatisticsEducationStudentMonth>, int>> GetEtStatisticsEducationStudentMonthPaging(IPagingRequest request);

        Task SyncClassCategoryId(long classId, long? classCategoryId);
    }
}
