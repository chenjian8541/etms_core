using ETMS.Entity.View;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.IDataAccess.EtmsManage.Lcs
{
    public interface ISysLcswAreaRegionDAL
    {
        Task<IEnumerable<LcsRegionsView>> GetRegion(string cityId);
    }
}
