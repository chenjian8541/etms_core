using ETMS.DataAccess.Core;
using ETMS.Entity.Database.Source;
using ETMS.IDataAccess;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.DataAccess
{
    public class StudentAccountRechargeLogDAL : DataAccessBase, IStudentAccountRechargeLogDAL
    {
        public StudentAccountRechargeLogDAL(IDbWrapper dbWrapper) : base(dbWrapper)
        {
        }

        public async Task AddStudentAccountRechargeLog(EtStudentAccountRechargeLog log)
        {
            await this._dbWrapper.Insert(log);
        }
    }
}
