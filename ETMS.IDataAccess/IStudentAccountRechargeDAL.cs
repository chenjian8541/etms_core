using ETMS.Entity.CacheBucket;
using ETMS.Entity.Common;
using ETMS.Entity.Database.Source;
using ETMS.Entity.View.OnlyOneFiled;
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

        Task<bool> EditStudentAccountRechargePhone(long id, string newPhone, string oldPhone);

        Task<StudentAccountRechargeBucket> GetStudentAccountRecharge(string phone);

        Task<EtStudentAccountRecharge> GetStudentAccountRecharge(long id);

        Task<Tuple<IEnumerable<EtStudentAccountRecharge>, int>> GetPaging(RequestPagingBase request);

        Task<EtStudentAccountRechargeBinder> GetAccountRechargeBinderByStudentId(long studentId);

        Task<bool> StudentAccountRechargeBinderAdd(string phone, EtStudentAccountRechargeBinder entity);

        Task<bool> StudentAccountRechargeBinderRemove(string phone, long rechargeBinderId,long studentId);

        Task<IEnumerable<OnlyOneFiledStudentId>> GetStudentAccountRechargeStudentIds(long studentAccountRechargeId);

        Task UpdatetStudentAccountRechargeStudentIds(long studentAccountRechargeId, string newRelationStudentIds);
    }
}
