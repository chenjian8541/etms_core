using ETMS.DataAccess.Core;
using ETMS.Entity.Common;
using ETMS.Entity.Database.Source;
using ETMS.Entity.View;
using ETMS.IDataAccess;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.DataAccess
{
    public class ComDAL : DataAccessBase, IComDAL
    {
        public ComDAL(IDbWrapper dbWrapper) : base(dbWrapper)
        { }

        public async Task ExecuteSql(string sql)
        {
            await _dbWrapper.Execute(sql);
        }
    }
}
