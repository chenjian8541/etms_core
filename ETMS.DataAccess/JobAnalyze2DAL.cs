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
    public class JobAnalyze2DAL : DataAccessBase, IJobAnalyze2DAL
    {
        public JobAnalyze2DAL(IDbWrapper dbWrapper) : base(dbWrapper)
        {
        }

        public async Task<Tuple<IEnumerable<EtUser>, int>> GetUserPaging(RequestPagingBase request)
        {
            return await _dbWrapper.ExecutePage<EtUser>("EtUser", "*", request.PageSize, request.PageCurrent, "Id DESC", request.ToString());
        }

        public async Task<Tuple<IEnumerable<EtStudent>, int>> GetStudentPaging(RequestPagingBase request)
        {
            return await _dbWrapper.ExecutePage<EtStudent>("EtStudent", "*", request.PageSize, request.PageCurrent, "Id DESC", request.ToString());
        }
    }
}
