using ETMS.DataAccess.Core;
using ETMS.DataAccess.Lib;
using ETMS.Entity.CacheBucket.EtmsManage;
using ETMS.Entity.Database.Manage;
using ETMS.Entity.Enum;
using ETMS.ICache;
using ETMS.IDataAccess.EtmsManage;
using ETMS.Utility;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using ETMS.Entity.EtmsManage.Common;

namespace ETMS.DataAccess.EtmsManage
{
    public class SysExplainDAL : BaseCacheDAL<SysExplainBucket>, ISysExplainDAL, IEtmsManage
    {
        public SysExplainDAL(ICacheProvider cacheProvider) : base(cacheProvider)
        {
        }

        protected override async Task<SysExplainBucket> GetDb(params object[] keys)
        {
            var type = keys[0].ToInt();
            var data = await this.ExecuteObject<SysExplain>($"SELECT TOP 5 * FROM [SysExplain] WHERE IsDeleted = {EmIsDeleted.Normal} AND [Type] = {type} ORDER BY Id DESC ");
            return new SysExplainBucket()
            {
                SysExplains = data.ToList()
            };
        }

        public async Task<bool> AddSysExplain(SysExplain entity)
        {
            await this.Insert(entity);
            await UpdateCache(entity.Type);
            return true;
        }

        public async Task<bool> EditSysExplain(SysExplain entity)
        {
            await this.Update(entity);
            await UpdateCache(entity.Type);
            return true;
        }

        public async Task<SysExplain> GetSysExplain(int id)
        {
            return await this.Find<SysExplain>(p => p.Id == id && p.IsDeleted == EmIsDeleted.Normal);
        }

        public async Task<bool> DelSysExplain(SysExplain entity)
        {
            await this.Execute($"DELETE [SysExplain] WHERE Id = {entity.Id} ");
            await UpdateCache(entity.Type);
            return true;
        }

        public async Task<Tuple<IEnumerable<SysExplain>, int>> GetPaging(AgentPagingBase request)
        {
            return await this.ExecutePage<SysExplain>("SysExplain", "*", request.PageSize, request.PageCurrent, "Id DESC", request.ToString());
        }

        public async Task<List<SysExplain>> GetSysExplainByType(int type)
        {
            var bucket = await GetCache(type);
            return bucket?.SysExplains;
        }
    }
}
