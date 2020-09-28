using ETMS.Entity.Common;
using ETMS.Entity.Database.Source;
using ETMS.Entity.View;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.IDataAccess
{
    public interface IGoodsDAL : IBaseDAL
    {
        Task<bool> ExistGoods(string name, long id = 0);

        Task<bool> AddGoods(EtGoods goods);

        Task<bool> EditGoods(EtGoods goods);

        Task<EtGoods> GetGoods(long id);

        Task<bool> DelGoods(long id);

        Task<Tuple<IEnumerable<EtGoods>, int>> GetPaging(RequestPagingBase request);

        Task<bool> AddGoodsInventoryLog(EtGoodsInventoryLog log);

        Task<bool> AddInventoryQuantity(long goodId, int addCount);

        /// <summary>
        /// 扣减库存 增加销售数量
        /// </summary>
        /// <param name="goodId"></param>
        /// <param name="addCount"></param>
        /// <returns></returns>
        Task<bool> SubtractInventoryAndAddSaleQuantity(long goodId, int count);

        Task<bool> AddInventoryAndDeductionSaleQuantity(long goodId, int count);

        Task<Tuple<IEnumerable<GoodsInventoryLogView>, int>> GetGoodsInventoryLogPaging(RequestPagingBase request);
    }
}
