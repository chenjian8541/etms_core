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
    public class SysTryApplyLogDAL : ISysTryApplyLogDAL, IEtmsManage
    {
        public async Task AddSysTryApplyLog(SysTryApplyLog entity)
        {
            await this.Insert(entity);
        }

        public async Task<Tuple<IEnumerable<SysTryApplyLog>, int>> GetPaging(AgentPagingBase request)
        {
            return await this.ExecutePage<SysTryApplyLog>("SysTryApplyLog", "*", request.PageSize, request.PageCurrent, "Id DESC", request.ToString());
        }
    }
}
