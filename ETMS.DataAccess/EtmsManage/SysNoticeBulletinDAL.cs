using ETMS.DataAccess.Core;
using ETMS.DataAccess.Lib;
using ETMS.Entity.CacheBucket.EtmsManage;
using ETMS.Entity.Database.Manage;
using ETMS.ICache;
using ETMS.IDataAccess.EtmsManage;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using ETMS.Entity.EtmsManage.Common;
using ETMS.Entity.Enum;
using ETMS.Entity.Enum.EtmsManage;

namespace ETMS.DataAccess.EtmsManage
{
    public class SysNoticeBulletinDAL : BaseCacheDAL<SysNoticeBulletinBucket>, ISysNoticeBulletinDAL, IEtmsManage
    {
        public SysNoticeBulletinDAL(ICacheProvider cacheProvider) : base(cacheProvider)
        {
        }

        protected override async Task<SysNoticeBulletinBucket> GetDb(params object[] keys)
        {
            var log = await this.Find<SysNoticeBulletin>(p => p.Status == EmNoticeBulletinStatus.Normal);
            return new SysNoticeBulletinBucket()
            {
                NoticeBulletin = log
            };
        }

        public async Task<Tuple<IEnumerable<SysNoticeBulletin>, int>> GetPaging(AgentPagingBase request)
        {
            return await this.ExecutePage<SysNoticeBulletin>("SysNoticeBulletin", "*", request.PageSize, request.PageCurrent, "Id DESC", request.ToString());
        }

        public async Task AddSysNoticeBulletin(SysNoticeBulletin entity)
        {
            await this.Execute($"UPDATE [SysNoticeBulletin] SET [Status] = {EmNoticeBulletinStatus.Invalid} WHERE [Status] = {EmNoticeBulletinStatus.Normal}");
            await this.Insert(entity);
            await UpdateCache();
        }

        public async Task DelSysNoticeBulletin(long id)
        {
            await this.Execute($"DELETE [SysNoticeBulletin] WHERE id = {id} ;");
            await UpdateCache();
        }

        public async Task<SysNoticeBulletin> GetUsableLog()
        {
            var bucket = await GetCache();
            return bucket?.NoticeBulletin;
        }

        public async Task SetUserRead(int id, int tenantId, long userId)
        {
            await this.Insert(new SysNoticeBulletinRead()
            {
                IsDeleted = EmIsDeleted.Normal,
                ReadTime = DateTime.Now,
                Remark = string.Empty,
                TenantId = tenantId,
                BulletinId = id,
                UserId = userId
            });
        }

        public async Task<bool> GetUserIsRead(int id, int tenantId, long userId)
        {
            var log = await this.Find<SysNoticeBulletinRead>(p => p.BulletinId == id && p.TenantId == tenantId
            && p.UserId == userId && p.IsDeleted == EmIsDeleted.Normal);
            return log != null;
        }
    }
}
