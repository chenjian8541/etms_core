using ETMS.Entity.Common;
using ETMS.Entity.Database.Source;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.IDataAccess
{
    public interface IStudentAccountRechargeLogDAL : IBaseDAL
    {
        Task AddStudentAccountRechargeLog(EtStudentAccountRechargeLog log);

        Task<Tuple<IEnumerable<EtStudentAccountRechargeLog>, int>> GetPaging(RequestPagingBase request);

        Task UpdateStudentAccountRechargeLogPhone(long studentAccountRechargeId,string phone);

        Task<EtStudentAccountRechargeLog> GetAccountRechargeLogByOrderId(long orderId);
    }
}
