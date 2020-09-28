using ETMS.Entity.View.Persistence;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.IDataAccess
{
    public interface IStatisticsStudentSourceDAL : IBaseDAL
    {
        Task UpdateStatisticsStudentSource(DateTime time);

        Task<IEnumerable<StatisticsStudentSourceView>> GetStatisticsStudentSource(DateTime startTime, DateTime endTime);
    }
}
