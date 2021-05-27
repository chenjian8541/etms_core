using ETMS.DataAccess.Core;
using ETMS.DataAccess.Lib;
using ETMS.Entity.CacheBucket.EtmsManage;
using ETMS.Entity.Database.Manage;
using ETMS.Entity.Enum;
using ETMS.Entity.Enum.EtmsManage;
using ETMS.Entity.EtmsManage.Common;
using ETMS.ICache;
using ETMS.IDataAccess.EtmsManage;
using ETMS.Utility;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.DataAccess.EtmsManage
{
    public class SysSmsTemplateDAL : BaseCacheDAL<SysSmsTemplateBucket>, ISysSmsTemplateDAL, IEtmsManage
    {
        public SysSmsTemplateDAL(ICacheProvider cacheProvider) : base(cacheProvider)
        {
        }

        protected override async Task<SysSmsTemplateBucket> GetDb(params object[] keys)
        {
            var tenantId = keys[0].ToInt();
            var logs = await this.FindList<SysSmsTemplate>(p => p.IsDeleted == EmIsDeleted.Normal && p.TenantId == tenantId
            && p.HandleStatus == EmSysSmsTemplateHandleStatus.Pass);
            return new SysSmsTemplateBucket()
            {
                SysSmsTemplates = logs
            };
        }

        public async Task<SysSmsTemplate> GetSysSmsTemplateByTypeDb(int tenantId, int type)
        {
            return await this.Find<SysSmsTemplate>(p => p.TenantId == tenantId && p.Type == type && p.IsDeleted == EmIsDeleted.Normal);
        }

        public async Task<SysSmsTemplate> GetSysSmsTemplate(int id)
        {
            return await this.Find<SysSmsTemplate>(p => p.Id == id && p.IsDeleted == EmIsDeleted.Normal);
        }

        public async Task<bool> SaveSysSmsTemplate(SysSmsTemplate entity)
        {
            if (entity.Id > 0)
            {
                await this.Update(entity);
                await UpdateCache(entity.TenantId);
                return true;
            }
            var log = await this.Find<SysSmsTemplate>(p => p.TenantId == entity.TenantId && p.Type == entity.Type && p.IsDeleted == EmIsDeleted.Normal);
            if (log != null)
            {
                log.SmsContent = entity.SmsContent;
                log.HandleStatus = entity.HandleStatus;
                log.HandleContent = entity.HandleContent;
                await this.Update(log);
                await UpdateCache(log.TenantId);
                return true;
            }
            await this.Insert(entity);
            await UpdateCache(entity.TenantId);
            return true;
        }

        public async Task<List<SysSmsTemplate>> GetSysSmsTemplates(int tenantId)
        {
            var bucket = await GetCache(tenantId);
            return bucket?.SysSmsTemplates;
        }

        public async Task<List<SysSmsTemplate>> GetSysSmsTemplatesAll(int tenantId)
        {
            return await this.FindList<SysSmsTemplate>(p => p.IsDeleted == EmIsDeleted.Normal && p.TenantId == tenantId);
        }

        public async Task<bool> DelSysSmsTemplate(int id)
        {
            var db = await GetSysSmsTemplate(id);
            if (db != null)
            {
                db.IsDeleted = EmIsDeleted.Deleted;
                await this.Update(db);
                await UpdateCache(db.TenantId);
            }
            return true;
        }

        public async Task<Tuple<IEnumerable<SysSmsTemplate>, int>> GetPaging(AgentPagingBase request)
        {
            return await this.ExecutePage<SysSmsTemplate>("SysSmsTemplate", "*", request.PageSize, request.PageCurrent, $"case when HandleStatus = {EmSysSmsTemplateHandleStatus.Unreviewed} then 1 else 2 end,id desc", request.ToString());
        }
    }
}
