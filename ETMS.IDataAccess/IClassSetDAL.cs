using ETMS.Entity.Database.Source;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.IDataAccess
{
    public interface IClassSetDAL : IBaseDAL
    {
        Task<bool> AddClassSet(EtClassSet entity);

        Task DelClassSet(long id);

        Task<List<EtClassSet>> GetAllClassSet();
    }
}
