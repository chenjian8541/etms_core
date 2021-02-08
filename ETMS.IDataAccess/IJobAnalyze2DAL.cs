using ETMS.Entity.Common;
using ETMS.Entity.Database.Source;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.IDataAccess
{
    public interface IJobAnalyze2DAL : IBaseDAL
    {
        Task<Tuple<IEnumerable<EtUser>, int>> GetUserPaging(RequestPagingBase request);

        Task<Tuple<IEnumerable<EtStudent>, int>> GetStudentPaging(RequestPagingBase request);
    }
}
