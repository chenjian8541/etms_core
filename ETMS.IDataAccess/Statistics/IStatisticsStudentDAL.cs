using ETMS.Entity.Database.Source;
using ETMS.Entity.View.Persistence;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.IDataAccess
{
    public interface IStatisticsStudentDAL : IBaseDAL
    {
        Task SaveStatisticsStudent(int type, string contentData);

        Task<List<EtStatisticsStudent>> GetStatisticsStudent();

        Task<EtStatisticsStudent> GetStatisticsStudent(int type);

        Task<IEnumerable<StatisticsStudentType>> GetStatisticsStudentType();
    }
}
