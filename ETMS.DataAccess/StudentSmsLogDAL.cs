using ETMS.DataAccess.Core;
using ETMS.Entity.Common;
using ETMS.Entity.Database.Source;
using ETMS.IDataAccess;
using ETMS.Utility;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.DataAccess
{
    public class StudentSmsLogDAL : DataAccessBase, IStudentSmsLogDAL
    {
        public StudentSmsLogDAL(IDbWrapper dbWrapper) : base(dbWrapper)
        { }

        public async Task AddStudentSmsLog(List<EtStudentSmsLog> logs)
        {
            this._dbWrapper.InsertRange(logs);
            await DelOldLogs();
        }

        /// <summary>
        /// 删除一年前的记录
        /// </summary>
        /// <returns></returns>
        private async Task DelOldLogs()
        {
            await this._dbWrapper.Execute($"DELETE EtStudentSmsLog WHERE Ot <= '{DateTime.Now.AddYears(-1).EtmsToDateString()}'");
        }

        public async Task<Tuple<IEnumerable<EtStudentSmsLog>, int>> GetPaging(IPagingRequest request)
        {
            return await _dbWrapper.ExecutePage<EtStudentSmsLog>("EtStudentSmsLog", "*", request.PageSize, request.PageCurrent, "Id DESC", request.ToString());
        }
    }
}
