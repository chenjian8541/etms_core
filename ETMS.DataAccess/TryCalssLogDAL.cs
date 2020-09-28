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
    public class TryCalssLogDAL : DataAccessBase, ITryCalssLogDAL
    {
        public TryCalssLogDAL(IDbWrapper dbWrapper) : base(dbWrapper)
        { }

        public async Task<long> AddTryCalssLog(EtTryCalssLog log)
        {
            await _dbWrapper.Insert(log);
            return log.Id;
        }

        public async Task<bool> UpdateStatus(long tryCalssLogId, byte newStatus)
        {
            var count = await _dbWrapper.Execute($"UPDATE EtTryCalssLog SET [Status] = {newStatus} WHERE Id = {tryCalssLogId}");
            return count > 0;
        }

        public async Task<Tuple<IEnumerable<TryCalssLogView>, int>> GetPaging(RequestPagingBase request)
        {
            return await _dbWrapper.ExecutePage<TryCalssLogView>("TryCalssLogView", "*", request.PageSize, request.PageCurrent, "Id DESC", request.ToString());
        }
    }
}
