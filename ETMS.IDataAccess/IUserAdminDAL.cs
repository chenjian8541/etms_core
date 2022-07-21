using ETMS.Entity.Database.Source;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.IDataAccess
{
    public interface IUserAdminDAL : IBaseDAL
    {
        Task<EtUser> GetAdminUser();
    }
}
