using ETMS.Entity.Database.Manage;
using ETMS.Entity.EtmsManage.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.IDataAccess.EtmsManage
{
    public interface ISysExternalConfigDAL
    {
        Task AddSysExternalConfig(SysExternalConfig entity);

        Task EditSysExternalConfig(SysExternalConfig entity);

        Task<SysExternalConfig> GetSysExternalConfigById(int id);

        Task<SysExternalConfig> GetSysExternalConfigByType(int type);

        Task DelSysExternalConfig(SysExternalConfig entity);

        Task<Tuple<IEnumerable<SysExternalConfig>, int>> GetPaging(AgentPagingBase request);

        Task<List<SysExternalConfig>> GetSysExternalConfigs();
    }
}
