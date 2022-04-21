using ETMS.Entity.Common;
using ETMS.Entity.Database.Source;
using ETMS.Entity.Temp;
using ETMS.Entity.Temp.View;
using ETMS.Entity.View.Statis;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.IDataAccess
{
    public interface IJobAnalyze2DAL : IBaseDAL
    {
        Task<Tuple<IEnumerable<UserPagingView>, int>> GetUserPaging(RequestPagingBase request);

        Task<Tuple<IEnumerable<StudentPagingView>, int>> GetStudentPaging(RequestPagingBase request);

        Task<IEnumerable<TempStudentTypeCount>> GetStudentTypeCount();

        Task<int> GetUserCount();

        Task<int> GetTeahcerOfWork();

        Task<int> GetOrderCount();

        Task<int> GetClassRecordCount();

        Task<IEnumerable<TempClassCount>> GetClassCount();

        Task<IEnumerable<IncomeLogGroupType>> GetIncomeLogGroupType(DateTime startDate,DateTime endDate);

        Task<StatisticsClassTimesView> GetStatisticsClassTimes(DateTime startDate, DateTime endDate);

        Task<int> GetStudentBuyCourseCount(DateTime startDate, DateTime endDate);

        Task<decimal> GetStudentBuyCourseSum(DateTime startDate, DateTime endDate);

        Task<decimal> GetTenantSurplusClassTimes();

        Task<decimal> GetTenantSurplusSurplusMoney();
    }
}
