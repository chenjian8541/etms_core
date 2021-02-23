using ETMS.DataAccess.Core;
using ETMS.Entity.Common;
using ETMS.Entity.Database.Source;
using ETMS.Entity.Temp.View;
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

        public async Task<Tuple<IEnumerable<UserPagingView>, int>> GetUserPaging(RequestPagingBase request)
        {
            return await _dbWrapper.ExecutePage<UserPagingView>("EtUser", "Id,Phone", request.PageSize, request.PageCurrent, "Id DESC", request.ToString());
        }

        public async Task<Tuple<IEnumerable<StudentPagingView>, int>> GetStudentPaging(RequestPagingBase request)
        {
            return await _dbWrapper.ExecutePage<StudentPagingView>("EtStudent", "Id,Phone,PhoneBak", request.PageSize, request.PageCurrent, "Id DESC", request.ToString());
        }
    }
}
