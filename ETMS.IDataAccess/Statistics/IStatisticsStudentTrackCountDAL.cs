using ETMS.Entity.Database.Source;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.IDataAccess
{
    public interface IStatisticsStudentTrackCountDAL : IBaseDAL
    {
        Task AddStudentTrackCount(DateTime time, int addCount);

        Task DeductionStudentTrackCount(DateTime time, int deductionCount);

        Task<List<EtStatisticsStudentTrackCount>> GetStatisticsStudentTrackCount(DateTime startTime, DateTime endTime);
    }
}
