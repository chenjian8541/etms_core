using ETMS.DataAccess.Lib;
using ETMS.Entity.Config;
using ETMS.ICache;
using ETMS.IDataAccess;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.DataAccess.Core
{
    public abstract class DataAccessBase<T> : BaseCacheDAL<T>, IBaseDAL where T : class, ICacheDataContract, new()
    {
        protected readonly IDbWrapper _dbWrapper;

        public DataAccessBase(IDbWrapper dbWrapper, ICacheProvider cacheProvider) : base(cacheProvider)
        {
            this._dbWrapper = dbWrapper;
        }

        public virtual void InitTenantId(int tenantId)
        {
            base._tenantId = tenantId;
            this._dbWrapper.InitTenant(tenantId);
        }

        public virtual void ResetTenantId(int tenantId)
        {
            base._tenantId = tenantId;
            this._dbWrapper.ResetTenant(tenantId);
        }
    }
}
