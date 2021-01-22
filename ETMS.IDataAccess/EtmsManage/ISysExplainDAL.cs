using ETMS.Entity.Database.Manage;
using ETMS.Entity.EtmsManage.Common;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.IDataAccess.EtmsManage
{
    public interface ISysExplainDAL
    {
        Task<bool> AddSysExplain(SysExplain entity);

        Task<bool> EditSysExplain(SysExplain entity);

        Task<SysExplain> GetSysExplain(int id);

        Task<bool> DelSysExplain(SysExplain entity);

        Task<Tuple<IEnumerable<SysExplain>, int>> GetPaging(AgentPagingBase request);

        Task<List<SysExplain>> GetSysExplainByType(int type);
    }
}
