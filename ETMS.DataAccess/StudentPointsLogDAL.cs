using ETMS.DataAccess.Core;
using ETMS.Entity.Common;
using ETMS.Entity.Database.Source;
using ETMS.IDataAccess;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.DataAccess
{
    public class StudentPointsLogDAL : DataAccessBase, IStudentPointsLogDAL
    {
        public StudentPointsLogDAL(IDbWrapper dbWrapper) : base(dbWrapper)
        {
        }

        public async Task<bool> AddStudentPointsLog(EtStudentPointsLog log)
        {
            return await _dbWrapper.Insert(log);
        }

        public bool AddStudentPointsLog(List<EtStudentPointsLog> logs)
        {
            return _dbWrapper.InsertRange(logs);
        }

        public async Task<Tuple<IEnumerable<EtStudentPointsLog>, int>> GetPaging(IPagingRequest request)
        {
            return await _dbWrapper.ExecutePage<EtStudentPointsLog>("EtStudentPointsLog", "*", request.PageSize, request.PageCurrent, "Id DESC", request.ToString());
        }
    }
}
