using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.IDataAccess.Statistics
{
    public interface IStatisticsEducationDAL: IBaseDAL
    {
        Task StatisticsEducationUpdate(DateTime time);
    }
}
