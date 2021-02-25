using ETMS.Entity.Common;
using ETMS.Entity.Database.Source;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.IDataAccess
{
    public interface IStudentAccountRechargeDAL : IBaseDAL
    {
        Task<bool> ExistStudentAccountRecharge(string phone, long id = 0);

        Task<bool> AddStudentAccountRecharge(EtStudentAccountRecharge entity);

        Task<bool> EditStudentAccountRecharge(EtStudentAccountRecharge entity);

        Task<EtStudentAccountRecharge> GetStudentAccountRecharge(string phone);

        Task<EtStudentAccountRecharge> GetStudentAccountRecharge(long id);

        Task<Tuple<IEnumerable<EtStudentAccountRecharge>, int>> GetPaging(RequestPagingBase request);
    }
}
