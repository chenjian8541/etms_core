using ETMS.DataAccess.Lib;
using ETMS.Entity.Config;
using ETMS.ICache;
using ETMS.IDataAccess;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using ETMS.IDataAccess.Alien;

namespace ETMS.DataAccess.Core.Alien
{
    public abstract class DataAccessBaseAlien<T> : BaseCacheAlienDAL<T>, IBaseAlienDAL where T : class, ICacheDataContract, new()
    {
        protected int _headId { get; set; }

        private int _tenantId { get; set; } = -1;

        protected readonly IDbWrapper _dbWrapper;

        public DataAccessBaseAlien(IDbWrapper dbWrapper, ICacheProvider cacheProvider) : base(cacheProvider)
        {
            this._dbWrapper = dbWrapper;
            this._dbWrapper.InitTenant(_tenantId);
        }

        public virtual void InitHeadId(int headId)
        {
            this._headId = headId;
        }

        public virtual void ResetHeadId(int headId)
        {
            this._headId = headId;
        }
    }
}
