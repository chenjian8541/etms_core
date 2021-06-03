using ETMS.Entity.Database.Source;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.IDataAccess.MicroWeb
{
    public interface IMicroWebConfigDAL : IBaseDAL
    {
        Task<bool> SaveMicroWebConfig(EtMicroWebConfig entity);

        Task<EtMicroWebConfig> GetMicroWebConfig(int type);
    }
}
