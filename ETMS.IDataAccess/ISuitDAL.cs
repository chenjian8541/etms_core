using ETMS.Entity.Common;
using ETMS.Entity.Database.Source;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.IDataAccess
{
    public interface ISuitDAL : IBaseDAL
    {
        Task<bool> ExistSuit(string name, long id = 0);

        Task<bool> AddSuit(EtSuit suit, List<EtSuitDetail> suitDetails);

        Task<bool> EditSuit(EtSuit suit, List<EtSuitDetail> suitDetails);

        Task<bool> EditSuit(EtSuit suit);

        Task<Tuple<EtSuit, List<EtSuitDetail>>> GetSuit(long id);

        Task<bool> DelSuit(long id);

        Task<Tuple<IEnumerable<EtSuit>, int>> GetPaging(RequestPagingBase request);
    }
}
