using ETMS.Entity.Common;
using ETMS.Entity.Database.Source;
using ETMS.Entity.Temp.View;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.IDataAccess
{
    public interface IJobAnalyze2DAL : IBaseDAL
    {
        Task<Tuple<IEnumerable<UserPagingView>, int>> GetUserPaging(RequestPagingBase request);

        Task<Tuple<IEnumerable<StudentPagingView>, int>> GetStudentPaging(RequestPagingBase request);
    }
}
