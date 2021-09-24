using ETMS.Entity.View;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.IDataAccess.EtmsManage.Lcs
{
    public interface ISysLcsBankDAL
    {
        Task<IEnumerable<LcsRegionsView>> GetAllBankHead();

        Task<IEnumerable<LcsRegionsView>> GetBank(string headNumber,string cityCode);
    }
}
