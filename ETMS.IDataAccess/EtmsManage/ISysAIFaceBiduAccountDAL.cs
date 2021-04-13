using ETMS.Entity.Database.Manage;
using ETMS.Entity.EtmsManage.Common;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.IDataAccess.EtmsManage
{
    public interface ISysAIFaceBiduAccountDAL
    {
        Task<SysAIFaceBiduAccount> GetSysAIFaceBiduAccount(int id);

        Task<int> AddSysAIFaceBiduAccount(SysAIFaceBiduAccount entity);

        Task<bool> EditSysAIFaceBiduAccount(SysAIFaceBiduAccount entity);

        Task<List<SysAIFaceBiduAccount>> GetSysAIFaceBiduAccountSystem();

        Task<List<SysAIFaceBiduAccount>> GetSysAIFaceBiduAccount();

        Task<bool> ExistAIFaceBiduAccount(string appid, int id = 0);

        Task<bool> IsCanNotDel(int id);

        Task<Tuple<IEnumerable<SysAIFaceBiduAccount>, int>> GetPaging(AgentPagingBase request);
    }
}
