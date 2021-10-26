using ETMS.Entity.CacheBucket.Mall;
using ETMS.Entity.Common;
using ETMS.Entity.Database.Source;
using ETMS.Entity.Temp;
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

        Task UpdateTagContent(List<long> ids, string newTagContent);

        Task<MallGoodsBucket> GetMallGoods(long id);

        Task<bool> DelMallGoods(long id);

        Task<MallGoodsNearOrderIndexView> GetMallGoodsNearOrderIndex(long orderIndex, int type);

        Task<bool> UpdateOrderIndex(long id, long newOrderIndex);

        Task UpdateRelatedName(byte productType, long relatedId, string newName);

        Task<Tuple<IEnumerable<MallGoodsSimpleView>, int>> GetPagingSimple(IPagingRequest request);

        Task<Tuple<IEnumerable<MallGoodsComplexView>, int>> GetPagingComplex(IPagingRequest request);
    }
}
