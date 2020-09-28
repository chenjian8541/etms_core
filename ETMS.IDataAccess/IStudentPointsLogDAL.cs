using ETMS.Entity.Common;
using ETMS.Entity.Database.Source;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.IDataAccess
{
    public interface IStudentPointsLogDAL : IBaseDAL
    {
        Task<bool> AddStudentPointsLog(EtStudentPointsLog log);

        bool AddStudentPointsLog(List<EtStudentPointsLog> logs);

        Task<Tuple<IEnumerable<EtStudentPointsLog>, int>> GetPaging(IPagingRequest request);
    }
}
