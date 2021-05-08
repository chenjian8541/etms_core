using ETMS.DataAccess.Core;
using ETMS.Entity.Common;
using ETMS.Entity.Database.Source;
using ETMS.Entity.Enum;
using ETMS.ICache;
using ETMS.IDataAccess;
using ETMS.Utility;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.DataAccess
{
    public class TryCalssApplyLogDAL : DataAccessBase, ITryCalssApplyLogDAL
    {
        public TryCalssApplyLogDAL(IDbWrapper dbWrapper) : base(dbWrapper)
        {
        }

        public async Task<EtTryCalssApplyLog> GetTryCalssApplyLog(long id)
        {
            return await _dbWrapper.Find<EtTryCalssApplyLog>(id);
        }

        public async Task<bool> AddTryCalssApplyLog(EtTryCalssApplyLog log)
        {
            return await _dbWrapper.Insert(log);
        }

        public async Task<bool> EditTryCalssApplyLog(EtTryCalssApplyLog log)
        {
            return await _dbWrapper.Update(log);
        }

        public async Task<Tuple<IEnumerable<EtTryCalssApplyLog>, int>> GetPaging(RequestPagingBase request)
        {
            return await _dbWrapper.ExecutePage<EtTryCalssApplyLog>("EtTryCalssApplyLog", "*", request.PageSize, request.PageCurrent, $"case when HandleStatus = {EmTryCalssApplyHandleStatus.Unreviewed} then 1 else 2 end,id desc", request.ToString());
        }
    }
}
