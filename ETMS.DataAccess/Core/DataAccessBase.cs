using ETMS.IDataAccess;
using System;
using System.Collections.Generic;
using System.Text;
using System.Transactions;

namespace ETMS.DataAccess.Core
{
    public abstract class DataAccessBase : IBaseDAL
    {
        protected int _tenantId { get; set; }

        protected readonly IDbWrapper _dbWrapper;

        public DataAccessBase(IDbWrapper dbWrapper)
        {
            this._dbWrapper = dbWrapper;
        }

        public virtual void InitTenantId(int tenantId)
        {
            this._tenantId = tenantId;
            this._dbWrapper.InitTenant(tenantId);
        }

        public virtual void ResetTenantId(int tenantId)
        {
            this._tenantId = tenantId;
            this._dbWrapper.ResetTenant(tenantId);
        }

        protected virtual TransactionScope GetTransactionScope()
        {
            return new TransactionScope(TransactionScopeOption.Required, new TransactionOptions() { IsolationLevel = IsolationLevel.RepeatableRead });
        }
    }
}
