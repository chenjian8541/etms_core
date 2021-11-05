using ETMS.Entity.Database.Source;
using ETMS.Entity.View;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.IDataAccess.MallGoodsDAL
{
    public interface IMallCartDAL : IBaseDAL
    {
        Task<long> AddMallCart(EtMallCart entity);

        Task<MallCartView> GetMallCart(long id);
    }
}
