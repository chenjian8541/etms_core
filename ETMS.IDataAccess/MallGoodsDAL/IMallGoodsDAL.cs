using ETMS.Entity.CacheBucket.Mall;
using ETMS.Entity.Common;
using ETMS.Entity.Database.Source;
using ETMS.Entity.View.MallGoods;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.IDataAccess.MallGoodsDAL
{
    public interface IMallGoodsDAL : IBaseDAL
    {
        Task<bool> ExistMlGoods(string name, long id = 0);

        Task<long> GetMaxOrderIndex();

        Task<bool> AddMallGoods(EtMallGoods mlGoods, List<EtMallCoursePriceRule> mlCoursePriceRules);

        Task EditMallGoods(EtMallGoods mlGoods, List<EtMallCoursePriceRule> mlCoursePriceRules);

        Task<MallGoodsBucket> GetMallGoods(long id);

        Task<bool> DelMallGoods(long id);

        Task<bool> UpdateOrderIndex(long id, long newOrderIndex);

        Task<Tuple<IEnumerable<MallGoodsSimpleView>, int>> GetPagingSimple(RequestPagingBase request);

        Task<Tuple<IEnumerable<MallGoodsComplexView>, int>> GetPagingComplex(RequestPagingBase request);
    }
}
