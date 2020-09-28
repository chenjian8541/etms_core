using ETMS.Entity.CacheBucket;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.IDataAccess
{
    public interface IParentStudentDAL : IBaseDAL
    {
        Task<IEnumerable<ParentStudentInfo>> GetParentStudents(int tenantId, string phone);

        Task<IEnumerable<ParentStudentInfo>> UpdateCacheAndGetParentStudents(int tenantId, string phone);
    }
}
