using ETMS.Entity.Database.Source;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.IDataAccess
{
    public interface IClassCategoryDAL : IBaseDAL
    {
        Task<bool> AddClassCategory(EtClassCategory entity);

        Task DelClassCategory(long id);

        Task<List<EtClassCategory>> GetAllClassCategory();
    }
}
