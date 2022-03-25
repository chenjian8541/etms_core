using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ETMS.IDataAccess.Alien;

namespace ETMS.DataAccess.Core.Alien
{
    public abstract class DataAccessBaseAlien : IBaseAlienDAL
    {
        protected int _headId { get; set; }

        private int _tenantId { get; set; } = -1;

        protected readonly IDbWrapper _dbWrapper;

        public DataAccessBaseAlien(IDbWrapper dbWrapper)
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
