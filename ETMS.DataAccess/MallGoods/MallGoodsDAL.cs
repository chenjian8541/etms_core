using ETMS.DataAccess.Core;
using ETMS.Entity.CacheBucket.Mall;
using ETMS.Entity.Common;
using ETMS.Entity.Database.Source;
using ETMS.Entity.Enum;
using ETMS.Entity.Temp;
using ETMS.Entity.View.MallGoods;
using ETMS.ICache;
using ETMS.IDataAccess.MallGoodsDAL;
using ETMS.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.DataAccess.MallGoods
{
    public class MallGoodsDAL : DataAccessBase<MallGoodsBucket>, IMallGoodsDAL
    {
        public MallGoodsDAL(IDbWrapper dbWrapper, ICacheProvider cacheProvider) : base(dbWrapper, cacheProvider)
        {
        }

        protected override async Task<MallGoodsBucket> GetDb(params object[] keys)
        {
            var id = keys[1].ToLong();
            var mallGoods = await _dbWrapper.Find<EtMallGoods>(p => p.TenantId == _tenantId && p.Id == id && p.IsDeleted == EmIsDeleted.Normal);
            if (mallGoods == null)
            {
                return null;
            }
            var mallCoursePriceRules = await _dbWrapper.FindList<EtMallCoursePriceRule>(p => p.MallGoodsId == mallGoods.Id
            && p.TenantId == _tenantId && p.IsDeleted == EmIsDeleted.Normal);
            return new MallGoodsBucket()
            {
                MallGoods = mallGoods,
                MallCoursePriceRules = mallCoursePriceRules
            };
        }

        public async Task<bool> ExistMlGoods(string name, long id = 0)
        {
            var mlGoods = await _dbWrapper.Find<EtMallGoods>(p => p.TenantId == _tenantId && p.Name == name && p.Id != id && p.IsDeleted == EmIsDeleted.Normal);
            return mlGoods != null;
        }

        public async Task<long> GetMaxOrderIndex()
        {
            var obj = await _dbWrapper.ExecuteScalar("SELECT TOP 1 Id FROM EtMallGoods ORDER BY Id DESC");
            if (obj == null)
            {
                return 0;
            }
            return obj.ToLong();
        }

        public async Task<bool> AddMallGoods(EtMallGoods mlGoods, List<EtMallCoursePriceRule> mlCoursePriceRules)
        {
            await _dbWrapper.Insert(mlGoods);
            var gid = EtmsHelper2.GetIdEncrypt(mlGoods.Id);
            await _dbWrapper.Execute($"UPDATE EtMallGoods SET GId = '{gid}' WHERE Id = {mlGoods.Id}");
            if (mlCoursePriceRules != null && mlCoursePriceRules.Any())
            {
                foreach (var s in mlCoursePriceRules)
                {
                    s.MallGoodsId = mlGoods.Id;
                }
                _dbWrapper.InsertRange(mlCoursePriceRules);
            }
            await base.UpdateCache(_tenantId, mlGoods.Id);
            return true;
        }

        public async Task EditMallGoods(EtMallGoods mlGoods, List<EtMallCoursePriceRule> mlCoursePriceRules)
        {
            await _dbWrapper.Update(mlGoods);
            await _dbWrapper.Execute($"DELETE EtMallCoursePriceRule WHERE TenantId = {_tenantId} AND MallGoodsId = {mlGoods.Id} ");
            if (mlCoursePriceRules != null && mlCoursePriceRules.Any())
            {
                _dbWrapper.InsertRange(mlCoursePriceRules);
            }
            await base.UpdateCache(_tenantId, mlGoods.Id);
        }

        public async Task UpdateTagContent(List<long> ids, string newTagContent)
        {
            if (ids == null || ids.Count == 0)
            {
                return;
            }
            if (ids.Count == 1)
            {
                await _dbWrapper.Execute($"UPDATE EtMallGoods SET TagContent = '{newTagContent}' WHERE Id = {ids[0]} ");
                RemoveCache(_tenantId, ids[0]);
            }
            else
            {
                await _dbWrapper.Execute($"UPDATE EtMallGoods SET TagContent = '{newTagContent}' WHERE Id IN ({string.Join(',', ids)}) ");
                foreach (var id in ids)
                {
                    RemoveCache(_tenantId, id);
                }
            }
        }

        public async Task<MallGoodsBucket> GetMallGoods(long id)
        {
            return await base.GetCache(_tenantId, id);
        }

        public async Task<bool> DelMallGoods(long id)
        {
            await _dbWrapper.Execute($"UPDATE EtMallGoods SET IsDeleted = {EmIsDeleted.Deleted} WHERE id = {id};DELETE EtMallCoursePriceRule WHERE MallGoodsId = {id};");
            base.RemoveCache(_tenantId, id);
            return true;
        }

        public async Task<MallGoodsNearOrderIndexView> GetMallGoodsNearOrderIndex(long orderIndex, int type)
        {
            if (type == 1)
            {
                var obj = await _dbWrapper.ExecuteObject<MallGoodsNearOrderIndexView>(
                    $"select top 1 Id,OrderIndex from EtMallGoods WHERE TenantId = {_tenantId} AND [IsDeleted] = {EmIsDeleted.Normal} AND OrderIndex > {orderIndex} ORDER BY OrderIndex ASC");
                return obj.FirstOrDefault();
            }
            else
            {
                var obj = await _dbWrapper.ExecuteObject<MallGoodsNearOrderIndexView>(
                    $"select top 1 Id,OrderIndex from EtMallGoods WHERE TenantId = {_tenantId} AND [IsDeleted] = {EmIsDeleted.Normal} AND OrderIndex < {orderIndex} ORDER BY OrderIndex DESC");
                return obj.FirstOrDefault();
            }
        }

        public async Task<bool> UpdateOrderIndex(long id, long newOrderIndex)
        {
            await _dbWrapper.Execute($"UPDATE EtMallGoods SET OrderIndex = {newOrderIndex} WHERE Id = {id}");
            base.RemoveCache(_tenantId, id);
            return true;
        }

        public async Task UpdateRelatedName(byte productType, long relatedId, string newName)
        {
            await _dbWrapper.Execute($"UPDATE EtMallGoods SET RelatedName = '{newName}' WHERE TenantId = {_tenantId} AND ProductType = {productType} AND RelatedId = {relatedId} AND IsDeleted = {EmIsDeleted.Normal}");
        }

        public async Task<Tuple<IEnumerable<MallGoodsSimpleView>, int>> GetPagingSimple(IPagingRequest request)
        {
            return await _dbWrapper.ExecutePage<MallGoodsSimpleView>("EtMallGoods", "Id,Points,GId,ProductType,ProductTypeDesc,RelatedId,Name,OrderIndex,OriginalPrice,Price,PriceDesc,ImgCover,OriginalPriceDesc,RelatedName,TagContent", request.PageSize, request.PageCurrent, "OrderIndex DESC", request.ToString());
        }

        public async Task<Tuple<IEnumerable<MallGoodsComplexView>, int>> GetPagingComplex(IPagingRequest request)
        {
            return await _dbWrapper.ExecutePage<MallGoodsComplexView>("EtMallGoods", "Id,Points,GId,ProductType,ProductTypeDesc,RelatedId,Name,OrderIndex,OriginalPrice,Price,PriceDesc,ImgCover,RelatedName,TagContent,OriginalPriceDesc,SpecContent", request.PageSize, request.PageCurrent, "OrderIndex DESC", request.ToString());
        }
    }
}
