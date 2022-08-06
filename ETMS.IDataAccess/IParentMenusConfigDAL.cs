using ETMS.Entity.Config;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.IDataAccess
{
    public interface IParentMenusConfigDAL : IBaseDAL
    {
        Task<List<ParentMenuConfigOutput>> GetParentMenuConfig();

        Task UpdateParentMenuConfig();

        void ClearMenuConfig();
    }
}
