using ETMS.DataAccess.Core;
using ETMS.DataAccess.Core.Alien;
using ETMS.DataAccess.Lib;
using ETMS.Entity.CacheBucket.Alien;
using ETMS.Entity.Common;
using ETMS.Entity.Database.Alien;
using ETMS.Entity.Enum;
using ETMS.Entity.View.Alien;
using ETMS.ICache;
using ETMS.IDataAccess.Alien;
using ETMS.Utility;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;

namespace ETMS.DataAccess.Alien
{
    public class MgOrganizationDAL : DataAccessBaseAlien<MgOrganizationBucket>, IMgOrganizationDAL
    {
        public MgOrganizationDAL(IDbWrapper dbWrapper, ICacheProvider cacheProvider) : base(dbWrapper, cacheProvider)
        {
        }

        private List<MgOrganization> allOrganizations;

        protected override async Task<MgOrganizationBucket> GetDb(params object[] keys)
        {
            allOrganizations = await _dbWrapper.FindList<MgOrganization>(p => p.HeadId == _headId && p.IsDeleted == EmIsDeleted.Normal);
            return new MgOrganizationBucket()
            {
                AllOrganization = allOrganizations,
                MgOrganizationView = GetChild(0)
            };
        }

        private List<MgOrganizationView> GetChild(long parentId)
        {
            var myViews = new List<MgOrganizationView>();
            var myOrganizations = allOrganizations.Where(p => p.ParentId == parentId);
            if (!myOrganizations.Any())
            {
                return myViews;
            }
            foreach (var p in myOrganizations)
            {
                myViews.Add(new MgOrganizationView()
                {
                    Id = p.Id,
                    Name = p.Name,
                    ParentId = p.ParentId,
                    ParentsAll = p.ParentsAll,
                    Remark = p.Remark,
                    UserCount = p.UserCount,
                    Child = GetChild(p.Id)
                });
            }
            return myViews;
        }

        public async Task<MgOrganizationBucket> GetOrganizationBucket()
        {
            return await GetCache();
        }

        public async Task<MgOrganization> GetOrganization(long id)
        {
            return await _dbWrapper.Find<MgOrganization>(p => p.Id == id && p.IsDeleted == EmIsDeleted.Normal);
        }

        public async Task AddOrganization(MgOrganization entity)
        {
            await _dbWrapper.Insert(entity);
            await UpdateCache(_headId);
        }

        public async Task EditOrganization(MgOrganization entity)
        {
            await _dbWrapper.Update(entity);
            await UpdateCache(_headId);
        }

        public async Task DelOrganization(long id)
        {
            await _dbWrapper.Execute($"UPDATE MgOrganization SET IsDeleted = {EmIsDeleted.Deleted} WHERE Id = {id}");
            await _dbWrapper.Execute($"UPDATE MgOrganization SET IsDeleted = {EmIsDeleted.Deleted} WHERE HeadId = {_headId} AND IsDeleted = {EmIsDeleted.Normal} AND ParentsAll LIKE '%,{id},%'");
            await UpdateCache(_headId);
        }
    }
}
