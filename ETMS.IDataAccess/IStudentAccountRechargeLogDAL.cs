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
    }
}
