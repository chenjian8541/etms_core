using ETMS.Entity.Database.Source;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.IDataAccess
{
    public interface IGiftCategoryDAL : IBaseDAL
    {
        Task<bool> AddGiftCategory(EtGiftCategory entity);

        Task DelGiftCategory(long id);

        Task<List<EtGiftCategory>> GetAllGiftCategory();
    }
}
