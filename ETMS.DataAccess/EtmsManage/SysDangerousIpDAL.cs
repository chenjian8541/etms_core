using ETMS.DataAccess.Core;
using ETMS.Entity.Database.Manage;
using ETMS.Entity.EtmsManage.Common;
using ETMS.IDataAccess.EtmsManage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.DataAccess.EtmsManage
{
    public class SysDangerousIpDAL : ISysDangerousIpDAL, IEtmsManage
    {
        public async Task AddSysDangerousIp(SysDangerousIp entity)
        {
            await this.Insert(entity);
        }

        public async Task<Tuple<IEnumerable<SysDangerousIp>, int>> GetPaging(AgentPagingBase request)
        {
            return await this.ExecutePage<SysDangerousIp>("SysDangerousIp", "*", request.PageSize, request.PageCurrent, "Id DESC", request.ToString());
        }
    }
}
