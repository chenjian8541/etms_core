using ETMS.Entity.View.Persistence;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.IDataAccess
{
    public interface IStatisticsSalesCourseDAL : IBaseDAL
    {
        Task UpdateStatisticsSalesCourse(DateTime time);

        Task<IEnumerable<GetStatisticsSalesCourseByAmountView>> GetStatisticsSalesCourseForAmount(DateTime startTime, DateTime endTime, int topLimit);

        Task<IEnumerable<GetStatisticsSalesCourseForCountView>> GetStatisticsSalesCourseForCount(DateTime startTime, DateTime endTime, int topLimit, byte bugUnit);
    }
}
