using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.IDataAccess.EtmsManage
{
    public interface IEtmsSourceDAL : IBaseDAL
    {
        void InitEtmsSourceData(int tenantId, string tenantName, string userName, string userPhone);
    }
}
