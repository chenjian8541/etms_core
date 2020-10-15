using ETMS.DataAccess.Core;
using ETMS.DataAccess.Lib;
using ETMS.IDataAccess.EtmsManage;
using ETMS.LOG;
using ETMS.Utility;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.DataAccess.EtmsManage
{
    public class EtmsSourceDAL : DataAccessBase, IEtmsSourceDAL
    {
        public EtmsSourceDAL(IDbWrapper dbWrapper) : base(dbWrapper)
        {
        }

        public void InitEtmsSourceData(int tenantId, string tenantName, string userName, string userPhone)
        {
            var initSql = EtmsSourceScript.InitSql;
            var processSql = string.Format(initSql, tenantId, tenantName, userName, userPhone);
            Log.Debug(processSql, this.GetType());
            var sqls = processSql.Split("GO");
            using (var ts = GetTransactionScope())
            {
                foreach (var s in sqls)
                {
                    if (string.IsNullOrEmpty(s.Trim()))
                    {
                        continue;
                    }
                    this._dbWrapper.ExecuteSync(s);
                }
                ts.Complete();
            }
        }
    }
}
