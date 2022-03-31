using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ETMS.DataAccess.Alien.Core;
using ETMS.IDataAccess.Alien;

namespace ETMS.DataAccess.Core.Alien
{
    public abstract class DataAccessBaseAlien : IBaseAlienDAL
    {
        protected int _headId { get; set; }

        protected readonly IDbWrapperAlien _dbWrapper;

        public DataAccessBaseAlien(IDbWrapperAlien dbWrapper)
        {
            this._dbWrapper = dbWrapper;
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
