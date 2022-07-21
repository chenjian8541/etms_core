using ETMS.DataAccess.Core;
using ETMS.Entity.CacheBucket;
using ETMS.Entity.Common;
using ETMS.Entity.Database.Source;
using ETMS.Entity.Enum;
using ETMS.Entity.Temp;
using ETMS.Entity.View;
using ETMS.ICache;
using ETMS.IDataAccess;
using ETMS.Utility;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Linq;

namespace ETMS.DataAccess
{
    public class UserAdminDAL : DataAccessBase<AdminUserBucket>, IUserAdminDAL
    {
        public UserAdminDAL(IDbWrapper dbWrapper, ICacheProvider cacheProvider) : base(dbWrapper, cacheProvider)
        {
        }

        protected async override Task<AdminUserBucket> GetDb(params object[] keys)
        {
            var adminUser = await _dbWrapper.Find<EtUser>(p => p.TenantId == _tenantId && p.IsDeleted == EmIsDeleted.Normal
            && p.IsAdmin == true);
            if (adminUser == null)
            {
                //防止admin账号被删除，正常情况下administrator是不允许被删除的
                adminUser = await _dbWrapper.Find<EtUser>(p => p.TenantId == _tenantId && p.IsDeleted == EmIsDeleted.Normal);
            }
            return new AdminUserBucket()
            {
                AdminUser = adminUser
            };
        }

        public async Task<EtUser> GetAdminUser()
        {
            var bucket = await GetCache(_tenantId);
            return bucket.AdminUser;
        }
    }
}
