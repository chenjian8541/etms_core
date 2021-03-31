using ETMS.Entity.Database.Source;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.IDataAccess
{
    public interface IStatisticsStudentCountDAL : IBaseDAL
    {
        Task AddStudentCount(DateTime time, int addCount);

        Task DeductionStudentCount(DateTime time, int deductionCount);

        Task<List<EtStatisticsStudentCount>> GetStatisticsStudentCount(DateTime startTime, DateTime endTime);
    }
}
