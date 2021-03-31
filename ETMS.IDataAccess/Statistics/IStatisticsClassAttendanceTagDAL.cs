using ETMS.Entity.View.Persistence;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.IDataAccess
{
    public interface IStatisticsClassAttendanceTagDAL: IBaseDAL
    {
        Task UpdateStatisticsClassAttendanceTag(DateTime time);

        Task<IEnumerable<StatisticsClassAttendanceTagView>> GetStatisticsClassAttendanceTag(DateTime startTime, DateTime endTime);
    }
}
