using ETMS.Entity.Database.Source;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.IDataAccess
{
    public interface IStudentAccountRechargeBinderCacheDAL : IBaseDAL
    {
        Task UpdateStudentAccountRechargeBinder(long studentId);

        Task<EtStudentAccountRechargeBinder> GetStudentAccountRechargeBinder(long studentId);

        void RemoveStudentAccountRechargeBinder(long studentId);
    }
}
