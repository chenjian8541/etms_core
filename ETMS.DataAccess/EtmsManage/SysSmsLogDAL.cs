using ETMS.DataAccess.Core;
using ETMS.Entity.Database.Manage;
using ETMS.Entity.Enum;
using ETMS.Entity.EtmsManage.Common;
using ETMS.Entity.EtmsManage.View;
using ETMS.IDataAccess.EtmsManage;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
namespace ETMS.DataAccess.EtmsManage
{
    public class SysSmsLogDAL : ISysSmsLogDAL, IEtmsManage
    {
        public async Task AddSysSmsLog(List<SysSmsLog> smsLogs)
        {
            await this.InsertRange(smsLogs);
        }

        public async Task<Tuple<IEnumerable<SysSmsLog>, int>> GetPaging(AgentPagingBase request)
        {
            return await this.ExecutePage<SysSmsLog>("SysSmsLog", "*", request.PageSize, request.PageCurrent, "Id DESC", request.ToString());
        }
    }
}
